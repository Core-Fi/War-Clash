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
    private Vector3d target { get; set; }

    private Vector3d Target
    {
        get { return target; }
        set
        {
            target = value;
            _so.Forward = (target - Self.Position).Normalize();
        }
    }

    public int Radius;
    public Formation Formation;
    private bool _firstStart;
    public bool Finish { get; private set; }
    private SceneObject _so;
    protected override void OnInit()
    {
        base.OnInit();
        _index = 0;
        Finish = false;
        _firstStart = true;
        _so = Self as SceneObject;
    }
    protected override void OnStart()
    {
        base.OnStart();
    }
    bool Arrive()
    {
        if (Vector3d.SqrDistance(Target, Self.Position) < FixedMath.One/10 || Vector3d.Dot(_so.Forward, Target - Self.Position) < 0)
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
                Target = Path[_index];
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
            Self.Position = Target;
            GridService.Clear(Target, Self as SceneObject);
            _index++;
            if (_index == Path.Count)
            {
                GridService.TagAs(Target, Self as SceneObject, NodeType.BeTaken);
                Finish = true;
                return;
            }
            Target = Path[_index];
        }
        if (Vector3d.SqrDistance(Self.Position, _finalTarget) <= FixedMath.Create(Radius))
        {
            _index = Path.Count - 1;
        }
        if (_index == Path.Count - 1)
        {
            if (GridService.IsNotEmptyBy(Target) != Self )//|| (_caculatedIndex&(_index+1)) == 0)
            {
              //  _caculatedIndex += _index + 1;
                Vector3d t;
                if (Formation == Formation.Quad)
                {
                    GridService.SearchNearEmptyPoint(Path[_index], out t);
                }
                else
                {
                    GridService.SearchNearCircleEmptyPoint(Self.Position, Path[_index], Radius, out t);
                }
                Target = t;
                GridService.TagAs(Target, Self as SceneObject, NodeType.FlagAsTarget);
            }
        }
        Vector3d desiredVelocity = _so.Forward * Self.Speed;
        var nextPosi = Self.Position + desiredVelocity * LockFrameMgr.FixedFrameTime;
        if (!JPSAStar.active.IsWalkable(nextPosi))
        {
            List<PathFinderNode> list = new List<PathFinderNode>();
            JPSAStar.active.AStarFindPath(Self.Position, Target, list);
            for (int i = 0; i < list.Count-1; i++)
            {
                Path.Insert(0, JPSAStar.active.P2V(list[i]));
            }
            Target = Path[0];
        }
        Self.Position += desiredVelocity * LockFrameMgr.FixedFrameTime;
        UnityEngine.Debug.DrawLine(Self.Position.ToVector3(), Target.ToVector3(), Color.red, 0.1f);
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
