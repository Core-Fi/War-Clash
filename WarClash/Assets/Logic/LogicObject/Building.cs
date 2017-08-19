using Logic;
using Logic.LogicObject;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : SceneObject
{
    public enum BuildingEvent
    {
        OnCreate = 1000
    }
    
    internal override void OnInit()
    {
        base.OnInit();

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
