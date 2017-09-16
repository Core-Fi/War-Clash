using System;
using Logic;
using Logic.LogicObject;
using System.Collections;
using System.Collections.Generic;
using Config;
using Logic.Config;
using UnityEngine;

public class Building : SceneObject
{
    public static readonly Dictionary<int, Type> BuildingId_Type = new Dictionary<int, Type>
    {
        {1001, typeof(BarackBuilding)}
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
