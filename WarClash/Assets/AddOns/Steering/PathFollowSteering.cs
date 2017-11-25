using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AStar;
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
    bool Arrive()
    {
        SceneObject so = Self as SceneObject;
        if (Vector3d.SqrDistance(_target, Self.Position) < FixedMath.One/10 || Vector3d.Dot(so.Forward, _target - Self.Position) < 0)
        {
            return true;
        }
        return false;
    }

    private Vector3d _finalTarget;
    private int _caculatedIndex;
    public override void GetDesiredSteering(SteeringResult rst)
    {
       
        if (Finish)
            return;
        if (_firstStart)
        {
            _index = 0;
            if (Path!=null && Path.Count > 0)
            {
                _target = Path[_index];
                _finalTarget = Path[Path.Count - 1];
            }
            else
            {
                Finish = true;
                return;
            }
            _firstStart = false;
        }
        if (Arrive())
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
        if (Vector3d.SqrDistance(Self.Position, _finalTarget) <= FixedMath.Create(Radius))
        {
            _index = Path.Count - 1;
        }
        if (_index == Path.Count - 1)
        {
            if (GridService.IsNotEmptyBy(_target) != Self )//|| (_caculatedIndex&(_index+1)) == 0)
            {
              //  _caculatedIndex += _index + 1;
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
        var nextPosi = Self.Position + desiredVelocity * LockFrameMgr.FixedFrameTime;
        if (!JPSAStar.active.IsWalkable(nextPosi))
        {
            List<PathFinderNode> list = new List<PathFinderNode>();
            JPSAStar.active.AStarFindPath(Self.Position, _target, list);
            for (int i = 0; i < list.Count-1; i++)
            {
                Path.Insert(0, JPSAStar.active.P2V(list[i]));
            }
            _target = Path[0];
        }
        Self.Position += desiredVelocity * LockFrameMgr.FixedFrameTime;
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
