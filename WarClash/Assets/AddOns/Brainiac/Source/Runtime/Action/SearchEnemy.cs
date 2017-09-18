using UnityEngine;
using System;
using System.Collections.Generic;
using Brainiac;
using Lockstep;
using Logic;
using Logic.LogicObject;

[AddNodeMenu("Action/SearchEnemy")]
public class SearchEnemy : Brainiac.Action
{
    private SceneObject _self;
    private List<SceneObject> enemyList = new List<SceneObject>(4); 
    public override void OnStart(AIAgent agent)
    {
        base.OnStart(agent);
        _self = agent.SceneObject;
        _searchEnemy = Search;
    }
    private Logic.Objects.ObjectCollection<int, SceneObject>.VoidAction<SceneObject> _searchEnemy;
    private void Search(SceneObject c)
    {
        if (c != _self && c.Hp > 0 && c.Team != _self.Team && (c is Character))
        {
            enemyList.Add(c);
        }
    }

    protected override BehaviourNodeStatus OnExecute(AIAgent agent)
	{
        enemyList.Clear();
        Logic.LogicCore.SP.SceneManager.currentScene.ForEachDo<SceneObject>(_searchEnemy);
	    if (LogicCore.SP.WriteToLog)
	    {
	        LogicCore.SP.Writer.AppendLine(UnityEngine.Random.Range(0, 1000).ToString());
	    }
	    SceneObject target = null;
	    if (enemyList.Count > 0)
	    {
	        foreach (var enemy in enemyList)
	        {
	            if (target == null)
	            {
	                target = enemy;
	            }
	            else
	            {
	                if (Vector3d.SqrDistance(agent.SceneObject.Position, enemy.Position) <
	                    Vector3d.SqrDistance(agent.SceneObject.Position, target.Position))
	                {
	                    target = enemy;
	                }
	            }
	        }
	        //int index = UnityEngine.Random.Range(0, enemyList.Count - 1);
         //   target = enemyList[index];
	    }
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