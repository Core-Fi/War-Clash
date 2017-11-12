using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lockstep;
using Logic;
using Logic.LogicObject;
using UnityEngine;

public enum Formation
{
    Quad,
    Circle
}
class PathFollowSteering : BaseSteering
{
    public List<Vector3d> Path;
    private int _index;
    private Vector3d _target;
    public int Radius;
    public Formation Formation;
    private bool _firstStart;
    public bool Finish { get; private set; }
    protected override void OnInit()
    {
        base.OnInit();
        _index = 0;
        Finish = false;
        _firstStart = true;
    }

    protected override void OnStart()
    {
        base.OnStart();
    }

    public override void GetDesiredSteering(SteeringResult rst)
    {
       
        if (Finish)
            return;
        if (_firstStart)
        {
            if (Path!=null && Path.Count > 0)
            {
                _target = Path[_index];
            }
            else
            {
                Finish = true;
                return;
            }
            _firstStart = false;
        }
        if ( Vector3d.SqrDistance(_target, Self.Position) < FixedMath.One)
        {
            Self.Position = _target;
            GridService.Clear(_target, Self as SceneObject);
            _index++;
            if (_index == Path.Count)
            {
                GridService.TagAs(_target, Self as SceneObject, NodeType.BeTaken);
                Finish = true;
                return;
            }
            _target = Path[_index];
        }
        if (_index == Path.Count - 1)
        {
            if (GridService.IsNotEmptyBy(_target.x.ToInt(), _target.z.ToInt()) != Self)
            {
                if (Formation == Formation.Quad)
                {
                    GridService.SearchNearEmptyPoint(Path[_index], out _target);
                }
                else
                {
                    GridService.SearchNearCircleEmptyPoint(Self.Position, Path[_index], Radius, out _target);
                }
                GridService.TagAs(_target, Self as SceneObject, NodeType.FlagAsTarget);
            }
        }
        Vector3d dir = _target - Self.Position;
        Vector3d desiredVelocity = dir.Normalize() * Self.Speed;
        Self.Position += desiredVelocity * LockFrameMgr.FixedFrameTime;
        //var acc = (desiredVelocity - Self.Velocity) / (LockFrameMgr.FixedFrameTime);
        //rst.DesiredSteering = acc;
        
        UnityEngine.Debug.DrawLine(Self.Position.ToVector3(), _target.ToVector3(), Color.red, 0.1f);
    }

    protected override void OnExit()
    {
        base.OnExit();
        Path = null;
        _index = 0;
        Finish = false;
        _firstStart = true;
    }
}
