using UnityEngine;
using System;
using Brainiac;
using Logic.LogicObject;
using Lockstep;
using Brainiac.Serialization;

[AddNodeMenu("Action/AttackAction")]
public class AttackAction : Brainiac.Action
{
    private Character _self;
    [BTProperty("SkillPath")]
    public MemoryVar skillid;

    private Character target = null;
    private bool isRunningSkill = false;
    public override void OnStart(AIAgent agent)
    {
        base.OnStart(agent);
        _self = agent.SceneObject as Character;
    }

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
                bool rst = _self.ReleaseSkill(skillid.AsInt.Value, target);
                if (!rst)
                    return BehaviourNodeStatus.Failure;
                else
                    isRunningSkill = true;
            }
            if(isRunningSkill)
            {
                if(_self.IsRunningSkill)
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