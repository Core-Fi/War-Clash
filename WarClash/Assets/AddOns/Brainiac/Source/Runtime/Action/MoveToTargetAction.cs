using Brainiac;
using Logic.LogicObject;
using Lockstep;
using Logic;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using LockStep;
using Pathfinding;
using UnityEngine;
using UnityEngine.AI;

[AddNodeMenu("Action/MoveToTargetAction")]
public class MoveToTargetAction : Brainiac.Action
{
    private enum  Stage
    {
        CaculatingPath,
        StartMove,
        Moving,
    }
    private SceneObject target = null;
    private Npc _npc;
    private List<Vector3d> _path;
    private int _pathIndex = 0;
    private Stage stage;
    private Vector3d previousPosi;
    private bool _depart;
    private PathFollowSteering _pathFollowSteering;
    private bool _onRightPlace;
    public override void OnStart(AIAgent agent)
    {
        base.OnStart(agent);
         _path = new List<Vector3d>(5);
        _npc = agent.SceneObject as Npc;
    }

    protected override void OnEnter(AIAgent agent)
    {
        base.OnStart(agent);
        _onRightPlace = false;
        target = agent.Blackboard.GetItem("target") as SceneObject;
        if (target != null)
        {
            var speed = SceneObject.AttributeManager[AttributeType.MaxSpeed];
            SceneObject.AttributeManager.SetBase(AttributeType.Speed, speed);
            target.EventGroup.ListenEvent((int)SceneObject.SceneObjectEvent.Positionchange, OnTargetPosiChanged);
        }
        _onRightPlace = OnRightPlace();
        if (!_onRightPlace)
        {
            _pathFollowSteering = _npc.SteeringManager.AddSteering<PathFollowSteering>(2);
            CacualtePath();
        }
    }
    protected override void OnExit(AIAgent agent)
    {
        if (target != null)
        {
            target.EventGroup.DelEvent((int)SceneObject.SceneObjectEvent.Positionchange, OnTargetPosiChanged);
        }
        SceneObject.AttributeManager.SetBase(AttributeType.Speed, 0);
        _npc.SteeringManager.RemoveSteering<PathFollowSteering>();
        base.OnExit(agent);
    }
    private void OnTargetPosiChanged(object sender, EventMsg e)
    {
        var curPosi = target.Position;
        if (Vector3d.SqrDistance(curPosi, previousPosi) > 1)
        {
            CacualtePath();
        }
    }

    private bool OnRightPlace()
    {
        return GridService.OnRightPlace(_npc, target.Position, _npc.Conf.AtkRange / 100);
    }
    private void CacualtePath()
    {
        if (OnRightPlace())
        {
            return;
        }
      //  long distance = Vector3d.Distance(target.Position, SceneObject.Position);
        if (target != null )//&& distance > GetRange())
        {
            previousPosi = target.Position;
            //navimesh的接口 先注释
            //_path.Clear();
            //FixedABPath path = FixedABPath.Construct(base.SceneObject.Position, target.Position, null);
            //path.CacualteNow();
            //ReflectionCaculator.CaculateReflectionPoints(path, _path);
            //_path.Add(path.EndPoint);

            JPSAStar.active.GetPath(SceneObject.Position, target.Position, _path);
            Vector3d moveDirection = (_path[1] - SceneObject.Position).Normalize();
            SceneObject.Forward = moveDirection;
            _pathFollowSteering.Formation = Formation.Circle;
            _pathFollowSteering.Path = _path;
            _pathFollowSteering.Radius = 2;//_npc.Conf.AtkRange / 100;
        }
    }

    private long GetRange()
    {
        Npc npc = SceneObject as Npc;
        if (npc != null)
        {
            return npc.Conf.AtkRange.IntHundredToLong();
        }
        else return FixedMath.One;
    }

    protected override BehaviourNodeStatus OnExecute(AIAgent agent)
	{
        if(target != null && target.Hp>0 && _path.Count>0)
        {
#if UNITY_EDITOR
            if (LogicCore.SP.WriteToLog)
            {
                LogicCore.SP.Writer.Append(SceneObject.Id);
                LogicCore.SP.Writer.Append(SceneObject.Position.ToStringRaw());
                LogicCore.SP.Writer.Append("    ");
                LogicCore.SP.Writer.Append(SceneObject.Forward.ToStringRaw());
                LogicCore.SP.Writer.AppendLine();
            }
#endif
            if(_onRightPlace)
                return BehaviourNodeStatus.Success;
            if (_pathFollowSteering.Finish)
            {
                return BehaviourNodeStatus.Success;
            }
            //long moveDistance = agent.SceneObject.GetAttributeValue(Logic.AttributeType.Speed).Mul(FixedMath.One / 15);
            //while (moveDistance > 0 && _pathIndex < _path.Count)
            //{
            //    long nextCornerDistance = Vector3d.Distance(_path[_pathIndex],
            //        agent.SceneObject.Position);
            //    if (nextCornerDistance < moveDistance)
            //    {
            //        agent.SceneObject.Position = _path[_pathIndex];
            //        moveDistance -= nextCornerDistance;
            //        _pathIndex++;
            //        if (_pathIndex == _path.Count)
            //        {
            //            break;
            //        }
            //        else
            //        {
            //            Vector3d moveDirection = (_path[_pathIndex] - agent.SceneObject.Position).Normalize();
            //            SceneObject.Forward = moveDirection;
            //        }
            //    }
            //    else
            //    {
            //        agent.SceneObject.Position += agent.SceneObject.Forward * moveDistance;
            //        break;
            //    }
            //}
            return BehaviourNodeStatus.Running;
        }
        return BehaviourNodeStatus.Failure;
	}
}