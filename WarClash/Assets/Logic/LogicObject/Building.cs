using System;
using Logic;
using Logic.LogicObject;
using System.Collections;
using System.Collections.Generic;
using Config;
using Lockstep;
using Logic.Config;
using UnityEngine;

public class Building : SceneObject, IFixedAgent
{
    #region IFixedAgent Impl
    public IList<IFixedAgent> AgentNeighbors { get; set; }
    public IList<long> AgentNeighborSqrDists { get; set; }
    public IFixedAgent Next { get; set; }
    public long Radius { get; set; }
    public long InsertAgentNeighbour(IFixedAgent fixedAgent, long rangeSq)
    {
        if (this == fixedAgent) return rangeSq;
        var dist = (fixedAgent.Position.x - Position.x).Mul(fixedAgent.Position.x - Position.x)
                   + (fixedAgent.Position.z - Position.z).Mul(fixedAgent.Position.z - Position.z);
        if (dist < rangeSq)
        {
            if (AgentNeighbors.Count < 10)
            {
                AgentNeighbors.Add(fixedAgent);
                AgentNeighborSqrDists.Add(dist);
            }
            var i = AgentNeighbors.Count - 1;
            if (dist < AgentNeighborSqrDists[i])
            {
                while (i != 0 && dist < AgentNeighborSqrDists[i - 1])
                {
                    AgentNeighbors[i] = AgentNeighbors[i - 1];
                    AgentNeighborSqrDists[i] = AgentNeighborSqrDists[i - 1];
                    i--;
                }
                AgentNeighbors[i] = fixedAgent;
                AgentNeighborSqrDists[i] = dist;
            }

            if (AgentNeighbors.Count == 10)
                rangeSq = AgentNeighborSqrDists[AgentNeighbors.Count - 1];
        }
        return rangeSq;
    }
    #endregion
    

    public static readonly Dictionary<int, Type> BuildingIdType = new Dictionary<int, Type>
    {
        {1001, typeof(BarackBuilding)},
        {1002, typeof(Tower) }
    };
    public BuildingConf Conf;
    public enum BuildingEvent
    {
        OnCreate = 1000
    }
    
    internal override void OnInit(CreateInfo createInfo)
    {
        base.OnInit(createInfo);
        var buildingCreateInfo = (createInfo as BuildingCreateInfo);
        Conf = ConfigMap<BuildingConf>.Get(buildingCreateInfo.BuildingId);
        base.EventGroup.ListenEvent(SceneObjectEvent.Positionchange.ToInt(), OnPositionChange);
    }
    private void OnPositionChange(object sender, EventMsg e)
    {
        LogicCore.SP.SceneManager.CurrentScene.FixedQuadTreeForBuilding.Relocate(this);
    }
    internal override void ListenEvents()
    {
        base.ListenEvents();
    }

    internal override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);
    }

    private void Dead()
    {
        
    }
}
