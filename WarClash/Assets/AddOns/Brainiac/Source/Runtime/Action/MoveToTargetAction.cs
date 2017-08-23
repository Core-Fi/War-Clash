using UnityEngine;
using System;
using Brainiac;
using Logic.LogicObject;
using Lockstep;
using Logic;
using Newtonsoft.Json.Utilities;
using UnityEditor;
using UnityEngine.AI;

[AddNodeMenu("Action/MoveToTargetAction")]
public class MoveToTargetAction : Brainiac.Action
{
    private Character target = null;
    private NavMeshPath _path;
    private int _pathIndex = 0;
    public override void OnStart(AIAgent agent)
    {
        base.OnStart(agent);
        _path = new NavMeshPath();
    }

    protected override void OnEnter(AIAgent agent)
    {
        base.OnStart(agent);
        target = agent.Blackboard.GetItem("Target") as Character;
        CacualtePath();
        if (target != null)
        {
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
        CacualtePath();
    }
    private void CacualtePath()
    {
        if (target != null)
        {
            var mask = NavMesh.GetAreaFromName("Walkable");
            mask = 1 << mask;
            var success = NavMesh.CalculatePath(SceneObject.Position.ToVector3(), target.Position.ToVector3(), mask, _path);
            if (_path.corners.Length > 1)
            {
                _pathIndex = 1;
            }
        }
    }
    protected override BehaviourNodeStatus OnExecute(AIAgent agent)
	{
        if(target != null && _path!=null)
        {
            long distance = Vector3d.Distance(target.Position, agent.SceneObject.Position);
            if(distance<FixedMath.One)
            {
                return BehaviourNodeStatus.Success;
            }
            long moveDistance = agent.SceneObject.GetAttributeValue(Logic.AttributeType.Speed).Mul(FixedMath.One / 15);
            while (moveDistance > 0 && _pathIndex < _path.corners.Length)
            {
                long nextCornerDistance = Vector3d.Distance(new Vector3d(_path.corners[_pathIndex]),
                    agent.SceneObject.Position);
                Vector3d moveDirection = (new Vector3d(_path.corners[_pathIndex]) - agent.SceneObject.Position).Normalize();
                if (nextCornerDistance < moveDistance)
                {
                    agent.SceneObject.Position = new Vector3d(_path.corners[_pathIndex]);
                    moveDistance -= nextCornerDistance;
                    SceneObject.Forward = moveDirection;
                    _pathIndex++;
                }
                else
                {
                    SceneObject.Forward = moveDirection;
                    agent.SceneObject.Position += moveDirection * moveDistance;
                    break;
                }
            }
            return BehaviourNodeStatus.Running;
        }
        return BehaviourNodeStatus.Failure;
	}
}