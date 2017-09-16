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
    private Character target = null;
    private List<Vector3d> _path;
    private int _pathIndex = 0;
    private Stage stage;
    private Vector3d previousPosi;
    public override void OnStart(AIAgent agent)
    {
        base.OnStart(agent);
         _path = new List<Vector3d>(5);
    }

    protected override void OnEnter(AIAgent agent)
    {
        base.OnStart(agent);
        target = agent.Blackboard.GetItem("Target") as Character;
        CacualtePath();
        if (target != null)
        {
            var speed = SceneObject.AttributeManager[AttributeType.MaxSpeed];
            SceneObject.AttributeManager.SetBase(AttributeType.Speed, speed);
            target.EventGroup.ListenEvent((int)SceneObject.SceneObjectEvent.Positionchange, OnTargetPosiChanged);
        }
    }
    protected override void OnExit(AIAgent agent)
    {
        if (target != null)
        {
            target.EventGroup.DelEvent((int)SceneObject.SceneObjectEvent.Positionchange, OnTargetPosiChanged);
        }
        SceneObject.AttributeManager.SetBase(AttributeType.Speed, 0);
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
    private void CacualtePath()
    {
        if (target != null)
        {
            previousPosi = target.Position;
            _path.Clear();
            FixedABPath path = FixedABPath.Construct(base.SceneObject.Position, target.Position, null);
            path.CacualteNow();
            ReflectionCaculator.CaculateReflectionPoints(path, _path);
            _path.Add(path.EndPoint);
            stage = Stage.StartMove;
            _pathIndex = 0;
            Vector3d moveDirection = (_path[_pathIndex] - SceneObject.Position).Normalize();
            SceneObject.Forward = moveDirection;
            stage = Stage.Moving;
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
        if(target != null && !target.IsDeath() && _path!=null)
        {
            if (LogicCore.SP.WriteToLog)
            {
                LogicCore.SP.Writer.Append(SceneObject.Id);
                LogicCore.SP.Writer.Append(SceneObject.Position.ToStringRaw());
                LogicCore.SP.Writer.Append("    ");
                LogicCore.SP.Writer.Append(SceneObject.Forward.ToStringRaw());
                LogicCore.SP.Writer.AppendLine();
            }
            
            long distance = Vector3d.Distance(target.Position, agent.SceneObject.Position);
            if(distance< GetRange())
            {
                return BehaviourNodeStatus.Success;
            }
            long moveDistance = agent.SceneObject.GetAttributeValue(Logic.AttributeType.Speed).Mul(FixedMath.One / 15);
            while (moveDistance > 0 && _pathIndex < _path.Count)
            {
                long nextCornerDistance = Vector3d.Distance(_path[_pathIndex],
                    agent.SceneObject.Position);
                if (nextCornerDistance < moveDistance)
                {
                    agent.SceneObject.Position = _path[_pathIndex];
                    moveDistance -= nextCornerDistance;
                    _pathIndex++;
                    if (_pathIndex == _path.Count)
                    {
                        break;
                    }
                    else
                    {
                        Vector3d moveDirection = (_path[_pathIndex] - agent.SceneObject.Position).Normalize();
                        SceneObject.Forward = moveDirection;
                    }
                }
                else
                {
                    agent.SceneObject.Position += agent.SceneObject.Forward * moveDistance;
                    break;
                }
            }
            return BehaviourNodeStatus.Running;
        }
        return BehaviourNodeStatus.Failure;
	}
}