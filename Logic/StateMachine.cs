using Lockstep;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Logic
{
    public abstract class State
    {
        public abstract void OnStart();
        public abstract void OnUpdate();
        public abstract void OnStop();
    }

    public class MoveState : State
    {
        public Vector3d dir;
        public float speed;
        public override void OnStart()
        {
        }

        public override void OnStop()
        {
        }

        public override void OnUpdate()
        {
        }
    }


}
