using UnityEngine;
using System;
using Brainiac;

[AddNodeMenu("Action/Move")]
public class Move : Brainiac.Action
{
	protected override BehaviourNodeStatus OnExecute(AIAgent agent)
	{
        return BehaviourNodeStatus.Failure;
	}
}