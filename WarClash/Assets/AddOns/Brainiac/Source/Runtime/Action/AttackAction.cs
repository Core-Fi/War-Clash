using UnityEngine;
using System;
using Brainiac;
using Logic.LogicObject;
using Lockstep;
using Brainiac.Serialization;

[AddNodeMenu("Action/AttackAction")]
public class AttackAction : Brainiac.Action
{
    [BTProperty("SkillPath")]
    public MemoryVar skillPath;

    private Character target = null;
    private bool isRunningSkill = false;
    public override void OnReset()
    {
        base.OnReset();
        target = null;
        isRunningSkill = false;
    }
    protected override BehaviourNodeStatus OnExecute(AIAgent agent)
    {
        if (target == null)
            target = agent.Blackboard.GetItem("Target") as Character;
        if (target != null)
        {
            if (isRunningSkill == false)
            {
                string path = skillPath.AsString;
                bool rst = agent.Character.ReleaseSkill(path, target);
                if (!rst)
                    return BehaviourNodeStatus.Failure;
                else
                    isRunningSkill = true;
            }
            if(isRunningSkill)
            {
                if(agent.Character.IsRunningSkill)
                {
                    return BehaviourNodeStatus.Running;
                }
                else
                {
                    return BehaviourNodeStatus.Success;
                }
            }
        }
        return BehaviourNodeStatus.Failure;
    }
}