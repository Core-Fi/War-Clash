using UnityEngine;
using System;
using Brainiac;
using Logic.LogicObject;

[AddNodeMenu("Action/SearchEnemy")]
public class SearchEnemy : Brainiac.Action
{
    private SceneObject _self;
    public override void OnStart(AIAgent agent)
    {
        base.OnStart(agent);
        _self = agent.SceneObject;
        _searchEnemy = Search;
    }
    private Logic.Objects.ObjectCollection<int, SceneObject>.BoolAction<SceneObject> _searchEnemy;
    private bool Search(SceneObject c)
    {
        return c != _self && c.Hp>0 && c.Team!=_self.Team && (c is Character);
    }

    protected override BehaviourNodeStatus OnExecute(AIAgent agent)
	{
        SceneObject target = Logic.LogicCore.SP.SceneManager.currentScene.ForEachDo<SceneObject>(_searchEnemy);
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