using UnityEngine;
using System;
using Brainiac;
using Logic.LogicObject;
using Lockstep;

[AddNodeMenu("Action/MoveToTargetAction")]
public class MoveToTargetAction : Brainiac.Action
{
    private Character target = null;

	protected override BehaviourNodeStatus OnExecute(AIAgent agent)
	{
        if(target == null)
            target = agent.Blackboard.GetItem("Target") as Character;
        if(target != null)
        {
            long distance = Vector3d.Distance(target.Position, agent.Character.Position);
            if(distance<FixedMath.One)
            {
                return BehaviourNodeStatus.Success;
            }
            Vector3d dir = (target.Position - agent.Character.Position);//.Normalize();
            dir.Normalize();
            agent.Character.Forward = dir;
            dir.Mul(agent.Character.GetAttributeValue(Logic.AttributeType.SPEED));
            dir.Mul(FixedMath.One.Div(FixedMath.One * 15));
            agent.Character.Position += dir;
            return BehaviourNodeStatus.Running;
        }
        return BehaviourNodeStatus.Failure;
	}
}