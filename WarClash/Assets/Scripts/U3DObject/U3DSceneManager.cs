using System;
using System.Collections;
using System.Collections.Generic;
using Logic;
using UnityEngine;
using Logic.LogicObject;

public class U3DSceneManager {

    public U3DScene u3dScene { get; private set; }
    public U3DSceneManager()
    {
        Logic.LogicCore.SP.sceneManager.eventGroup.ListenEvent((int)Logic.SceneManager.SceneManagerEvent.ONSWITCHSCENE, OnSwitchScene);
    }

    private void OnSwitchScene(object sender, EventMsg e)
    {
        EventSingleArgs<Scene> msg = e as EventSingleArgs<Scene>;
        Scene scene = msg.value;
        u3dScene = new U3DScene();
        u3dScene.Init(scene);
    }
    public void Update()
    {
        u3dScene.Update();
    }
}
