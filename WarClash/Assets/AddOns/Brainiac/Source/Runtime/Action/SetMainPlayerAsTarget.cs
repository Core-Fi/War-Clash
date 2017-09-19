using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Brainiac;
using Logic.LogicObject;
[AddNodeMenu("Action/SetMainPlayerAsTarget")]
class SetMainPlayerAsTargetAction : global::Brainiac.Action
{
    protected override BehaviourNodeStatus OnExecute(AIAgent agent)
    {
        if (MainPlayer.SP != null)
        {
            agent.Blackboard.SetItem("target", MainPlayer.SP);
            return BehaviourNodeStatus.Success;
        }
        return BehaviourNodeStatus.Failure;
    }
}
