using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lockstep;
using Logic.Skill;
using Logic.Skill.Actions;
using UnityEngine;

namespace Logic.LogicObject
{
    public enum ProjectileType
    {
        Stright,
        Target,
    }
    public abstract class Projectile : SceneObject
    {
        public enum ProjectileEvent 
        {
            OnHit = 200
        }
        //private int hitCount;
        //private List<Character> targets = new List<Character>();
      //  public SceneObject PreviousSo { get; private set; }
        public SceneObject Sender { get; private set; }
        public SceneObject Receiver { get; private set; }
        public object Data { get; private set; }
        public long InitHeight = FixedMath.One * 2;
        public CreateProjectileAction ProjectileAction { get; private set; }
        protected long Speed;
        protected Vector3d MoveDir;
 
        public virtual void Init(CreateProjectileAction action, SceneObject sender, SceneObject receiver, object data)
        {
            EventGroup.Clear();
            ProjectileAction = action;
            this.Sender = sender;
            this.Receiver = receiver;
            this.Data = data;
            Position = sender.Position;
            Speed = (FixedMath.One * action.Speed / 100);
            OnInit();
            
        }

        protected virtual void OnInit()
        {
            
        }

        protected virtual bool IsFinish()
        {
            return false;
        }
        //public bool SetTarget()
        //{
        //    List<Character> allPlayers = new List<Character>(LogicCore.SP.SceneManager.currentScene.Count<Character>());

        //    LogicCore.SP.SceneManager.currentScene.ForEachDo<Character>((character) =>
        //    {
        //        if (character != sender && !targets.Contains(character))
        //        {
        //           allPlayers.Add(character);
        //        }
        //    });
        //    if (allPlayers.Count > 0)
        //    {
        //        //allPlayers.Sort((a, b) =>
        //        //{
        //        //    Lockstep.Vector3d
        //        //    return (int) (Vector3.Distance(a.Position, this.Position)*100) -
        //        //           (int) (Vector3.Distance(b.Position, this.Position)*100);
        //        //});
        //        previousSo = receiver;
        //        receiver = allPlayers[0];
        //        targets.Add(allPlayers[0]);
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}
        internal override void OnInit(CreateInfo createInfo)
        {
            base.OnInit(createInfo);

        }
        //float XZDistance(Vector3 a, Vector3 b)
        //{
        //    a.y = 0;
        //    b.y = 0;
        //    return Vector3.Distance(a, b);
        //}
        internal override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);
            //Vector3 dir = this.receiver.Position - this.Position;
            //Vector3 posi = dir.normalized * (20f) * deltaTime;
            //float totalDistance = this.previousSo == null
            //    ? XZDistance(this.sender.Position, this.receiver.Position)
            //    : XZDistance(this.previousSo.Position, this.receiver.Position);
            //float passedDistance = this.previousSo == null
            //    ? XZDistance(this.sender.Position, this.Position)
            //    : XZDistance(this.previousSo.Position, this.Position);
            //Vector3 finalPosi = this.Position + posi;
            //if (totalDistance != 0)
            //    finalPosi.y = this.projectileAction.cac.Evaluate(passedDistance / totalDistance) * 2;
            //this.Position = finalPosi;

            //if (Vector3.Distance(this.Position, this.receiver.Position) <
            //   1)
            //{
            //    this.OnHit();
            //}
        }
        
        internal override void OnFixedUpdate(long deltaTime)
        {
            base.OnFixedUpdate(deltaTime);
            Position += Forward.Mul(Speed).Mul(FixedMath.One.Div(LockFrameMgr.FixedFrameRate));
            if (IsFinish())
            {
                OnDie();
            }
        }

        protected virtual void OnHit(SceneObject receiver)
        {
            EventManager.TriggerEvent(ProjectileAction.hitEvent, new RuntimeData(Sender, receiver, Data));
            EventGroup.FireEvent((int)ProjectileEvent.OnHit, this, null);
            LogicCore.SP.SceneManager.CurrentScene.RemoveSceneObject(this.Id);
        }
        protected virtual void OnDie()
        {
            EventManager.TriggerEvent(ProjectileAction.dieEvent, new RuntimeData(Sender, null, Data));
            LogicCore.SP.SceneManager.CurrentScene.RemoveSceneObject(this.Id);
        }

    }

    public class StrightProjectile : Projectile
    {
        private long _leftTime;
        protected override void OnInit()
        {
            _leftTime = FixedMath.One * base.ProjectileAction.time / 1000;
        }

        internal override void OnFixedUpdate(long deltaTime)
        {
            _leftTime -= deltaTime;
            LogicCore.SP.SceneManager.CurrentScene.ForEachDo<Player>((p) =>
            {
                if (Vector3d.SqrDistance(Position, p.Position) < FixedMath.One / 5)
                {
                    OnHit(p);
                }
            });
            base.OnFixedUpdate(deltaTime);
        }

        protected override bool IsFinish()
        {
            return _leftTime <= 0;
        }
    }

    public class TargetProjectile : Projectile
    {
        internal override void OnFixedUpdate(long deltaTime)
        {
            Forward = (Receiver.Position - Position).Normalize();
            base.OnFixedUpdate(deltaTime);

        }

        protected override bool IsFinish()
        {
            return Vector3d.SqrDistance(Position, Receiver.Position) < FixedMath.Half / 5;
        }
    }
}


