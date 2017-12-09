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
        Character.EventGroup.ListenEvent(Character.CharacterEvent.Executedisplayaction.ToInt(), ExecuteDisplayAction);
        Character.EventGroup.ListenEvent(Character.CharacterEvent.Stopdisplayaction.ToInt(), StopDisplayAction);
        Character.EventGroup.ListenEvent(Character.CharacterEvent.Startskill.ToInt(), OnSkillStart);
        Character.EventGroup.ListenEvent(Character.CharacterEvent.Cancelskill.ToInt(), OnSkillCancel);
        Character.EventGroup.ListenEvent(Character.CharacterEvent.Endskill.ToInt(), OnSkillEnd);
        Character.EventGroup.ListenEvent(SceneObject.SceneObjectEvent.OnAttributechange.ToInt(), OnAttributeChange);
    }

    protected virtual void OnAttributeChange(object sender, EventMsg e)
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
        animator = Go.GetComponent<Animator>();
        SetSpeed();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
    }

    public void SetSpeed()
    {
        long speed = Character.AttributeManager[AttributeType.Speed];
        if (animator != null && Go.activeSelf)
        {
            animator.SetFloat("Speed", speed.ToFloat());
        }
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
