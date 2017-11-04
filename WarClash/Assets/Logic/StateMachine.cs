using Lockstep;
using Logic.LogicObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pathfinding;
using UnityEngine;
using UnityEngine.AI;

namespace Logic
{
    public class StateMachine
    {
        public State state;
        public Character Character;
        public StateMachine(Character character)
        {
            this.Character = character;
        }
        public void Start<T>() where T : State, IPool
        {
            if(this.state != null)
            {
                if (this.state is T)
                {
                    return;
                }
                else
                {
                    this.state.Stop();
                }
            }
            this.state = Pool.SP.Get<T>() as State;
            this.state.Character = Character;
            this.state.Start();

        }

        public void Update()
        {
            if(state != null)
            {
                state.Update();
            }
        }
    }
    public abstract class State : IPool
    {
        public Character Character;

        public void Start()
        {
            OnStart();
        }

        public void Update()
        {
            OnUpdate();
        }

        public void Stop()
        {
            OnStop();
        }
        protected abstract void OnStart();
        protected abstract void OnUpdate();
        protected abstract void OnStop();
        public void Reset()
        {
           
        }
    }

    public class IdleState : State
    {
        protected override void OnStart()
        {
            Character.AttributeManager.SetBase(AttributeType.Speed, 0);
        }

        protected override void OnStop()
        {

        }
        protected override void OnUpdate()
        {

        }
    }

    public class MoveState : State
    {
        protected override void OnStart()
        {
            var speed = Character.AttributeManager[AttributeType.MaxSpeed];
            Character.AttributeManager.SetBase(AttributeType.Speed, speed);
            Character.Velocity = Character.Forward * speed;
        }
        protected override void OnStop()
        {
            Character.AttributeManager.SetBase(AttributeType.Speed, 0);
        }
        protected override void OnUpdate()
        {
            Vector3d posi = Character.Forward;
            posi.Mul(Character.AttributeManager[AttributeType.Speed]);
            posi = posi.Div(LockFrameMgr.FixedFrameRate);
            var finalPosi = Character.Position + posi;
            //var fp = AstarPath.active.GetNearest(finalPosi);
            //var b = (fp.node as MeshNode).ContainsPoint(Vector3d.ToInt3(finalPosi));
            var b = true;//JPSAStar.active.IsWalkable(finalPosi);
            if (LogicCore.SP.WriteToLog)
            {
                LogicCore.SP.Writer.AppendLine(Character.Position.ToStringRaw()+" "+Character.Forward.ToStringRaw());
            }
            if (b)
            {
                Character.Position = finalPosi;
            }
            else
            {
                //Character.Position = fp.constFixedClampedPosition;
                
            }
        }
    }
}
