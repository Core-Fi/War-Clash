using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Logic.Skill;
using Logic.Skill.Actions;
using UnityEngine;

namespace Logic.LogicObject
{
    public class Projectile : SceneObject
    {
        public enum ProjectileEvent 
        {
            ONHIT = 200
        }
        private int hitCount;
        private List<Character> targets = new List<Character>();
        public SceneObject previousSo { get; private set; }
        public SceneObject sender { get; private set; }
        public SceneObject receiver { get; private set; }
        public object data { get; private set; }
        public CreateProjectileAction projectileAction { get; private set; }

        internal virtual void SetValue(CreateProjectileAction action, SceneObject sender, SceneObject receiver, object data)
        {
            projectileAction = action;
            this.sender = sender;
            this.receiver = receiver;
            this.data = data;
        }
        
        public bool SetTarget()
        {
            List<Character> allPlayers = new List<Character>(LogicCore.SP.sceneManager.currentScene.Count<Character>());

            LogicCore.SP.sceneManager.currentScene.ForEachDo<Character>((character) =>
            {
                if (character != sender && !targets.Contains(character))
                {
                   allPlayers.Add(character);
                }
            });
            if (allPlayers.Count > 0)
            {
                //allPlayers.Sort((a, b) =>
                //{
                //    Lockstep.Vector3d
                //    return (int) (Vector3.Distance(a.Position, this.Position)*100) -
                //           (int) (Vector3.Distance(b.Position, this.Position)*100);
                //});
                previousSo = receiver;
                receiver = allPlayers[0];
                targets.Add(allPlayers[0]);
                return true;
            }
            else
            {
                return false;
            }
        }
        internal override void OnInit()
        {
            base.OnInit();
        }
        float XZDistance(Vector3 a, Vector3 b)
        {
            a.y = 0;
            b.y = 0;
            return Vector3.Distance(a, b);
        }
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

        public void OnHit()
        {
            hitCount++;
            EventManager.AddEvent(projectileAction.hitEventEffectPath, new SkillRunningData(sender, receiver, data));
            EventGroup.FireEvent((int)ProjectileEvent.ONHIT, this, null);
            bool hasTarget = SetTarget();
            if (hitCount > projectileAction.hitCount || !hasTarget)
            {
                LogicCore.SP.sceneManager.currentScene.RemoveSceneObject(this.ID);
            }
        }

    }
}
