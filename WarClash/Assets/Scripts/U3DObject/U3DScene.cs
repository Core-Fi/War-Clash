using Logic.LogicObject;
using Logic.Objects;
using System.Collections;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using Lockstep;
using Pathfinding;
using UnityEngine.AI;


public class U3DScene : ObjectCollection<int, U3DSceneObject>
{
    private Scene Scene;
    public U3DScene()
    {
    }
    public void Init(Scene scene)
    {
        this.Scene = scene;
        ListenEvents();
        UnityEngine.SceneManagement.SceneManager.LoadScene(Scene.Name);
        Main.SP.StartCoroutine(LoadScene());
    }

    public static bool IsFixed;
    IEnumerator LoadScene()
    {
        AsyncOperation asyn = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("scene01");
        yield return asyn;
        OnInit();
        
    }
    protected virtual void OnInit()
    {
        

    }
    protected virtual void ListenEvents()
    {
        Scene.EventGroup.ListenEvent((int)Scene.SceneEvent.Addsceneobject,OnAddSceneObject);
        Scene.EventGroup.ListenEvent((int)Scene.SceneEvent.Removesceneobject, OnRemoveSceneObject);
    }

    private void OnRemoveSceneObject(object sender, EventMsg e)
    {
        EventSingleArgs<int> msg = e as EventSingleArgs<int>;
        U3DSceneObject uso = GetObject(msg.value);
        uso.Destroy();
        RemoveObject(msg.value);
    }

    private void OnAddSceneObject(object sender, EventMsg e)
    {
        EventSingleArgs<SceneObject> msg = e as EventSingleArgs<SceneObject>;
        Type t = LogicObjectCorresponding.Corresponding[msg.value.GetType()];
        U3DSceneObject uso = Activator.CreateInstance(t) as U3DSceneObject;
        AddObject(msg.value.Id, uso);
        uso.Init(msg.value);
        uso.ListenEvents();
    }

    public override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);
    }
}
