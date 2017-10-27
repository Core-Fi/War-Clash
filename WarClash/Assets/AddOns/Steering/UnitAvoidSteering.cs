using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lockstep;
using Logic;
using Logic.LogicObject;
using RVO;
using UnityEngine;

class UnitAvoidSteering : BaseSteering
{
    private const long RadiusMargin = FixedMath.One / 10;
    private const long CosAvoidAngle = -FixedMath.One * 90 / 100;
    private Vector3d _selfCollisionPos;
    private const long FovReverseAngleCos = -FixedMath.One / 2;
    private readonly List<IFixedAgent> _neighbors = new List<IFixedAgent>();
    public long MinimumAvoidVectorMagnitude = FixedMath.One/4;
    public override Vector3d GetDesiredSteering()
    {
        if(Self.Velocity.sqrMagnitude<FixedMath.One/1000)
            return Vector3d.zero;
        _selfCollisionPos = Vector3d.zero;
        _neighbors.Clear();
        LogicCore.SP.SceneManager.CurrentScene.FixedQuadTreeForBuilding.Query(Self, FixedMath.One*2, _neighbors);
        LogicCore.SP.SceneManager.CurrentScene.FixedQuadTree.Query(Self, FixedMath.One * 4, _neighbors);
        if (_neighbors.Count == 0) return Vector3d.zero;
        var avoidVec = Avoid(_neighbors, _neighbors.Count, Self.Velocity);
        if (avoidVec.sqrMagnitude < this.MinimumAvoidVectorMagnitude.Mul(MinimumAvoidVectorMagnitude))
        {
            return Vector3d.zero;
        }
        var acc = avoidVec.Div(LockFrameMgr.FixedFrameTime);
        return acc;
    }
    private Vector3d Avoid(IList<IFixedAgent> units, int unitsLength, Vector3d currentVelocity)
    {
        Vector3d normalVelocity = currentVelocity.Normalize();
        Vector3d combinedAvoidVector = Vector3d.zero;
        for (int i = 0; i < unitsLength; i++)
        {
            var other = units[i] as IFixedAgent;
            var direction = Self.Position - other.Position;
            if (direction.sqrMagnitude> Self.Radius.Mul(Self.Radius)*4 && Vector3d.Dot(normalVelocity, (Self.Position - other.Position)) > 0)
            {
                continue;
            }
            long combinedRadius = other.Radius + RadiusMargin;
            Vector3d avoidVector = GetAvoidVector(Self.Position, currentVelocity, normalVelocity, other.Position,
                (other is ISteering)?(other as ISteering).Velocity : Vector3d.zero, combinedRadius);
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
        if(vectorLength<=0) return Vector3d.zero;
        Vector3d avoidNormalized = (avoidDirection/(avoidMagnitude));
        Vector3d avoidVector = avoidNormalized * vectorLength;
        long dotAngle = Vector3d.Dot(avoidNormalized, normalVelocity);
        if (dotAngle <= CosAvoidAngle)
        {
            // the collision is considered "head-on", thus we compute a perpendicular avoid vector instead
            avoidVector = new Vector3d(avoidVector.z, avoidVector.y, -avoidVector.x);
        }
        else if ((Vector3d.Dot(otherVelocity, avoidVector) > 0 && Vector3d.Dot(currentVelocity, otherVelocity) >= 0))
        {
            // if supposed to be preventing front-passing, then check whether we should prevent it in this case and if so compute a different avoid vector
            avoidVector = _selfCollisionPos - selfPosi;
        }
        long collisionDistance = Math.Max(FixedMath.One/3, (_selfCollisionPos-selfPosi).magnitude);
        avoidVector = avoidVector*( currentVelocity.magnitude.Div( collisionDistance));
        return avoidVector;
    }
    private Vector3d GetAvoidDirectionVector(Vector3d selfPos, Vector3d currentVelocity, Vector3d otherPos, Vector3d otherVelocity, long combinedRadius)
    {
        long a = ((currentVelocity.x - otherVelocity.x) .Mul (currentVelocity.x - otherVelocity.x)) +
                  ((currentVelocity.z - otherVelocity.z).Mul(currentVelocity.z - otherVelocity.z));
        long b = (2 * (selfPos.x - otherPos.x).Mul(currentVelocity.x - otherVelocity.x)) +
                  (2 * (selfPos.z - otherPos.z).Mul(currentVelocity.z - otherVelocity.z));
        long c = ((selfPos.x - otherPos.x).Mul(selfPos.x - otherPos.x)) +
                  ((selfPos.z - otherPos.z).Mul(selfPos.z - otherPos.z)) -
                  (combinedRadius.Mul(combinedRadius));
        long d = (b.Mul( b)) - (4 * a.Mul(c));
        if (d <= 0)
        {
            // if there are not 2 intersection points, then skip
            return Vector3d.zero;
        }
        // compute "heavy" calculations only once
        long dSqrt = FixedMath.Sqrt(d);
        long doubleA = 2 * a;

        // compute roots, which in this case are actually time values informing of when the collision starts and ends
        long t1 = (-b + dSqrt).Div( doubleA);
        long t2 = (-b - dSqrt).Div (doubleA);

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
        Vector3d selfCollisionPos = selfPos + (currentVelocity *( time));
        Vector3d otherCollisionPos = otherPos + (otherVelocity *(time));
        _selfCollisionPos = selfCollisionPos;
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
