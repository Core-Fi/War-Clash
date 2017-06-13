using Logic.LogicObject;
using Logic.Objects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logic;
using System;

public class U3DScene : ObjectCollection<int, U3DSceneObject>
{

    public Scene scene;
    public U3DScene()
    {
    }
    public void Init(Scene scene)
    {
        this.scene = scene;
        OnInit();
    }
    public void ListenEvents()
    {
        scene.EventGroup.AddEvent((int)Scene.SceneEvent.ADDSCENEOBJECT,OnAddSceneObject);
        scene.EventGroup.AddEvent((int)Scene.SceneEvent.REMOVESCENEOBJECT, OnRemoveSceneObject);
    }

    private void OnRemoveSceneObject(object sender, EventMsg e)
    {
        EventSingleArgs<SceneObject> msg = e as EventSingleArgs<SceneObject>;
        U3DSceneObject uso = GetObject(msg.value.ID);
        uso.Destroy();
        RemoveObject(msg.value.ID);
    }

    private void OnAddSceneObject(object sender, EventMsg e)
    {
        EventSingleArgs<SceneObject> msg = e as EventSingleArgs<SceneObject>;
        U3DSceneObject uso = null;
        if(msg.value is Projectile)
        {
            uso = new U3DProjectile();
        }
        AddObject(msg.value.ID, uso);
        uso.Init(msg.value);

    }

    public virtual void OnInit()
    {
        ListenEvents();
    }
    public void Update()
    {
        OnUpdate();
    }
    public virtual void OnUpdate()
    {

    }
}
