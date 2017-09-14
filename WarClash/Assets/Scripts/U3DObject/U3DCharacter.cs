using Logic.LogicObject;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logic;
using System;
using Logic.Skill.Actions;
using Lockstep;
using Object = UnityEngine.Object;

public class U3DCharacter : U3DSceneObject{

    public Character Character;
    private U3DDisplayActionManager _u3DDisplayManager;
    public Animator animator;
    public override void OnInit()
    {
        base.OnInit();
        Character = So as Character;
        _u3DDisplayManager = new U3DDisplayActionManager(this);
    }
    public override void ListenEvents()
    {
        base.ListenEvents();
        Character.EventGroup.ListenEvent((int)Character.CharacterEvent.Executedisplayaction, ExecuteDisplayAction);
        Character.EventGroup.ListenEvent((int)Character.CharacterEvent.Stopdisplayaction, StopDisplayAction);
        Character.EventGroup.ListenEvent((int)Character.CharacterEvent.Startskill, OnSkillStart);
        Character.EventGroup.ListenEvent((int)Character.CharacterEvent.Cancelskill, OnSkillCancel);
        Character.EventGroup.ListenEvent((int)Character.CharacterEvent.Endskill, OnSkillEnd);
        Character.EventGroup.ListenEvent((int)SceneObject.SceneObjectEvent.Onattributechange, OnAttributeChange);
    }

    private void OnAttributeChange(object sender, EventMsg e)
    {
        EventSingleArgs<AttributeMsg> msg = e as EventSingleArgs<AttributeMsg>;
        if(msg == null) return;
        if(msg.value.At == AttributeType.Speed)
        {
            SetSpeed();
        }
    }

    public override void OnLoadedRes(string name, Object obj)
    {
        base.OnLoadedRes(name, obj);
        SetSpeed();
        animator = Go.GetComponent<Animator>();
        
    }

    public void SetSpeed()
    {
        long speed = Character.AttributeManager[AttributeType.Speed];
        if(animator!=null)
            animator.SetFloat("Speed", speed.ToFloat());
    }
    private void OnSkillEnd(object sender, EventMsg e)
    {
    }

    private void OnSkillCancel(object sender, EventMsg e)
    {
    }

    private void OnSkillStart(object sender, EventMsg e)
    {
    }

    private void StopDisplayAction(object sender, EventMsg e)
    {
        EventSingleArgs<DisplayAction> msg = e as EventSingleArgs<DisplayAction>;
        _u3DDisplayManager.Stop(msg.value);
    }
    private void ExecuteDisplayAction(object sender, EventMsg e)
    {
        EventSingleArgs<DisplayAction> msg = e as EventSingleArgs<DisplayAction>;
        _u3DDisplayManager.Play(msg.value);
    }
    public override void OnDestroy()
    {
        base.OnDestroy();
    }
}
