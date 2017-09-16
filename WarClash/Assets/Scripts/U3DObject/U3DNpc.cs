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
        Resource.LoadAsset(Npc.Conf.ResPath, OnLoadedRes);
    }

    public override void OnLoadedRes(string name, Object obj)
    {
        base.OnLoadedRes(name, obj);
        _navMeshAgent = Go.GetComponent<NavMeshAgent>();
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
            var logicPosi = Character.Position.ToVector3();
            var logicForward = Character.Forward.ToVector3();
            Transform.position = Vector3.Lerp(Transform.position, logicPosi, Time.deltaTime * 6);
            Transform.forward = Vector3.Lerp(Transform.forward, logicForward, Time.deltaTime * 6);
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
    }
}
