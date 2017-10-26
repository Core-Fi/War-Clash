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
    public class Projectile : SceneObject
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
        public CreateProjectileAction ProjectileAction { get; private set; }
        private Vector3d _moveDir;
        private long _speed;
        private Vector3d _initPosi;
        private long _distance;
        internal virtual void SetValue(CreateProjectileAction action, SceneObject sender, SceneObject receiver, object data)
        {
            ProjectileAction = action;
            this.Sender = sender;
            this.Receiver = receiver;
            this.Data = data;
            Position = sender.Position;
            _speed = (FixedMath.One * action.distance / 100).Div(action.time*FixedMath.One/1000);
            if (this.Receiver == null)
            {
                _distance = (FixedMath.One * action.distance / 100);
                _initPosi = sender.Position;
                _moveDir = sender.Forward;
                this.Forward = _moveDir;
            }
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
            if (Receiver != null)
            {
                this.Forward = (Receiver.Position - Position).Normalize();
                Position += Forward.Mul(_speed).Mul(FixedMath.One.Div(LockFrameMgr.FixedFrameRate));
                if (Vector3d.SqrDistance(Position, Receiver.Position) < FixedMath.Half/5)
                {
                    OnHit(Receiver);
                }
            }
            else
            {
                Position += Forward .Mul( _speed) .Mul(FixedMath.One.Div(LockFrameMgr.FixedFrameRate));
                if (Vector3d.SqrDistance(Position, _initPosi) > _distance.Mul(_distance))
                {
                    OnHit(null);
                }
                else
                {
                    LogicCore.SP.SceneManager.CurrentScene.ForEachDo<Player>((p) =>
                    {
                        if (Vector3d.SqrDistance(Position, p.Position) < FixedMath.One / 5)
                        {
                            OnHit(p);
                        }
                    });
                }
            }
        }

        public void OnHit(SceneObject receiver)
        {
            EventManager.AddEvent(ProjectileAction.hitEvent, new RuntimeData(Sender, receiver, Data));
            EventGroup.FireEvent((int)ProjectileEvent.OnHit, this, null);
            LogicCore.SP.SceneManager.CurrentScene.RemoveSceneObject(this.Id);
        }

    }
}
