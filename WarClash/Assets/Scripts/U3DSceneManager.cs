using System;
using System.Collections;
using System.Collections.Generic;
using Logic;
using UnityEngine;
using Logic.LogicObject;

public class U3DSceneManager {

    U3DScene u3dScene;
    public void Init()
    {
        Logic.LogicCore.SP.eventGroup.AddEvent((int)Logic.SceneManager.SceneManagerEvent.ONSWITCHSCENE, OnSwitchScene);
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
