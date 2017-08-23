using Lockstep;
using Logic.LogicObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Logic
{
    public class StateMachine
    {
        public State state;
        public Character character;
        public StateMachine(Character character)
        {
            this.character = character;
        }
        public void Start(State state)
        {
            if(this.state != null)
            {
                this.state.OnStop();
            }
            this.state = state;
            this.state.character = character;
            this.state.OnStart();
        }

        public void Update()
        {
            if(state != null)
            {
                state.OnUpdate();
            }
        }
    }
    public abstract class State
    {
        public Character character;
        public abstract void OnStart();
        public abstract void OnUpdate();
        public abstract void OnStop();
    }

    public class IdleState : State
    {
        public override void OnStart()
        {
            character.AttributeManager.SetBase(AttributeType.Speed, 0);
        }

        public override void OnStop()
        {

        }

        public override void OnUpdate()
        {

        }
    }

    public class MoveState : State
    {
      //  public Vector3d dir;
       // public long speed;
       // private FixedQuaternion rotateRate;
       // private Vector3d startPosi;
        public override void OnStart()
        {
            var speed = character.AttributeManager[AttributeType.MaxSpeed];
            character.AttributeManager.SetBase(AttributeType.Speed, speed);
           // rotateRate = FixedQuaternion.AngleAxis(FixedMath.One*1, new Vector3d(UnityEngine.Vector3.up));
         //   startPosi = character.Position;
        }
        public override void OnStop()
        {
            character.AttributeManager.SetBase(AttributeType.Speed, 0);
        }
        public override void OnUpdate()
        {
           // dir = rotateRate * dir;
            Vector3d posi = character.Forward;
            posi.Mul(character.AttributeManager[AttributeType.Speed]);
            posi.Mul(FixedMath.One.Div(FixedMath.One * 15));
            character.Position += posi;
        //    character.Forward = dir;
        }
    }
}
