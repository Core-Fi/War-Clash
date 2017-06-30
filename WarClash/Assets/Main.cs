using Logic;
using Logic.LogicObject;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour {

    U3DSceneManager u3dSceneManager;
	// Use this for initialization
	void Start ()
    {
        Logic.LogicCore.SP.Init();
        u3dSceneManager = new U3DSceneManager();
        Logic.LogicCore.SP.sceneManager.SwitchScene(new Scene());
        var npc1 = LogicCore.SP.sceneManager.currentScene.CreateSceneObject<Npc>();
        npc1.Position = new Lockstep.Vector3d(new Vector3(-10, 0, 0));
        var npc2 = LogicCore.SP.sceneManager.currentScene.CreateSceneObject<Npc>();
        npc2.Position = new Lockstep.Vector3d(new Vector3(10, 0, 0));
        //npc.ReleaseSkill(Application.streamingAssetsPath+"/skills/1.skill");
    }
	
	// Update is called once per frame
	void Update () {
        u3dSceneManager.Update();
        Logic.LogicCore.SP.Update(Time.deltaTime);
	}
    void FixedUpdate()
    {
        for (int i = 0; i < 100; i++)
        {
            Logic.LogicCore.SP.FixedUpdate();
        }
    }
}
