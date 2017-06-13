using Logic.LogicObject;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logic;

public class U3DNpc : U3DCharacter{

    public Npc npc;
    private GameObject go;
    public override void OnInit()
    {
        base.OnInit();
        npc = so as Npc;
        GameObject g = Resources.Load("Prefabs/footman_prefab") as GameObject;
        go = GameObject.Instantiate(g);
        go.transform.position = npc.Position;
    }
    public override void ListenEvents()
    {
        base.ListenEvents();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
    }
}
