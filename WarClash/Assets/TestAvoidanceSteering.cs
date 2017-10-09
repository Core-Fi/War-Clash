using System;
using System.Collections;
using System.Collections.Generic;
using Lockstep;
using UnityEngine;

public class TestAvoidanceSteering : MonoBehaviour {


        
        public float radiusMargin = 0.1f;

        /// <summary>
        /// Whether to accumulate steering vectors (true) or only avoid the first colliding other unit (false).
        /// If false, the SteeringScanner must sort units with distance.
        /// When true, groups generally form lanes when avoiding other groups. When false, groups generally interweave when avoiding other groups.
        /// </summary>
        public bool accumulateAvoidVectors = true;

        public float headOnCollisionAngle = 175f;

        public float minimumAvoidVectorMagnitude = 0.25f;

     
        public bool preventPassingInFront = false;

    
        public bool drawGizmos = false;

        /// <summary>
        /// A hardcoded value defining how far into the collision in time that the avoidance vector is computed at.
        /// 0.25 = 25 % inside the collision time interval.
        /// </summary>
        private const float _collisionTimeFactor = 0.25f;

        private int _ignoredUnits;

        private float _fovReverseAngleCos;
        private float _omniAwareRadius;
        private float _cosAvoidAngle;

        private Vector3 _selfCollisionPos;
        private Vector3 _lastAvoidVector;
        private Vector3 _lastAvoidPos;
        public Test918 _unitData;
        public void Start()
        {
            float deg2Rad = Mathf.Deg2Rad;
            _omniAwareRadius = _unitData.radius * 2f;
            _cosAvoidAngle = Mathf.Cos(this.headOnCollisionAngle * deg2Rad);
        }

        /// <summary>
        /// Gets the desired steering output.
        /// </summary>
        /// <param name="input">The steering input containing relevant information to use when calculating the steering output.</param>
        /// <param name="output">The steering output to be populated.</param>
        public  Vector3 GetDesiredSteering()
        {
            _selfCollisionPos = Vector3.zero;
            _lastAvoidVector = Vector3.zero;
            _lastAvoidPos = Vector3.zero;
            var units = GameObject.FindObjectsOfType<Test918>();
            List<Test918> others = new List<Test918>();
            for (int i = 0; i < units.Length; i++)
            {
                if (units[i] != _unitData)
                    others.Add(units[i]);
            }
            int othersCount = others.Count;

            Vector3 avoidVector = Avoid(others, othersCount, _unitData.velocity.ToVector3());
            if (avoidVector.sqrMagnitude < (this.minimumAvoidVectorMagnitude * this.minimumAvoidVectorMagnitude))
            {
                // if the computed avoid vector's magnitude is less than the minimumAvoidVectorMagnitude, then discard it
                return Vector3.zero;
            }

            // apply the avoidance force as a full deceleration capped force (not over time)
            Vector3 steeringVector = avoidVector / (1/30f);

            _lastAvoidVector = steeringVector;
            return steeringVector;
        }

        /// <summary>
        /// Avoids the specified units.
        /// </summary>
        /// <param name="units">The units list.</param>
        /// <param name="unitsLength">Length of the units list.</param>
        /// <param name="currentVelocity">This unit's current velocity.</param>
        /// <returns>An avoid vector, if there are any to avoid, otherwise Vector3.zero.</returns>
        private Vector3 Avoid(List<Test918> units, int unitsLength, Vector3 currentVelocity)
        {
            return Avoid(units, unitsLength, new Vector3d(currentVelocity)).ToVector3();
            Vector3 selfPos = _unitData.transform.position;
            Vector3 normalVelocity = currentVelocity.normalized;
            Vector3 combinedAvoidVector = Vector3.zero;

            // iterate through scanned units list
            for (int i = 0; i < unitsLength; i++)
            {
                var other = units[i];

                Vector3 otherPos = other.transform.position;
                float combinedRadius =  other.radius + radiusMargin;
                Vector3 otherVelocity = other.velocity.ToVector3();
                Vector3 avoidVector = GetAvoidVector(selfPos, currentVelocity, normalVelocity, otherPos, otherVelocity, combinedRadius);
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

        private Vector3d Avoid(List<Test918> units, int unitsLength, Vector3d currentVelocity)
    {
        Vector3d selfPos = new Vector3d(_unitData.transform.position);;
        Vector3d normalVelocity = currentVelocity.Normalize();
        Vector3d combinedAvoidVector = Vector3d.zero;

        // iterate through scanned units list
        for (int i = 0; i < unitsLength; i++)
        {
            var other = units[i];

            Vector3d otherPos = new Vector3d(other.transform.position);
            long combinedRadius = FixedMath.Create( other.radius + radiusMargin);
            Vector3d otherVelocity = other.velocity;
            Vector3d avoidVector = GetAvoidVector(selfPos, currentVelocity, normalVelocity, otherPos, otherVelocity, combinedRadius);
            combinedAvoidVector += avoidVector;
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
    private Vector3 GetAvoidVector(Vector3 selfPos, Vector3 currentVelocity, Vector3 normalVelocity, Vector3 otherPos, Vector3 otherVelocity, float combinedRadius)
        {
            return GetAvoidVector(new Vector3d(selfPos), new Vector3d(currentVelocity), new Vector3d(normalVelocity), new Vector3d(otherPos), new Vector3d(otherVelocity), FixedMath.Create(combinedRadius)).ToVector3();
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
            else if ( (Vector3.Dot(otherVelocity, avoidVector) > 0f && Vector3.Dot(currentVelocity, otherVelocity) >= 0f))
            {
                // if supposed to be preventing front-passing, then check whether we should prevent it in this case and if so compute a different avoid vector
                avoidVector = selfPos - _selfCollisionPos;
            }

            // scale the avoid vector depending on the distance to collision, shorter distances need larger magnitudes and vice versa
            float collisionDistance = Mathf.Max(1f, (_selfCollisionPos - selfPos).magnitude);
            avoidVector *= currentVelocity.magnitude / collisionDistance;

            return avoidVector;
        }
    private Vector3d GetAvoidVector(Vector3d selfPosi, Vector3d currentVelocity, Vector3d normalVelocity,
        Vector3d otherPosi, Vector3d otherVelocity, long combinedRadius)
    {
        Vector3d avoidDirection = GetAvoidDirectionVector(selfPosi, currentVelocity, otherPosi, otherVelocity, combinedRadius);

        long avoidMagnitude = avoidDirection.magnitude;
        if (avoidMagnitude == 0) return Vector3d.zero;
        long vectorLength = combinedRadius / 2;
        if (vectorLength <= 0) return Vector3d.zero;
        Vector3d avoidNormalized = (avoidDirection / (avoidMagnitude));
        Vector3d avoidVector = avoidNormalized * vectorLength;
        long dotAngle = Vector3d.Dot(avoidNormalized, normalVelocity);
        if (dotAngle <= _cosAvoidAngle)
        {
            // the collision is considered "head-on", thus we compute a perpendicular avoid vector instead
            avoidVector = new Vector3d(avoidVector.z, avoidVector.y, -avoidVector.x);
        }
        else if ((Vector3d.Dot(otherVelocity, avoidVector) > 0 && Vector3d.Dot(currentVelocity, otherVelocity) >= 0))
        {
            // if supposed to be preventing front-passing, then check whether we should prevent it in this case and if so compute a different avoid vector
            avoidVector = new Vector3d(_selfCollisionPos) - selfPosi;
        }
        long collisionDistance = Math.Max(FixedMath.One, (new Vector3d(_selfCollisionPos) - selfPosi).magnitude);
        avoidVector = avoidVector * (currentVelocity.magnitude.Div(collisionDistance));
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
            return GetAvoidDirectionVector(new Vector3d(selfPos), new Vector3d(currentVelocity), new Vector3d(otherPos), new Vector3d(otherVelocity), FixedMath.Create(combinedRadius)).ToVector3();
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
            return selfCollisionPos - otherCollisionPos;
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
        // time += Math.Abs(t2 - t1) /4;

        // compute actual collision positions
        Vector3d selfCollisionPos = selfPos + (currentVelocity * (time));
        Vector3d otherCollisionPos = otherPos + (otherVelocity * (time));
        _selfCollisionPos = selfCollisionPos.ToVector3();
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
}
