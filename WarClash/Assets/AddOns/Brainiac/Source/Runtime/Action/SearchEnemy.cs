using UnityEngine;
using System;
using System.Collections.Generic;
using Brainiac;
using Brainiac.Serialization;
using Lockstep;
using Logic;
using Logic.LogicObject;

[AddNodeMenu("Action/SearchEnemy")]
public class SearchEnemy : Brainiac.Action
{
    private SceneObject _self;
    private List<SceneObject> enemyList = new List<SceneObject>(4);
    [BTProperty("Building As Target")]
    public MemoryVar Building;
    [BTProperty("Enemy As Target")]
    public MemoryVar Enemy;
    public override void OnStart(AIAgent agent)
    {
        base.OnStart(agent);
        _self = agent.SceneObject;
        _searchEnemy = Search;
        
    }
    private Logic.Objects.ObjectCollection<int, SceneObject>.VoidAction<SceneObject> _searchEnemy;
    private void Search(SceneObject c)
    {
        if (c != _self && c.Hp > 0 && c.Team != _self.Team)
        {
            if (Building.AsBool.Value && c is Building)
            {
                enemyList.Add(c);
            }
            else if(Enemy.AsBool.Value && c is Character)
            {
                enemyList.Add(c);
            }
        }
    }

    protected override BehaviourNodeStatus OnExecute(AIAgent agent)
	{
        enemyList.Clear();
        Logic.LogicCore.SP.SceneManager.CurrentScene.ForEachDo<SceneObject>(_searchEnemy);
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
	        int s =UnityEngine.Random.Range(0, 100);
	        int rand = LogicCore.SP.LockFrameMgr.RandomRange(0, enemyList.Count - 1);
            target = enemyList[rand];
            LogicCore.SP.Write(rand.ToString());
	        //int index = UnityEngine.Random.Range(0, enemyList.Count - 1);
	        //   target = enemyList[index];
	    }
        if(target!=null)
        {
            agent.Blackboard.SetItem("target", target);
            return BehaviourNodeStatus.Success;
        }else
        {
            agent.Blackboard.SetItem("target", null);
            return BehaviourNodeStatus.Failure;
        }
	}

}