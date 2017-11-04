using System;
using System.Collections.Generic;
using System.Text;
using Apex.PathFinding;
using Logic.Skill;
using UnityEngine;
using Brainiac;
using Lockstep;

namespace Logic.LogicObject
{
    public class Character : SceneObject, ISteering
    {
        #region IFixedAgent Impl
        public IFixedAgent Next { get; set; }
        #endregion
        #region ISteering Impl
        public Vector3d Velocity { get; set; }
        public long Speed { get { return AttributeManager[AttributeType.Speed]; } }
        public long Radius { get; set; }
        public Vector3d Acceleration { get; set; }
        public long MaxDeceleration { get; set; }
        public long MaxAcceleration { get; set; }
        #endregion
        public enum CharacterEvent 
        {
            Startskill = 100,
            Cancelskill,
            Endskill,
            Executedisplayaction,
            Stopdisplayaction,
        }
      
        public SkillManager SkillManager { get; private set; }
        public AIAgent AiAgent { get; protected set; }
        public SteeringManager SteeringManager { get; protected set; }

        internal override void OnInit(CreateInfo createInfo)
        {
            base.OnInit(createInfo);
            SkillManager = new SkillManager(this);
            SteeringManager = new SteeringManager(this);
            AttributeManager.New(AttributeType.Speed, 0);
            AttributeManager.New(AttributeType.MaxSpeed, Lockstep.FixedMath.One * 2);
            AttributeManager.New(AttributeType.Maxhp, Lockstep.FixedMath.One * 100);
            AttributeManager.New(AttributeType.Hp, Lockstep.FixedMath.One * 300);
            base.EventGroup.ListenEvent((int)SceneObjectEvent.Positionchange, OnPositionChange);
            MaxAcceleration = FixedMath.Create(20);
            MaxDeceleration = FixedMath.Create(30);
        }
        internal override void ListenEvents()
        {
            base.ListenEvents();
        }
        public override void OnAttributeChange(AttributeType at, long old, long newValue)
        {
            base.OnAttributeChange(at, old, newValue);
            if (at == AttributeType.Hp)
            {
                if(newValue == 0)
                    Dead();
            }
        }

        private void OnPositionChange(object sender, EventMsg e)
        {
            LogicCore.SP.SceneManager.CurrentScene.FixedQuadTree.Relocate(this);
        }
        public bool ReleaseSkill(int id)
        {
            if (!SkillManager.IsRunningSkill)
            {
                SkillManager.ReleaseSkill(id);
                return true;
            }
            else
                return false;
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
        public bool ReleaseSkill(int id, SceneObject target)
        {
            if (!SkillManager.IsRunningSkill && !IsDeath())
            {
                SkillManager.ReleaseSkill(id, target);
                return true;
            }
            else
                return false;
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
            EventManager.AddEvent(2, new RuntimeData() { sender = this });
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
            SkillManager.FixedUpdate();
            Vector3d acc;
            var hasResult = SteeringManager.GetDesiredAcceleration(out acc);
            if (!hasResult)
            {
                acc = Vector3d.ClampMagnitude(-Velocity / LockFrameMgr.FixedFrameTime, MaxDeceleration);
            }
            Velocity += acc.Mul(LockFrameMgr.FixedFrameTime);
            if (Velocity.sqrMagnitude > 100)
            {
                Velocity = Velocity.Normalize() * Speed;
            }
            //Velocity = Vector3d.ClampMagnitude(Velocity, Speed);

            //if (Velocity.sqrMagnitude > 100)
            //{
            //    var agent = new FixedAgent(this);
            //    var caculated = agent.UpdateAgent();
            //    Position += Vector3d.ClampMagnitude(caculated.ToVector3d(), Speed).Mul(LockFrameMgr.FixedFrameTime);
            //}
            
            Position += Velocity.Mul(LockFrameMgr.FixedFrameTime);
        }
       
        internal override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);
        }

    }
}
