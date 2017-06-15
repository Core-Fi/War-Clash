using Logic.LogicObject;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logic;
using UnityEngine.AI;

public class U3DNpc : U3DCharacter{

    public Npc npc { get; private set; }
    private GameObject go;
    private NavMeshAgent navMeshAgent;
    public override void OnInit()
    {
        base.OnInit();
        npc = so as Npc;
        GameObject g = Resources.Load("Prefabs/footman_prefab") as GameObject;
        go = GameObject.Instantiate(g);
        go.transform.position = npc.Position.ToVector3();
        animator = go.GetComponent<Animator>();
        navMeshAgent = go.GetComponent<NavMeshAgent>();
        SetSpeed();
    }
    public override void ListenEvents()
    {
        base.ListenEvents();
    }
    public override void OnUpdate()
    {
        base.OnUpdate();
        go.transform.position = character.Position.ToVector3();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
    }
}
