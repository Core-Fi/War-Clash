using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lockstep;
using Logic.LogicObject;
using Newtonsoft.Json;
using UnityEngine;

namespace Logic.Skill.Actions
{
    [Display("创建子弹")]
    [Serializable]
    public class CreateProjectileAction : BaseAction
    {
        [Display("速度")]
        [JsonProperty]
        public int Speed { get; private set; }

        [Display("时间(毫秒)")]
        [JsonProperty]
        public int time { get; private set; }
        [Display("弹跳次数")]
        [JsonProperty]
        public int hitCount { get; private set; }

        [Display("碰撞事件效果")]
        [JsonProperty]
        public int hitEvent { get; private set; }
        [Display("死亡事件效果")]
        [JsonProperty]
        public int dieEvent { get; private set; }
        [Display("子弹类型")]
        [JsonProperty]
        public ProjectileType ProjectileType { get; private set; }

        [Display("曲线")]
        [JsonProperty]
        public FixedAnimationCurve cac { get; private set; }


        public override void Execute(SceneObject sender, SceneObject reciever, object data)
        {
            var createInfo = Pool.SP.Get<CreateInfo>();
            createInfo.Id = IDManager.SP.GetID();
            createInfo.Position = sender.Position;
            createInfo.Forward = sender.Forward;
            Projectile projectile = null;
            if (ProjectileType == ProjectileType.Stright)
                projectile = LogicCore.SP.SceneManager.CurrentScene.CreateSceneObject<StrightProjectile>(createInfo);
            else if(ProjectileType == ProjectileType.Target)
            {
                projectile = LogicCore.SP.SceneManager.CurrentScene.CreateSceneObject<TargetProjectile>(createInfo);
            }
            projectile.Init(this, sender, reciever, data);
            base.Execute(sender, reciever, data);
        }
    }
}
