using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logic;
using Logic.LogicObject;
using UnityEngine.UI;
using System;

public class BattleUI : MonoBehaviour {

    public UnityEngine.UI.Button atk_btn;

    public Button followMe_strategy_btn;
	// Use this for initialization
	void Start () {

        atk_btn.onClick.AddListener(OnButtonClick);
	    followMe_strategy_btn.onClick.AddListener(OnFollowMeClick);
    }

    private void OnFollowMeClick()
    {
        var cmd = new ChangeStrategyCommand{Strategy = (byte)LockFrameMgr.Strategy.FollowPlayer, Sender = MainPlayer.SP.Id};
        LogicCore.SP.LockFrameMgr.SendCommand(cmd);
    }

    private void OnButtonClick()
    {
        var cmd = Pool.SP.Get<ReleaseSkillCommand>();
        cmd.Id = 1;
        cmd.Sender = LogicCore.SP.SceneManager.CurrentScene.GetObject<MainPlayer>().Id;
        LogicCore.SP.LockFrameMgr.SendCommand(cmd);
    }
	// Update is called once per frame
	void Update () {
		
	}
}
