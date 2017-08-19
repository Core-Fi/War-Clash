using UnityEngine;
using System;
using Brainiac;
using Logic.LogicObject;
using Lockstep;
using Logic;
using Newtonsoft.Json.Utilities;
using UnityEngine.AI;

[AddNodeMenu("Action/MoveToTargetAction")]
public class MoveToTargetAction : Brainiac.Action
{
    private Character target = null;
    private SceneObject self = null;
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
        self = agent.SceneObject;
        target = agent.Blackboard.GetItem("Target") as Character;
        if (target != null)
        {
            var mask = NavMesh.GetAreaFromName("Walkable");
            mask = 1 << mask;
            var success = NavMesh.CalculatePath(self.Position.ToVector3(), target.Position.ToVector3(), mask, _path);
            if (_path.corners.Length > 1)
            {
                _pathIndex = 1;
            }
            self.AttributeManager.SetBase(AttributeType.SPEED, self.AttributeManager[AttributeType.MAXSPEED]);
        }
    }

    protected override void OnExit(AIAgent agent)
    {
        base.OnExit(agent);
        self.AttributeManager.SetBase(AttributeType.SPEED, 0);
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
            long moveDistance = agent.SceneObject.GetAttributeValue(Logic.AttributeType.SPEED).Mul(FixedMath.One / 15);
            while (moveDistance > 0 && _pathIndex < _path.corners.Length)
            {
                long nextCornerDistance = Vector3d.Distance(new Vector3d(_path.corners[_pathIndex]),
                    agent.SceneObject.Position);
                Vector3d moveDirection = (new Vector3d(_path.corners[_pathIndex]) - agent.SceneObject.Position).Normalize();
                if (nextCornerDistance < moveDistance)
                {
                    agent.SceneObject.Position = new Vector3d(_path.corners[_pathIndex]);
                    moveDistance -= nextCornerDistance;
                    self.Forward = moveDirection;
                    _pathIndex++;
                }
                else
                {
                    self.Forward = moveDirection;
                    agent.SceneObject.Position += moveDirection * moveDistance;
                    break;
                }
            }
            return BehaviourNodeStatus.Running;
        }
        return BehaviourNodeStatus.Failure;
	}
}