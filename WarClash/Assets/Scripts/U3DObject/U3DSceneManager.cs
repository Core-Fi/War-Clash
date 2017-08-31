using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logic.LogicObject;

public class U3DSceneManager
{
    public U3DScene U3DScene { get; private set; }
    public U3DSceneManager()
    {
        Logic.LogicCore.SP.SceneManager.EventGroup.ListenEvent((int)Logic.SceneManager.SceneManagerEvent.OnSwitchScene, OnSwitchScene);
    }
    private void OnSwitchScene(object sender, EventMsg e)
    {
        EventSingleArgs<Scene> msg = e as EventSingleArgs<Scene>;
        Scene scene = msg.value;
        U3DScene = new U3DScene();
        U3DScene.Init(scene);
    }
    public void Update()
    {
        U3DScene.Update(Time.deltaTime);
    }
}
