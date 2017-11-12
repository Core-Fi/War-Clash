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
        [Display("距离")]
        [JsonProperty]
        public int distance { get; private set; }

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
        [Display("曲线")]
        [JsonProperty]
        public FixedAnimationCurve cac { get; private set; }

        public override void Execute(SceneObject sender, SceneObject reciever, object data)
        {
            var createInfo = Pool.SP.Get<CreateInfo>();
            createInfo.Id = IDManager.SP.GetID();
            createInfo.Position = sender.Position;
            createInfo.Forward = sender.Forward;
            var projectile = LogicCore.SP.SceneManager.CurrentScene.CreateSceneObject<Projectile>(createInfo);
            projectile.SetValue(this, sender, reciever, data);
            base.Execute(sender, reciever, data);
        }
    }
}
