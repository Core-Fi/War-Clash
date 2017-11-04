#define ANNOTATE_AVOIDOBSTACLES
using System.Collections.Generic;
using System.Linq;
using Lockstep;
using Logic;
using UnityEngine;


    /// <summary>
    /// Steers a vehicle to be repulsed by stationary obstacles
    /// </summary>
    /// <remarks>
    /// For every obstacle detected, this will:
    /// 1) Add up a repulsion vector that is the distance between the vehicle 
    /// and the obstacle, divided by the squared magnitude of the distance 
    /// between the obstacle and the vehicle's future intended position. This is
    /// done because the further an obstacle is from our desired position, the
    /// least we care about it (which could have side effects when dealing with
    /// very large obstacles which we don't happen to intersect, need to review).
    /// 2) If we would intersect this obstacle on our current path, then we 
    /// multiply this repulsion vector by a factor of the number of detected
    /// (since the others might just be to the side and we want to give it
    /// higher weight).
    /// 3) Divide the total by the number of obstacles.
    /// The final correction vector is the old desired velocity reflected 
    /// along the calculated avoidance vector.
    /// </remarks>
    public class SphericalObstacleSteering : BaseSteering
    {
        #region Structs

        /// <summary>
        /// Struct used to store the next likely intersection with an obstacle
        /// for a vehicle's current direction.
        /// </summary>
        public struct PathIntersection
        {
            public bool Intersect;
            public float Distance;
            public IFixedAgent Obstacle;

            public PathIntersection(IFixedAgent obstacle)
            {
                Obstacle = obstacle;
                Intersect = false;
                Distance = float.MaxValue;
            }
        };

        #endregion

        #region Private fields

        [SerializeField] private float _estimationTime = 2;

        #endregion
        #region Public properties

        /// <summary>
        /// How far in the future to estimate the vehicle position
        /// </summary>
        public float EstimationTime
        {
            get { return _estimationTime; }
            set { _estimationTime = value; }
        }

    #endregion
    private readonly List<IFixedAgent> _neighbors = new List<IFixedAgent>();

    //public Vector3d PredictFutureDesiredPosition(ISteering agent, long predictionTime)
    //{
    //    return agent.Position + (agent.Velocity * predictionTime);
    //}
    public Vector3 PredictFutureDesiredPosition(ISteering agent, float predictionTime)
    {
        return agent.Position.ToVector3() + (agent.Velocity.ToVector3() * predictionTime);
    }
    /// <summary>
    /// Calculates the force necessary to avoid the detected spherical obstacles
    /// </summary>
    /// <returns>
    /// Force necessary to avoid detected obstacles, or Vector3.zero
    /// </returns>
    /// <remarks>
    /// This method will iterate through all detected spherical obstacles that 
    /// are within MinTimeToCollision, and calculate a repulsion vector based
    /// on them.
    /// </remarks>
    public override void GetDesiredSteering(SteeringResult rst)
    {
            var avoidance = Vector3.zero;
        LogicCore.SP.SceneManager.CurrentScene.FixedQuadTreeForBuilding.Query(Self, FixedMath.One * 2, _neighbors);
        if (_neighbors.Count == 0) return ;

        /*
         * While we could just calculate movement as (Velocity * predictionTime) 
         * and save ourselves the substraction, this allows other vehicles to
         * override PredictFuturePosition for their own ends.
         */
        var futurePosition = PredictFutureDesiredPosition(Self, _estimationTime);

#if ANNOTATE_AVOIDOBSTACLES
            Debug.DrawLine(Self.Position.ToVector3(), futurePosition, Color.cyan);
#endif

            /*
             * Test all obstacles for intersection with the vehicle's future position.
             * If we find that we are going to intersect them, use their position
             * and distance to affect the avoidance - the further away the intersection
             * is, the less weight they'll carry.
             */
            UnityEngine.Profiling.Profiler.BeginSample("Accumulate spherical obstacle influences");
            for (var i = 0; i < _neighbors.Count; i++)
            {
                var sphere = _neighbors[i];
                if (sphere == null || sphere.Equals(null))
                    continue; // In case the object was destroyed since we cached it
                var next = FindNextIntersectionWithSphere(sphere, futurePosition, sphere);
                var avoidanceMultiplier = 0.1f;
                if (next.Intersect)
                {
#if ANNOTATE_AVOIDOBSTACLES
                    Debug.DrawRay(Self.Position.ToVector3(), Self.Velocity.ToVector3().normalized * next.Distance, Color.yellow);
#endif
                    var timeToObstacle = next.Distance / Self.Speed.ToInt();
                    avoidanceMultiplier = 2 * (_estimationTime / timeToObstacle);
                }

                var oppositeDirection = Self.Position - sphere.Position;
                avoidance += avoidanceMultiplier * oppositeDirection.ToVector3();
                if (Vector3.Dot(avoidance.normalized, Self.Velocity.ToVector3().normalized) < Mathf.Cos(175f * Mathf.Deg2Rad))
                {
                    avoidance = new Vector3(avoidance.z, 0, avoidance.x);
                }
            }
            UnityEngine.Profiling.Profiler.EndSample();

            avoidance /= _neighbors.Count;

            var newDesired = Vector3.Reflect(Self.Velocity.ToVector3(), avoidance);

#if ANNOTATE_AVOIDOBSTACLES
            Debug.DrawLine(Self.Position.ToVector3(), Self.Position.ToVector3() + avoidance, Color.green);
            Debug.DrawLine(Self.Position.ToVector3(), futurePosition, Color.blue);
            Debug.DrawLine(Self.Position.ToVector3(), Self.Position.ToVector3() + newDesired, Color.white);
#endif
        }

        /// <summary>
        /// Finds a vehicle's next intersection with a spherical obstacle
        /// </summary>
        /// <param name="vehicle">
        /// The vehicle to evaluate.
        /// </param>
        /// <param name="futureVehiclePosition">
        /// The position where we expect the vehicle to be soon
        /// </param>
        /// <param name="obstacle">
        /// A spherical obstacle to check against <see cref="DetectableObject"/>
        /// </param>
        /// <returns>
        /// A PathIntersection with the intersection details <see cref="PathIntersection"/>
        /// </returns>
        /// <remarks>We could probably spin out this function to an independent tool class</remarks>
        public static PathIntersection FindNextIntersectionWithSphere(IFixedAgent vehicle, Vector3 futureVehiclePosition,
            IFixedAgent obstacle)
        {
            // this mainly follows http://www.lighthouse3d.com/tutorials/maths/ray-sphere-intersection/

            var intersection = new PathIntersection(obstacle);

            var combinedRadius = vehicle.Radius.ToFloat() + obstacle.Radius.ToFloat();
            var movement = futureVehiclePosition - vehicle.Position.ToVector3();
            var direction = movement.normalized;

            var vehicleToObstacle = obstacle.Position - vehicle.Position;

            // this is the length of vehicleToObstacle projected onto direction
            var projectionLength = Vector3.Dot(direction, vehicleToObstacle.ToVector3());

            // if the projected obstacle center lies further away than our movement + both radius, we're not going to collide
            if (projectionLength > movement.magnitude + combinedRadius)
            {
                //print("no collision - 1");
                return intersection;
            }

            // the foot of the perpendicular
            var projectedObstacleCenter = vehicle.Position.ToVector3() + projectionLength * direction;

            // distance of the obstacle to the pathe the vehicle is going to take
            var obstacleDistanceToPath = (obstacle.Position.ToVector3() - projectedObstacleCenter).magnitude;
            //print("obstacleDistanceToPath: " + obstacleDistanceToPath);

            // if the obstacle is further away from the movement, than both radius, there's no collision
            if (obstacleDistanceToPath > combinedRadius)
            {
                //print("no collision - 2");
                return intersection;
            }

            // use pythagorean theorem to calculate distance out of the sphere (if you do it 2D, the line through the circle would be a chord and we need half of its length)
            var halfChord = Mathf.Sqrt(combinedRadius * combinedRadius + obstacleDistanceToPath * obstacleDistanceToPath);

            // if the projected obstacle center lies opposite to the movement direction (aka "behind")
            if (projectionLength < 0)
            {
                // behind and further away than both radius -> no collision (we already passed)
                if (vehicleToObstacle.magnitude > combinedRadius)
                    return intersection;

                var intersectionPoint = projectedObstacleCenter - direction * halfChord;
                intersection.Intersect = true;
                intersection.Distance = (intersectionPoint - vehicle.Position.ToVector3()).magnitude;
                return intersection;
            }

            // calculate both intersection points
            var intersectionPoint1 = projectedObstacleCenter - direction * halfChord;
            var intersectionPoint2 = projectedObstacleCenter + direction * halfChord;

            // pick the closest one
            var intersectionPoint1Distance = (intersectionPoint1 - vehicle.Position.ToVector3()).magnitude;
            var intersectionPoint2Distance = (intersectionPoint2 - vehicle.Position.ToVector3()).magnitude;

            intersection.Intersect = true;
            intersection.Distance = Mathf.Min(intersectionPoint1Distance, intersectionPoint2Distance);

            return intersection;
        }

#if ANNOTATE_AVOIDOBSTACLES
        private void OnDrawGizmos()
        {
            if (Self == null) return;
            foreach (var o in _neighbors)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(o.Position.ToVector3(), o.Radius);
            }
        }
#endif

    }
