using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logic;
using Logic.LogicObject;

public class BattleUI : MonoBehaviour {

    public UnityEngine.UI.Button btn;
	// Use this for initialization
	void Start () {
		
        btn.onClick.AddListener(OnButtonClick);


	}
    private void OnButtonClick()
    {
        var npc = LogicCore.SP.SceneManager.currentScene.CreateSceneObject<Npc>();
        npc.Position = new Lockstep.Vector3d(new Vector3(Random.Range(-10,10),0, 0));
    }
	// Update is called once per frame
	void Update () {
		
	}
}
