using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logic.LogicObject;

public class U3DSceneManager
{
    public IU3DScene U3dScene { get; private set; }
    public U3DSceneManager()
    {
        Logic.LogicCore.SP.SceneManager.EventGroup.ListenEvent((int)Logic.SceneManager.SceneManagerEvent.OnSwitchScene, OnSwitchScene);
    }
    private void OnSwitchScene(object sender, EventMsg e)
    {
        EventSingleArgs<IScene> msg = e as EventSingleArgs<IScene>;
        var battleScene = msg.value;
        if (battleScene is BattleScene)
        {
            U3dScene = new U3DBattleScene();
        }
        else if (battleScene is HotFixScene)
        {
            U3dScene = new U3DHotFixScene();
        }
        U3dScene.Init(battleScene);
    }
    public void Update()
    {
        U3dScene.Update(Time.deltaTime);
    }
}
