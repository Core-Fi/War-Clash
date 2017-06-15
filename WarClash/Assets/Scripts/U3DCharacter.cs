using Logic.LogicObject;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logic;
using System;
using Logic.Skill.Actions;
using Lockstep;

public class U3DCharacter : U3DSceneObject{

    public Character character;
    private U3DDisplayActionManager u3dDisplayManager;
    public Animator animator;
    public override void OnInit()
    {
        base.OnInit();
        character = so as Character;
        u3dDisplayManager = new U3DDisplayActionManager(this);
    }
    public override void ListenEvents()
    {
        base.ListenEvents();
        character.EventGroup.AddEvent((int)Character.CharacterEvent.EXECUTEDISPLAYACTION, ExecuteDisplayAction);
        character.EventGroup.AddEvent((int)Character.CharacterEvent.STOPDISPLAYACTION, StopDisplayAction);
        character.EventGroup.AddEvent((int)Character.CharacterEvent.STARTSKILL, OnSkillStart);
        character.EventGroup.AddEvent((int)Character.CharacterEvent.CANCELSKILL, OnSkillCancel);
        character.EventGroup.AddEvent((int)Character.CharacterEvent.ENDSKILL, OnSKillEnd);
        character.EventGroup.AddEvent((int)Character.CharacterEvent.ONATTRIBUTECHANGE, OnAttributeChange);
    }

    private void OnAttributeChange(object sender, EventMsg e)
    {
        EventSingleArgs<AttributeMsg> msg = e as EventSingleArgs<AttributeMsg>;
        if(msg.value.at == AttributeType.SPEED)
        {
            SetSpeed();
        }
    }

    public void SetSpeed()
    {
        long speed = character.attributeManager[AttributeType.SPEED];
        animator.SetInteger("Speed", FixedMath.ToInt(speed));
    }
    private void OnSKillEnd(object sender, EventMsg e)
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
        u3dDisplayManager.Stop(msg.value);
    }
    private void ExecuteDisplayAction(object sender, EventMsg e)
    {
        EventSingleArgs<DisplayAction> msg = e as EventSingleArgs<DisplayAction>;
        u3dDisplayManager.Play(msg.value);
    }
    public override void OnDestroy()
    {
        base.OnDestroy();
    }
}
