using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logic;
using Logic.LogicObject;

public class BattleUI : MonoBehaviour {

    public UnityEngine.UI.Button atk_btn;
	// Use this for initialization
	void Start () {

        atk_btn.onClick.AddListener(OnButtonClick);


	}
    private void OnButtonClick()
    {
        var cmd = Pool.SP.Get<ReleaseSkillCommand>();
        cmd.Id = 1;
        cmd.Sender = LogicCore.SP.SceneManager.currentScene.GetObject<MainPlayer>().Id;
        LogicCore.SP.LockFrameMgr.SendCommand(cmd);
    }
	// Update is called once per frame
	void Update () {
		
	}
}
