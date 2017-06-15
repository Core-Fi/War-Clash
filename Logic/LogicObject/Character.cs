using System;
using System.Collections.Generic;
using System.Text;
using Logic.Skill;
using UnityEngine;

namespace Logic.LogicObject
{
    public class Character : SceneObject
    {
        public enum CharacterEvent 
        {
            STARTSKILL = 100,
            CANCELSKILL,
            ENDSKILL,
            EXECUTEDISPLAYACTION,
            STOPDISPLAYACTION,
            ONATTRIBUTECHANGE,
        }
        public StateMachine stateMachine { get; private set; }
        public SkillManager skillManager { get; private set; }
        public AttributeManager attributeManager { get; private set; }
        internal override void OnInit()
        {
            skillManager = new SkillManager(this);
            stateMachine = new StateMachine(this);
            attributeManager = new AttributeManager();

            attributeManager.OnAttributeChange += OnAttributeChange;
            attributeManager.New(AttributeType.SPEED, Lockstep.FixedMath.One * 2);
            attributeManager.New(AttributeType.MAXHP, Lockstep.FixedMath.One * 100);
            attributeManager.New(AttributeType.HP, Lockstep.FixedMath.One * 100);
        }
        public virtual void OnAttributeChange(AttributeType at, long old, long newValue)
        {
            EventGroup.FireEvent((int)CharacterEvent.ONATTRIBUTECHANGE, this,  EventGroup.NewArg<EventSingleArgs<AttributeMsg>, AttributeMsg>(new AttributeMsg() {
                at = at, newValue = newValue, oldValue = old
            }));
        } 
        internal override void ListenEvents()
        {
            base.ListenEvents();
        }
        public void ReleaseSkill(string path)
        {
            if (!skillManager.IsRunningSkill)
            {
                skillManager.ReleaseSkill(path);
            }
            stateMachine.Start(new MoveState() {dir = new Lockstep.Vector3d(Vector3.forward), speed = Lockstep.FixedMath.One * 2 });
        }
        
        public bool IsRunningSkill
        {
            get { return skillManager.IsRunningSkill; }
        }
        public void CancelSkill()
        {
            if (IsRunningSkill)
            {
                this.skillManager.CancelSkill();
            }
        }

        internal override void OnUpdate(float deltaTime)
        {
            skillManager.Update(deltaTime);
            stateMachine.Update();
            base.OnUpdate(deltaTime);
        }

    }
}
