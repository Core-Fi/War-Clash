using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class HudView:View
{
    public GameObject PlayerHudGo;
    private HudP _hudP;
    private List< PlayerHud> _playerHuds = new List<PlayerHud>();
    public override void OnInit(GameObject go)
    {
        base.OnInit(go);
        _hudP = new HudP(go);
        _hudP.Init();
        _hudP.m_template_player_go.SetActive(false);
        ListenEvent(UIEventList.DrawPlayerHud.ToInt(), DrawPlayerHud);
    }

    void DrawPlayerHud(object sender, EventMsg msg)
    {
        var u3DPlayer = sender as U3DPlayer;
        bool exist = false;
        for (int i = 0; i < _playerHuds.Count; i++)
        {
            if (_playerHuds[i].U3DPlayer.Equals(u3DPlayer))
            {
                exist = true;
            }
        }
        if (!exist)
        {
            var newGo = CreateNew(_hudP.m_template_player_go);
            newGo.SetActive(true);
            var playerHud = new PlayerHud(newGo, u3DPlayer);
            playerHud.Init();
            _playerHuds.Add(playerHud);
        }
        
    }

    private GameObject CreateNew(GameObject go)
    {
        var newgo = GameObject.Instantiate(go);
        newgo.transform.SetParent(go.transform.parent);
        newgo.SetActive(true);
        newgo.transform.position = go.transform.position;
        newgo.transform.rotation = go.transform.rotation;
        newgo.transform.localScale = go.transform.localScale;
        return newgo;
    }
    public override void OnDispose()
    {
        base.OnDispose();
        DelEvent(UIEventList.DrawPlayerHud.ToInt(), DrawPlayerHud);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        for (int i = 0; i < _playerHuds.Count; i++)
        {
            _playerHuds[i].Update();
        }

    }
}

