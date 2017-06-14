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
        Npc npc = new Npc();
        Logic.LogicCore.SP.sceneManager.currentScene.CreateNpc(npc);
        npc.ReleaseSkill(Application.streamingAssetsPath+"/skills/1.skill");
    }
	
	// Update is called once per frame
	void Update () {
        Logic.LogicCore.SP.Update(Time.deltaTime);
	}
    void FixedUpdate()
    {
        Logic.LogicCore.SP.FixedUpdate();
    }
}
