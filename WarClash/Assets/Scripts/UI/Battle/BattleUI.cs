using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logic;

public class BattleUI : MonoBehaviour {

    public UnityEngine.UI.Button btn;
	// Use this for initialization
	void Start () {
		
        btn.onClick.AddListener(OnButtonClick);


	}
    private void OnButtonClick()
    {
    //    LogicCore.SP.sceneManager.currentScene.

    }
	// Update is called once per frame
	void Update () {
		
	}
}
