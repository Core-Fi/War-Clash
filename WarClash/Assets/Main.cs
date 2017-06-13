using Logic.LogicObject;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour {

    U3DSceneManager u3dSceneManager;
	// Use this for initialization
	void Start () {
        Logic.LogicCore.SP.Init();
        u3dSceneManager = new U3DSceneManager();
        Logic.LogicCore.SP.sceneManager.SwitchScene(new Scene());
        Logic.LogicCore.SP.sceneManager.currentScene.CreateNpc(new Npc());
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
