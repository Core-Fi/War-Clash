using System;
using System.Collections.Generic;
using System.Text;
using Logic.Skill;
using UnityEngine;
using Brainiac;
using Lockstep;

namespace Logic.LogicObject
{
    public class Character : SceneObject, ISteering
    {
        #region IFixedAgent Impl
        public IList<IFixedAgent> AgentNeighbors { get; set; }
        public IList<long> AgentNeighborSqrDists { get; set; }
        public IFixedAgent Next { get; set; }
        public long InsertAgentNeighbour(IFixedAgent fixedAgent, long rangeSq)
        {
            if (this == fixedAgent) return rangeSq;
            var dist = (fixedAgent.Position.x - Position.x).Mul(fixedAgent.Position.x - Position.x)
                       + (fixedAgent.Position.z - Position.z).Mul(fixedAgent.Position.z - Position.z);
            if (dist < rangeSq)
            {
                if (AgentNeighbors.Count < 10)
                {
                    AgentNeighbors.Add(fixedAgent);
                    AgentNeighborSqrDists.Add(dist);
                }
                var i = AgentNeighbors.Count - 1;
                if (dist < AgentNeighborSqrDists[i])
                {
                    while (i != 0 && dist < AgentNeighborSqrDists[i - 1])
                    {
                        AgentNeighbors[i] = AgentNeighbors[i - 1];
                        AgentNeighborSqrDists[i] = AgentNeighborSqrDists[i - 1];
                        i--;
                    }
                    AgentNeighbors[i] = fixedAgent;
                    AgentNeighborSqrDists[i] = dist;
                }

                if (AgentNeighbors.Count == 10)
                    rangeSq = AgentNeighborSqrDists[AgentNeighbors.Count - 1];
            }
            return rangeSq;
        }
        #endregion
        #region ISteering Impl
        public Vector3d Velocity { get; set; }
        public long Speed { get { return AttributeManager[AttributeType.Speed]; } }
        public long Radius { get; set; }
        public Vector3d Acceleration { get; set; }
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
            AgentNeighbors = new List<IFixedAgent>();
            AgentNeighborSqrDists = new List<long>();
            AttributeManager.New(AttributeType.Speed, 0);
            AttributeManager.New(AttributeType.MaxSpeed, Lockstep.FixedMath.One * 2);
            AttributeManager.New(AttributeType.Maxhp, Lockstep.FixedMath.One * 100);
            AttributeManager.New(AttributeType.Hp, Lockstep.FixedMath.One * 300);
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
            var acceleration = SteeringManager.GetDesiredAcceleration();
            Velocity += acceleration.Mul(LockFrameMgr.FixedFrameTime);
            Position += Velocity.Mul(LockFrameMgr.FixedFrameTime);
        }

        internal override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);
        }

    }
}
