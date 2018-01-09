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
    private long _range;
    private long _rangeSqr;
    [BTProperty("Building As Target")]
    public MemoryVar Building;
    [BTProperty("Enemy As Target")]
    public MemoryVar Enemy;
    public override void OnStart(AIAgent agent)
    {
        base.OnStart(agent);
        _self = agent.SceneObject;
        _searchEnemy = Search;
        if (_self is Npc)
        {
            var npc = _self as Npc;
            _range = FixedMath.Create(npc.Conf.WaringRange) / 100;
        }
        else if (_self is Tower)
        {
            var tower = _self as Tower;
            _range = FixedMath.Create(tower.Conf.Param1)/100;
        }
        _rangeSqr = _range.Mul(_range);
    }
    private Logic.Objects.ObjectCollection<int, SceneObject>.VoidAction<SceneObject> _searchEnemy;
    private void Search(SceneObject c)
    {
        if (c != _self && c.Hp > 0 && c.Team != _self.Team &&
            Vector3d.SqrDistance(c.Position, _self.Position)< _rangeSqr)
        {
            if (Building.AsBool.Value && c is Building)
            {
                enemyList.Add(c);
            }
            if(Enemy.AsBool.Value && c is Character)
            {
                enemyList.Add(c);
            }
        }
    }

    protected override BehaviourNodeStatus OnExecute(AIAgent agent)
	{
        enemyList.Clear();
	    var bs = LogicCore.SP.SceneManager.CurrentScene as BattleScene;
        bs.ForEachDo<SceneObject>(_searchEnemy);
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
	        int rand = LogicCore.SP.LockFrameMgr.RandomRange(0, enemyList.Count - 1);
            target = enemyList[rand];
            LogicCore.SP.Write(rand.ToString());
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