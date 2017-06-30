using System;
using System.Collections.Generic;
using System.Text;
using Logic.Skill;
using UnityEngine;
using Brainiac;

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
        public AIAgent aiAgent;

        internal override void OnInit()
        {
            skillManager = new SkillManager(this);
            stateMachine = new StateMachine(this);
            BTAsset btAsset = Resources.Load("Test") as BTAsset;
            aiAgent = new AIAgent(this, btAsset);
            aiAgent.Start();
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
        public long GetAttributeValue(AttributeType at)
        {
            return attributeManager[at];
        }
        public long HP {
            get {
               return attributeManager[AttributeType.HP]; 
            }
            set
            {
                if(value <= 0)
                {
                    Dead();
                    attributeManager.SetBase(AttributeType.HP, 0);
                }
                else
                    attributeManager.SetBase(AttributeType.HP, value);
            }
        }

        internal override void ListenEvents()
        {
            base.ListenEvents();
        }
        public bool ReleaseSkill(string path)
        {
            if (!skillManager.IsRunningSkill)
            {
                skillManager.ReleaseSkill(path);
                return true;
            }
            else
                return false;
      //      stateMachine.Start(new MoveState() {dir = new Lockstep.Vector3d(Vector3.forward), speed = Lockstep.FixedMath.One * 2 });
        }
        public bool ReleaseSkill(string path, SceneObject target)
        {
            if (!skillManager.IsRunningSkill)
            {
                skillManager.ReleaseSkill(path, target);
                return true;
            }
            else
                return false;
            //      stateMachine.Start(new MoveState() {dir = new Lockstep.Vector3d(Vector3.forward), speed = Lockstep.FixedMath.One * 2 });
        }
        public bool IsDeath()
        {
            return HP == 0;
        }
        private void Dead()
        {
            OnDeath();
        }
        public virtual void OnDeath()
        {
            EventManager.AddEvent("1.event", new SkillRunningData() { sender = this });
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
            aiAgent.Tick();
            skillManager.Update(deltaTime);
            stateMachine.Update();
            base.OnUpdate(deltaTime);
        }

    }
}
