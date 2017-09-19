using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Brainiac;
using Brainiac.Serialization;

[AddNodeMenu("Action/CheckBlackboard")]
class CheckBlackboardAction : Brainiac.Action
{
    [BTProperty("BlackboardTag")]
    public MemoryVar BlackboardTag;
    [BTProperty("BlackboardValue")]
    public MemoryVar BlackboardValue;
    protected override BehaviourNodeStatus OnExecute(AIAgent agent)
    {
        var has = agent.Blackboard.HasItem<object>(BlackboardTag.AsString);
        if (has)
        {
            var item = agent.Blackboard.GetItem(BlackboardTag.AsString);
            if (item == null) return BehaviourNodeStatus.Failure;
            if (BlackboardValue.AsString.Equals(item)||
                BlackboardValue.AsBool.Equals(item) ||
                BlackboardValue.AsInt.Equals(item))
            {
                return BehaviourNodeStatus.Success;
            }
            else
            {
                return BehaviourNodeStatus.Failure;
            }
        }
        return BehaviourNodeStatus.Failure;
    }
}
