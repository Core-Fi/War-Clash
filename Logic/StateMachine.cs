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

    public class MoveState : State
    {
        public Vector3d dir;
        public long speed;
        public override void OnStart()
        {

        }
        public override void OnStop()
        {

        }
        public override void OnUpdate()
        {
            Vector3d posi = dir;
            posi.Mul(speed);
            posi.Mul(FixedMath.One.Div(FixedMath.One * 15));
            character.Position += posi;
        }
    }
}
