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
            Startskill = 100,
            Cancelskill,
            Endskill,
            Executedisplayaction,
            Stopdisplayaction,
        }
        public StateMachine StateMachine { get; private set; }
        public SkillManager SkillManager { get; private set; }
        public AIAgent AiAgent;

        internal override void OnInit()
        {
            SkillManager = new SkillManager(this);
            StateMachine = new StateMachine(this);
            BTAsset btAsset = Resources.Load("Test") as BTAsset;
            AiAgent = new AIAgent(this, btAsset);
            AiAgent.Start();
            AttributeManager.New(AttributeType.SPEED, 0);
            AttributeManager.New(AttributeType.MAXSPEED, Lockstep.FixedMath.One * 2);
            AttributeManager.New(AttributeType.MAXHP, Lockstep.FixedMath.One * 100);
            AttributeManager.New(AttributeType.HP, Lockstep.FixedMath.One * 100);
        }
        internal override void ListenEvents()
        {
            base.ListenEvents();
        }
        public override void OnAttributeChange(AttributeType at, long old, long newValue)
        {
            base.OnAttributeChange(at, old, newValue);
            if (at == AttributeType.HP)
            {
                if(newValue == 0)
                    Dead();
            }
        }
        public bool ReleaseSkill(string path)
        {
            if (!SkillManager.IsRunningSkill)
            {
                SkillManager.ReleaseSkill(path);
                return true;
            }
            else
                return false;
      //      StateMachine.Start(new MoveState() {dir = new Lockstep.Vector3d(Vector3.forward), speed = Lockstep.FixedMath.One * 2 });
        }
        public bool ReleaseSkill(string path, SceneObject target)
        {
            if (!SkillManager.IsRunningSkill && !IsDeath())
            {
                SkillManager.ReleaseSkill(path, target);
                return true;
            }
            else
                return false;
            //      StateMachine.Start(new MoveState() {dir = new Lockstep.Vector3d(Vector3.forward), speed = Lockstep.FixedMath.One * 2 });
        }
        public bool IsDeath()
        {
            return Hp == 0;
        }
        private void Dead()
        {
            OnDeath();
        }
        public virtual void OnDeath()
        {
            EventManager.AddEvent("1.event", new RuntimeData() { sender = this });
        }
        public bool IsRunningSkill
        {
            get { return SkillManager.IsRunningSkill; }
        }
        public void CancelSkill()
        {
            if (IsRunningSkill)
            {
                this.SkillManager.CancelSkill();
            }
        }

        internal override void OnFixedUpdate(long deltaTime)
        {
            base.OnFixedUpdate(deltaTime);
        }

        internal override void OnUpdate(float deltaTime)
        {
            AiAgent.Tick();
            SkillManager.Update(deltaTime);
            StateMachine.Update();
            base.OnUpdate(deltaTime);
        }

    }
}
