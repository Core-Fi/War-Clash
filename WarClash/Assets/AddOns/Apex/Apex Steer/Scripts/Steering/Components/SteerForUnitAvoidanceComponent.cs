using System;
using System.Collections.Generic;
using Lockstep;

namespace Apex.Steering.Components
{
    using Apex.Common;
    using Apex.DataStructures;
    using Apex.Units;
    using Apex.Utilities;
    using UnityEngine;

    /// <summary>
    /// A steering component that enables units to avoid other units, which are not in the same transient unit group.
    /// Requires the SteeringScanner component to be found on the unit.
    /// </summary>
    [RequireComponent(typeof(SteeringScanner))]
    [AddComponentMenu("Apex/Navigation/Steering/Steer for Unit Avoidance")]
    [ApexComponent("Steering")]
    public class SteerForUnitAvoidanceComponent : SteeringComponent, ISupportRuntimeStateChange
    {
        /// <summary>
        /// How large a margin should be added to the considered radius.
        /// </summary>
        [MinCheck(0.01f, label = "Radius Margin", tooltip = "A margin added to the unit's radius when it considers whether it overlaps with another unit.")]
        public float radiusMargin = 0.1f;

        /// <summary>
        /// Whether to accumulate steering vectors (true) or only avoid the first colliding other unit (false).
        /// If false, the SteeringScanner must sort units with distance.
        /// When true, groups generally form lanes when avoiding other groups. When false, groups generally interweave when avoiding other groups.
        /// </summary>
        [Label("Accumulate Avoid Vectors", "Whether to accumulate all avoid vectors (true), or only avoid the first perceived unit (false). If false, the SteeringScanner on this unit must sort units with distance. When true, groups generally form lanes when avoiding other groups. When false, groups generally interweave when avoiding other groups.")]
        public bool accumulateAvoidVectors = false;

        /// <summary>
        /// The angle at which a perpendicular is computed to the otherwise backwards-directed fleeing avoid vector.
        /// </summary>
        [RangeX(90.01f, 180f, label = "Head-On Collision Angle", tooltip = "The angle in degrees used for evaluating whether a collision is head-on, i.e if the angle between unit's velocities is greater than this value in degrees, the collision is considered head-on")]
        public float headOnCollisionAngle = 175f;

        /// <summary>
        /// The minimum avoid vector magnitude required, otherwise the avoid vector is ignored.
        /// </summary>
        [MinCheck(0.001f, label = "Minimum Avoid Vector Magnitude", tooltip = "A factor defining the minimum magnitude that an avoidance vector must have, otherwise it is ignored")]
        public float minimumAvoidVectorMagnitude = 0.25f;

        /// <summary>
        /// When true this unit will attempt to avoid behind other units, instead of avoiding in front of them.
        /// </summary>
        [Tooltip("When true this unit will attempt to avoid behind other units, instead of avoiding in front of them.")]
        public bool preventPassingInFront = false;

        /// <summary>
        /// Whether to draw the avoidance vector and the last avoided position as gizmos in real-time.
        /// </summary>
        [Tooltip("If true, draws the latest avoidance vector and the last avoided position as gizmos.")]
        public bool drawGizmos = false;

        /// <summary>
        /// A hardcoded value defining how far into the collision in time that the avoidance vector is computed at.
        /// 0.25 = 25 % inside the collision time interval.
        /// </summary>
        private const float _collisionTimeFactor = 0.25f;

        [SerializeField, AttributeProperty("Ignored Units", "Defines which types of units are ignored by this unit in relation to unit avoidance")]
        private int _ignoredUnits;

        private SteeringScanner _scanner;
        private float _fovReverseAngleCos;
        private float _omniAwareRadius;
        private float _cosAvoidAngle;
        private IUnitFacade _unitData;

        private Vector3 _selfCollisionPos;
        private Vector3d _fixedSelfCollisionPos;

        private Vector3 _lastAvoidVector;
        private Vector3 _lastAvoidPos;

        /// <summary>
        /// Gets the attribute mask that defines the attributes for which this unit will ignore other units.
        /// </summary>
        /// <value>
        /// The attribute mask with ignored unit types.
        /// </value>
        public AttributeMask ignoredUnits
        {
            get { return _ignoredUnits; }
            set { _ignoredUnits = value; }
        }

        /// <summary>
        /// Called on Start
        /// </summary>
        protected override void Start()
        {

            base.Start();

            _scanner = this.GetComponent<SteeringScanner>();
            if (_scanner == null)
            {
                Debug.LogError(this.gameObject.name + " is missing its SteeringScanner component");
            }

            _unitData = this.GetUnitFacade();

            float deg2Rad = Mathf.Deg2Rad;
            _fovReverseAngleCos = Mathf.Cos(((360f - _unitData.fieldOfView) / 2f) * deg2Rad);
            _omniAwareRadius = _unitData.radius * 2f;
            _cosAvoidAngle = Mathf.Cos(this.headOnCollisionAngle * deg2Rad);
        }

        /// <summary>
        /// Gets the desired steering output.
        /// </summary>
        /// <param name="input">The steering input containing relevant information to use when calculating the steering output.</param>
        /// <param name="output">The steering output to be populated.</param>
        public override void GetDesiredSteering(SteeringInput input, SteeringOutput output)
        {
            _selfCollisionPos = Vector3.zero;
            _lastAvoidVector = Vector3.zero;
            _lastAvoidPos = Vector3.zero;
            _fixedSelfCollisionPos = Vector3d.zero;
            var otherUnits = _scanner.units;
            int othersCount = otherUnits.count;
            if (othersCount == 0)
            {
                // if the scanner has found no units to avoid, exit
                return;
            }

            _unitData = input.unit;

            Vector3 avoidVector = Avoid(otherUnits, othersCount, input.currentPlanarVelocity);
            if (avoidVector.sqrMagnitude < (this.minimumAvoidVectorMagnitude * this.minimumAvoidVectorMagnitude))
            {
                // if the computed avoid vector's magnitude is less than the minimumAvoidVectorMagnitude, then discard it
                return;
            }
            
            // apply the avoidance force as a full deceleration capped force (not over time)
            Vector3 steeringVector = Vector3.ClampMagnitude(avoidVector / Time.fixedDeltaTime, input.maxDeceleration);

            _lastAvoidVector = steeringVector;
            output.desiredAcceleration = steeringVector;
            List<IFixedAgent> neighbors = new List<IFixedAgent>();
            for (int i = 0; i < otherUnits.count; i++)
            {
                var u = otherUnits[i];
                SteeringTest st = new SteeringTest();
                st.Position = new Vector3d(u.position);
                st.Velocity = new Vector3d(u.velocity);
                st.Radius = FixedMath.Create(u.radius);
                neighbors.Add(st);
            }
            var self = new SteeringTest();
            self.Position = new Vector3d(_unitData.position);
            self.Radius = FixedMath.Create(_unitData.radius);
            self.Velocity = new Vector3d(_unitData.velocity);
            UnitAvoidSteering uas = new UnitAvoidSteering();
            uas.Self = self;
            uas._selfCollisionPos = Vector3d.zero;
            var testAvoidVector = uas.Avoid(neighbors, neighbors.Count, self.Velocity);
            
            var fixedAvoidVector = Avoid(otherUnits, othersCount, new Vector3d(input.currentFullVelocity));
            if (Vector3.Distance(avoidVector, fixedAvoidVector.ToVector3()) > 0.1f)
            {
                Debug.LogError(avoidVector+"    "+fixedAvoidVector.ToVector3());
            }
            _lastAvoidVector =
                Vector3d.ClampMagnitude(fixedAvoidVector.Div(FixedMath.Create(Time.fixedDeltaTime)), FixedMath.Create(input.maxDeceleration)).ToVector3();

            output.desiredAcceleration = _lastAvoidVector;

        }

        /// <summary>
        /// Avoids the specified units.
        /// </summary>
        /// <param name="units">The units list.</param>
        /// <param name="unitsLength">Length of the units list.</param>
        /// <param name="currentVelocity">This unit's current velocity.</param>
        /// <returns>An avoid vector, if there are any to avoid, otherwise Vector3.zero.</returns>
        private Vector3 Avoid(IIterable<IUnitFacade> units, int unitsLength, Vector3 currentVelocity)
        {
            Vector3 selfPos = _unitData.position;
            Vector3 normalVelocity = currentVelocity.normalized;
            Vector3 combinedAvoidVector = Vector3.zero;

            // iterate through scanned units list
            for (int i = 0; i < unitsLength; i++)
            {
                var other = units[i];
                if (!other.isAlive)
                {
                    continue;
                }

                if ((_ignoredUnits & other.attributes) > 0)
                {
                    // other unit is found in the ignored attributes, so ignore it
                    continue;
                }

                if (_unitData.transientGroup != null && object.ReferenceEquals(other.transientGroup, _unitData.transientGroup))
                {
                    // ignore units in same transient unit group
                    continue;
                }

                if (other.determination < _unitData.determination)
                {
                    // ignore units with lower determination
                    continue;
                }

                Vector3 otherPos = other.position;
                Vector3 direction = otherPos.DirToXZ(selfPos);
                float distance = direction.magnitude;
                if (distance > _omniAwareRadius && Vector3.Dot(normalVelocity, direction / distance) > _fovReverseAngleCos)
                {
                    // the other unit is behind me and outside my 'omni aware radius', ignore it
                    continue;
                }

                float combinedRadius = _unitData.radius + other.radius + radiusMargin;
                Vector3 otherVelocity = other.velocity;
                Vector3 avoidVector = GetAvoidVector(selfPos, currentVelocity, normalVelocity, _unitData, otherPos, otherVelocity, other, combinedRadius);
                if (accumulateAvoidVectors)
                {
                  
                    // if accumulating, then keep summing avoid vectors up
                    combinedAvoidVector += avoidVector;
                }
                else
                {
                    // if not accumulating, then break after the first avoid vector is found
                    combinedAvoidVector = avoidVector;
                    break;
                }
            }

            return combinedAvoidVector;
        }

        /// <summary>
        /// Gets an avoidance vector.
        /// </summary>
        /// <param name="selfPos">This unit's position.</param>
        /// <param name="currentVelocity">This unit's current velocity.</param>
        /// <param name="normalVelocity">This unit's normalized current velocity.</param>
        /// <param name="unitData">This unit's UnitFacade.</param>
        /// <param name="otherPos">The other unit's position.</param>
        /// <param name="otherVelocity">The other unit's velocity.</param>
        /// <param name="otherData">The other unit's UnitFacade.</param>
        /// <param name="combinedRadius">The combined radius.</param>
        /// <returns>An avoidance vector from the other unit's collision position to this unit's collision position - if a collision actually is detected.</returns>
        private Vector3 GetAvoidVector(Vector3 selfPos, Vector3 currentVelocity, Vector3 normalVelocity, IUnitFacade unitData, Vector3 otherPos, Vector3 otherVelocity, IUnitFacade otherData, float combinedRadius)
        {
            Vector3 avoidDirection = GetAvoidDirectionVector(selfPos, currentVelocity, otherPos, otherVelocity, combinedRadius);
            float avoidMagnitude = avoidDirection.magnitude;
            if (avoidMagnitude == 0f)
            {
                // if there is absolutely no magnitude to the found avoid direction, then ignore it
                return Vector3.zero;
            }

            float vectorLength = combinedRadius * 0.5f;
            if (vectorLength <= 0f)
            {
                // if the units' combined radius is 0, then we cannot avoid
                return Vector3.zero;
            }

            // normalize the avoid vector and then set it's magnitude to the desired vector length (half of the combined radius)
            Vector3 avoidNormalized = (avoidDirection / avoidMagnitude);
            Vector3 avoidVector = avoidNormalized * vectorLength;

            float dotAngle = Vector3.Dot(avoidNormalized, normalVelocity);
            if (dotAngle <= _cosAvoidAngle)
            {
                // the collision is considered "head-on", thus we compute a perpendicular avoid vector instead
                avoidVector = new Vector3(avoidVector.z, avoidVector.y, -avoidVector.x);
            }
            else if (preventPassingInFront && (otherData.determination > unitData.determination) && (Vector3.Dot(otherVelocity, avoidVector) > 0f && Vector3.Dot(currentVelocity, otherVelocity) >= 0f))
            {
                // if supposed to be preventing front-passing, then check whether we should prevent it in this case and if so compute a different avoid vector
                avoidVector = _selfCollisionPos.DirToXZ(selfPos);
            }

            // scale the avoid vector depending on the distance to collision, shorter distances need larger magnitudes and vice versa
            float collisionDistance = Mathf.Max(1f, selfPos.DirToXZ(_selfCollisionPos).magnitude);
            avoidVector *= currentVelocity.magnitude / collisionDistance;

            return avoidVector;
        }

        /// <summary>
        /// Gets the avoid direction vector.
        /// </summary>
        /// <param name="selfPos">This unit's position.</param>
        /// <param name="currentVelocity">This unit's current velocity.</param>
        /// <param name="otherPos">The other unit's position.</param>
        /// <param name="otherVelocity">The other unit's velocity.</param>
        /// <param name="combinedRadius">The combined radius.</param>
        /// <returns>An avoidance direction vector, if a collision is detected.</returns>
        private Vector3 GetAvoidDirectionVector(Vector3 selfPos, Vector3 currentVelocity, Vector3 otherPos, Vector3 otherVelocity, float combinedRadius)
        {
            // use a 2nd degree polynomial function to determine intersection points between moving units with a velocity and radius
            float a = ((currentVelocity.x - otherVelocity.x) * (currentVelocity.x - otherVelocity.x)) +
                      ((currentVelocity.z - otherVelocity.z) * (currentVelocity.z - otherVelocity.z));
            float b = (2f * (selfPos.x - otherPos.x) * (currentVelocity.x - otherVelocity.x)) +
                      (2f * (selfPos.z - otherPos.z) * (currentVelocity.z - otherVelocity.z));
            float c = ((selfPos.x - otherPos.x) * (selfPos.x - otherPos.x)) +
                      ((selfPos.z - otherPos.z) * (selfPos.z - otherPos.z)) -
                      (combinedRadius * combinedRadius);
           

            float d = (b * b) - (4f * a * c);
            if (d <= 0f)
            {
                // if there are not 2 intersection points, then skip
                return Vector3.zero;
            }

            // compute "heavy" calculations only once
            float dSqrt = Mathf.Sqrt(d);
            float doubleA = 2f * a;

            // compute roots, which in this case are actually time values informing of when the collision starts and ends
            float t1 = (-b + dSqrt) / doubleA;
            float t2 = (-b - dSqrt) / doubleA;
            if (t1 < 0f && t2 < 0f)
            {
                // if both times are negative, the collision is behind us (compared to velocity direction)
                return Vector3.zero;
            }

            // find the lowest non-negative time, since this will be where the collision time interval starts
            float time = 0f;
            if (t1 < 0f)
            {
                time = t2;
            }
            else if (t2 < 0f)
            {
                time = t1;
            }
            else
            {
                time = Mathf.Min(t1, t2);
            }
           
            // the collision time we want is actually 25 % within the collision
            time += Mathf.Abs(t2 - t1) * _collisionTimeFactor;

            // compute actual collision positions
            Vector3 selfCollisionPos = selfPos + (currentVelocity * time);
            _selfCollisionPos = selfCollisionPos;
            Vector3 otherCollisionPos = otherPos + (otherVelocity * time);
            _lastAvoidPos = otherPos;

            // return an avoid vector from the other's collision position to this unit's collision position
            return otherCollisionPos.DirToXZ(selfCollisionPos);
        }
        private Vector3d Avoid(IIterable<IUnitFacade> units, int unitsLength, Vector3d currentVelocity)
        {
            Vector3d normalVelocity = currentVelocity.Normalize();
            Vector3d combinedAvoidVector = Vector3d.zero;
            for (int i = 0; i < unitsLength; i++)
            {
                var other = units[i] as IUnitFacade;
                var selfPosi = new Vector3d(_unitData.position);
                var direction = selfPosi - new Vector3d(other.position);
                var distance = direction.magnitude;
                if (distance > FixedMath.Create(_omniAwareRadius) && Vector3d.Dot(normalVelocity, (direction / distance)) > FixedMath.Create(_fovReverseAngleCos))
                {
                    continue;
                }
                long combinedRadius = FixedMath.Create(_unitData.radius + other.radius + radiusMargin);
                Vector3d avoidVector = GetAvoidVector(new Vector3d (_unitData.position), currentVelocity, normalVelocity, new Vector3d(other.position),
                    new Vector3d(other.velocity), combinedRadius);
                
                combinedAvoidVector += avoidVector;
            }
            return combinedAvoidVector;
        }

        private Vector3d GetAvoidVector(Vector3d selfPosi, Vector3d currentVelocity, Vector3d normalVelocity,
            Vector3d otherPosi, Vector3d otherVelocity, long combinedRadius)
        {
            Vector3d avoidDirection = GetAvoidDirectionVector(selfPosi, currentVelocity, otherPosi, otherVelocity, combinedRadius);
            if (avoidDirection.sqrMagnitude == 0) return Vector3d.zero;
            long avoidMagnitude = avoidDirection.magnitude;
            long vectorLength = combinedRadius / 2;
            if (vectorLength <= 0) return Vector3d.zero;
            Vector3d avoidNormalized = (avoidDirection / (avoidMagnitude));
            Vector3d avoidVector = avoidNormalized * vectorLength;
            long dotAngle = Vector3d.Dot(avoidNormalized, normalVelocity);
            if (dotAngle <= FixedMath.Create(_cosAvoidAngle))
            {
                // the collision is considered "head-on", thus we compute a perpendicular avoid vector instead
                avoidVector = new Vector3d(avoidVector.z, avoidVector.y, -avoidVector.x);
            }
            else if (false && (Vector3d.Dot(otherVelocity, avoidVector) > 0 && Vector3d.Dot(currentVelocity, otherVelocity) >= 0))
            {
                // if supposed to be preventing front-passing, then check whether we should prevent it in this case and if so compute a different avoid vector
                avoidVector = _fixedSelfCollisionPos - selfPosi;
            }
            long collisionDistance = Math.Max(FixedMath.One, (_fixedSelfCollisionPos - selfPosi).magnitude);
            avoidVector = avoidVector * (currentVelocity.magnitude.Div(collisionDistance));
            return avoidVector;
        }
        private Vector3d GetAvoidDirectionVector(Vector3d selfPos, Vector3d currentVelocity, Vector3d otherPos, Vector3d otherVelocity, long combinedRadius)
        {
            long a = ((currentVelocity.x - otherVelocity.x).Mul(currentVelocity.x - otherVelocity.x)) +
                      ((currentVelocity.z - otherVelocity.z).Mul(currentVelocity.z - otherVelocity.z));
            long b = (2 * (selfPos.x - otherPos.x).Mul(currentVelocity.x - otherVelocity.x)) +
                      (2 * (selfPos.z - otherPos.z).Mul(currentVelocity.z - otherVelocity.z));
            long c = ((selfPos.x - otherPos.x).Mul(selfPos.x - otherPos.x)) +
                      ((selfPos.z - otherPos.z).Mul(selfPos.z - otherPos.z)) -
                      (combinedRadius.Mul(combinedRadius));
            long d = (b.Mul(b)) - (4 * a.Mul(c));
           
            if (d <= 0)
            {
                // if there are not 2 intersection points, then skip
                return Vector3d.zero;
            }
            // compute "heavy" calculations only once
            long dSqrt = FixedMath.Sqrt(d);
            long doubleA = 2 * a;

            // compute roots, which in this case are actually time values informing of when the collision starts and ends
            long t1 = (-b + dSqrt).Div(doubleA);
            long t2 = (-b - dSqrt).Div(doubleA);
           
            if (t1 < 0 && t2 < 0)
            {
                // if both times are negative, the collision is behind us (compared to velocity direction)
                return Vector3d.zero;
            }

            // find the lowest non-negative time, since this will be where the collision time interval starts
            long time = 0;
            if (t1 < 0)
            {
                time = t2;
            }
            else if (t2 < 0)
            {
                time = t1;
            }
            else
            {
                time = Math.Min(t1, t2);
            }
           
            // the collision time we want is actually 25 % within the collision
             time += Math.Abs(t2 - t1) /4;

            // compute actual collision positions
            Vector3d selfCollisionPos = selfPos + (currentVelocity * (time));
            Vector3d otherCollisionPos = otherPos + (otherVelocity * (time));
            _fixedSelfCollisionPos = selfCollisionPos;
            //var selfgo = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            //var othergo = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            //selfgo.name = "self";
            //othergo.name = "other";
            //selfgo.transform.position = selfCollisionPos.ToVector3();
            //othergo.transform.position = otherCollisionPos.ToVector3();
            //Debug.LogError("pause " + self.name);
            //Debug.LogError(self.name+" "+selfPos+" "+otherPos+" "+selfCollisionPos+" "+otherCollisionPos);
            // return an avoid vector from the other's collision position to this unit's collision position
            return selfCollisionPos - otherCollisionPos;
        }
        void ISupportRuntimeStateChange.ReevaluateState()
        {
            // recompute on runtime state change
            _cosAvoidAngle = Mathf.Cos(headOnCollisionAngle * Mathf.Deg2Rad);
        }

        private void OnDrawGizmos()
        {
            if (!drawGizmos)
            {
                return;
            }

            if (_unitData == null || _unitData.gameObject == null)
            {
                return;
            }

            var renderer = _unitData.gameObject.GetComponent<Renderer>();
            if (renderer == null || renderer.material == null)
            {
                return;
            }

            var c = renderer.material.color;
            Gizmos.color = c;

            if (_lastAvoidVector.sqrMagnitude != 0f)
            {
                Gizmos.DrawLine(_unitData.position, _unitData.position + _lastAvoidVector);
            }

            if (_lastAvoidPos.sqrMagnitude != 0f)
            {
                Gizmos.DrawSphere(_selfCollisionPos, 0.25f);
            }
        }
    }
}