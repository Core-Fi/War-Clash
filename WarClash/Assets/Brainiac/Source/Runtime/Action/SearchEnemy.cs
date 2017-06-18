using UnityEngine;
using System;
using Brainiac;
using Logic.LogicObject;

[AddNodeMenu("Action/SearchEnemy")]
public class SearchEnemy : Brainiac.Action
{
    
	protected override BehaviourNodeStatus OnExecute(AIAgent agent)
	{
        Character target = Logic.LogicCore.SP.sceneManager.currentScene.ForEachDo<Character>((c)=>{

            return c!=agent.Character;

        });
        if(target!=null)
        {
            agent.Blackboard.SetItem("Target", target);
            return BehaviourNodeStatus.Success;
        }
        return BehaviourNodeStatus.Failure;
	}


}