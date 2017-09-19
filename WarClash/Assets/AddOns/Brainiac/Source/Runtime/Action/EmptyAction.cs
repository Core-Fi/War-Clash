using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Brainiac;
using Brainiac.Serialization;

[AddNodeMenu("Action/EmptyAction")]
class EmptyAction : Brainiac.Action
{
    protected override BehaviourNodeStatus OnExecute(AIAgent agent)
    {
        return BehaviourNodeStatus.Success;
    }
}
