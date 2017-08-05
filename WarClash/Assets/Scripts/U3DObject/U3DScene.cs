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
        UnityEngine.SceneManagement.SceneManager.LoadScene("Scene01");
        Main.inst.StartCoroutine(LoadScene());
    }
    IEnumerator LoadScene()
    {
        AsyncOperation asyn = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Scene01");
        yield return asyn;
        OnInit();
        ListenEvents();
    }
    protected virtual void OnInit()
    {
        

    }
    protected virtual void ListenEvents()
    {
        scene.EventGroup.ListenEvent((int)Scene.SceneEvent.ADDSCENEOBJECT,OnAddSceneObject);
        scene.EventGroup.ListenEvent((int)Scene.SceneEvent.REMOVESCENEOBJECT, OnRemoveSceneObject);
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
        }else if(msg.value is Npc)
        {
            uso = new U3DNpc();
        }
        AddObject(msg.value.ID, uso);
        uso.Init(msg.value);
        uso.ListenEvents();

    }

    public override void OnUpdate()
    {
        base.OnUpdate();
    }
}
