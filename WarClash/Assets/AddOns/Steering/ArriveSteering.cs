using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lockstep;
using Logic;
using UnityEngine;


class ArriveSteering : BaseSteering
{
    public Vector3d Target;
    private bool _finish;
    private readonly List<IFixedAgent> _neighbors = new List<IFixedAgent>();
    public override void Start()
    {
        base.Start();
    }

    public override void GetDesiredSteering(SteeringResult rst)
    {
        if (_finish)
            return;
        if (Vector3d.SqrDistance(Target, Self.Position)<FixedMath.One/2)
        {
            _finish = true;
        }
        var t = GetDestination(Target);
        //Target = new Vector3d(t.x, 0, t.y);
        Vector3d dir = Target - Self.Position;
        Vector3d desiredVelocity = dir.Normalize()*Self.Speed;
        var acc = (desiredVelocity - Self.Velocity)/(LockFrameMgr.FixedFrameTime);
        rst.DesiredSteering = acc;
    }

    public Vector2d GetDestination(Vector3d destination)
    {
        var destXz = new Vector2d(destination.x, destination.z);
        _neighbors.Clear();
        int times = 0;
        do
        {
            destXz += testOffset[times];
            LogicCore.SP.SceneManager.CurrentScene.FixedQuadTree.Query(destXz, Self.Radius, _neighbors);
            _neighbors.Remove(Self);
            times++;
        } while (_neighbors.Count != 0);
        return destXz;
    }
    List<Vector2d> testOffset = new List<Vector2d>{
        new Vector2d(new Vector2(0,0)),
        new Vector2d(new Vector2(-1,-1)),
        new Vector2d(new Vector2(-1, -1)) ,
        new Vector2d(new Vector2(-1, 0)) ,
        new Vector2d(new Vector2(-1, 1)) ,
        new Vector2d(new Vector2(0, -1)),
        new Vector2d(new Vector2(0,1)),
        new Vector2d(new Vector2(1,-1)),
        new Vector2d(new Vector2(1,0)),
        new Vector2d(new Vector2(1,1))
    };
    public ArriveSteering(ISteering self) : base(self)
    {

    }
}
