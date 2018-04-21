using Lockstep;
using Logic.LogicObject;

using Logic.Components;

namespace Logic
{
    public class StateMachine : SceneObjectBaseComponent
    {
        public enum Event
        {
            ExecuteState,
            StopState
        }
        public State State;
        public void Start<T>() where T : State, IPool
        {
            if(this.State != null)
            {
                if (this.State is T)
                {
                    return;
                }
                else
                {
                    this.State.Stop();
                    SceneObject.EventGroup.FireEvent((int)Event.StopState, this, EventGroup.NewArg<EventSingleArgs<State>, State>(this.State));
                }
            }
            this.State = Pool.SP.Get<T>() as State;
            this.State.SceneObject = SceneObject;
            this.State.Start();
            SceneObject.EventGroup.FireEvent((int)Event.ExecuteState, this, EventGroup.NewArg<EventSingleArgs<State>, State>(this.State));
        }

        public void FixedUpdate()
        {
            if(State != null)
            {
                State.FixedUpdate();
            }
        }
    }
    public abstract class State : IPool
    {
        public SceneObject SceneObject;
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
            Pool.SP.Recycle(this);
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
        private SceneObject _p = null;
        public string Path { get; private set; }

        protected override void OnReset()
        {
            Path = string.Empty;
            _p= null;
        }

        protected override void OnStart()
        {
            Path = "cube.prefab";
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
            SceneObject.AttributeManager.SetBase(AttributeType.Speed, 0);
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
            var speed = SceneObject.AttributeManager[AttributeType.MaxSpeed];
            SceneObject.AttributeManager.SetBase(AttributeType.Speed, speed);
        }
        protected override void OnStop()
        {
            SceneObject.AttributeManager.SetBase(AttributeType.Speed, 0);
        }
        protected override void OnFixedUpdate()
        {
            if (SceneObject.GetStatus(AttributeType.IsMovable))
            {
                Vector3d posi = SceneObject.Forward;
                posi.Mul(SceneObject.AttributeManager[AttributeType.Speed]);
                posi = posi.Div(LockFrameMgr.FixedFrameRate);
                var finalPosi = SceneObject.Position + posi;
                SceneObject.Position = finalPosi;
            }
        }
    }
    public class JumpState : State
    {
        public long InitSpeed = FixedMath.One * 15;
        protected override void OnStart()
        {
            SceneObject.TransformComp.Velocity.y += InitSpeed;
            SceneObject.TransformComp.EventGroup.ListenEvent((int)TransformComponent.Event.OnHitGround, OnHitGround);
        }
        protected override void OnStop()
        {

        }
        protected override void OnReset()
        {

        }
        protected override void OnFixedUpdate()
        {
        }
        private  void OnHitGround(object sender, EventMsg e)
        {
            var sm = SceneObject.GetComponent<StateMachine>();
            sm.Start<IdleState>();
        }
    }
}
