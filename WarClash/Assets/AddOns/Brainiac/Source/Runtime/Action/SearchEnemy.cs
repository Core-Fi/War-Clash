using UnityEngine;
using System;
using Brainiac;
using Logic.LogicObject;

[AddNodeMenu("Action/SearchEnemy")]
public class SearchEnemy : Brainiac.Action
{
    private Character self;
    public override void OnStart(AIAgent agent)
    {
        base.OnStart(agent);
        searchEnemy = Search;
    }
    private Logic.Objects.ObjectCollection<int, SceneObject>.BoolAction<Character> searchEnemy;
    private bool Search(Character c)
    {
        return c != self && !c.IsDeath();
    }

    protected override BehaviourNodeStatus OnExecute(AIAgent agent)
	{
        self = agent.Character;
        Character target = Logic.LogicCore.SP.sceneManager.currentScene.ForEachDo<Character>(searchEnemy);
        if(target!=null)
        {
            agent.Blackboard.SetItem("Target", target);
            return BehaviourNodeStatus.Success;
        }else
        {
            agent.Blackboard.SetItem("Target", null);
            return BehaviourNodeStatus.Failure;
        }
	}

}