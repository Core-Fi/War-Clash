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
                    Character.EventGroup.FireEvent(Character.CharacterEvent.StopState.ToInt(), this, EventGroup.NewArg<EventSingleArgs<State>, State>(this.state));
                }
            }
            this.state = Pool.SP.Get<T>() as State;
            this.state.Character = Character;
            this.state.Start();
            Character.EventGroup.FireEvent(Character.CharacterEvent.ExecuteState.ToInt(), this, EventGroup.NewArg<EventSingleArgs<State>, State>(this.state));
        }

        public void FixedUpdate()
        {
            if(state != null)
            {
                state.FixedUpdate();
            }
        }
    }
    public abstract class State : IPool
    {
        public Character Character;
        public bool IsRunning { get; private set; }

        public void Start()
        {
            IsRunning = true;
            OnStart();
        }

        public void FixedUpdate()
        {
            OnFixedUpdate();
        }

        public void Stop()
        {
            OnStop();
            IsRunning = false;
        }
        protected abstract void OnStart();
        protected abstract void OnFixedUpdate();
        protected abstract void OnStop();
        protected abstract void OnReset();
        public void Reset()
        {
           OnReset();
        }
    }

    public class GuiseState : State
    {
        private Player _p = null;
        public string Path { get; private set; }

        protected override void OnReset()
        {
            Path = string.Empty;
            _p= null;
        }

        protected override void OnStart()
        {
            if (Character is Player)
            {
                _p = Character as Player;
                Path = "cube.prefab";
            }
        }
        protected override void OnStop()
        {
        }

        protected override void OnFixedUpdate()
        {
        }
    }
    public class IdleState : State
    {
        protected override void OnStart()
        {
            Character.AttributeManager.SetBase(AttributeType.Speed, 0);
        }
        protected override void OnReset()
        {
        }
        protected override void OnStop()
        {

        }
        protected override void OnFixedUpdate()
        {

        }
    }

    public class MoveState : State
    {
        protected override void OnReset()
        {
        }
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
        protected override void OnFixedUpdate()
        {
            if (Character.GetStatus(AttributeType.IsMovable))
            {
                Vector3d posi = Character.Forward;
                posi.Mul(Character.AttributeManager[AttributeType.Speed]);
                posi = posi.Div(LockFrameMgr.FixedFrameRate);
                var finalPosi = Character.Position + posi;
                //var fp = AstarPath.active.GetNearest(finalPosi);
                //var b = (fp.node as MeshNode).ContainsPoint(Vector3d.ToInt3(finalPosi));
                var b = true; //JPSAStar.active.IsWalkable(finalPosi);
                if (LogicCore.SP.WriteToLog)
                {
                    LogicCore.SP.Writer.AppendLine(Character.Position.ToStringRaw() + " " +
                                                   Character.Forward.ToStringRaw());
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
}
