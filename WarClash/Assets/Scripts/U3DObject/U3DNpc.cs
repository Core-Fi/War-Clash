using Logic.LogicObject;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logic;
using UnityEngine.AI;

public class U3DNpc : U3DCharacter{

    public Npc Npc { get; private set; }
    private NavMeshAgent _navMeshAgent;
    public override void OnInit()
    {
        base.OnInit();
        Npc = So as Npc;
        Resource.LoadAsset("Footman_prefab.prefab", OnLoadedRes);
    }

    public override void OnLoadedRes(string name, Object obj)
    {
        Go = Object.Instantiate(obj) as GameObject;
        Go.name = this.Npc.ToString();
        var lo = Go.AddComponent<LogicObject>();
        lo.ID = Npc.Id;
        Go.transform.position = Npc.Position.ToVector3();
        animator = Go.GetComponent<Animator>();
        _navMeshAgent = Go.GetComponent<NavMeshAgent>();
        SetSpeed();
        base.OnLoadedRes(name, obj);
    }

    public override void ListenEvents()
    {
        base.ListenEvents();
    }
    public override void OnUpdate()
    {
        base.OnUpdate();
        if (Go != null)
        {
            Go.transform.position = Character.Position.ToVector3();
            Go.transform.forward = Character.Forward.ToVector3();
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
    }
}
