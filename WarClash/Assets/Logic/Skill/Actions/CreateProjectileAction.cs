using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Logic.LogicObject;

using UnityEngine;

namespace Logic
{
    // 摘要: 
    //     A single keyframe that can be injected into an animation curve.
    public class CustomKeyframe
    {
        public CustomKeyframe()
        {
            
        }
        public CustomKeyframe(float time, float value)
        {
            this.time = time;
            this.value = value;
        }

        public CustomKeyframe(float time, float value, float inTangent, float outTangent)
        {
            this.time = time;
            this.value = value;
            this.inTangent = inTangent;
            this.outTangent = outTangent;
        }

        public float inTangent { get; set; }
        public float outTangent { get; set; }
        public float time { get; set; }
        public float value { get; set; }
        public int tangentMode { get; set; }
    }
    [Serializable]
    public class CustomAnimationCurve
    {
        public CustomAnimationCurve()
        {
            
        }
        public CustomAnimationCurve(CustomKeyframe[] kfs)
        {
            keyframes = kfs;
        }
        public CustomKeyframe[] keyframes { get; private set; }

        private AnimationCurve animationCurve { get; set; }

        public float Evaluate(float time)
        {
            if (animationCurve == null)
            {
                if (keyframes != null)
                {
                    Keyframe[] keys = new Keyframe[keyframes.Length];
                    for (int j = 0; j < keys.Length; j++)
                    {
                        var ori_key = keyframes[j];
                        keys[j] = new Keyframe(ori_key.time, ori_key.value, ori_key.inTangent, ori_key.outTangent);
                        keys[j].tangentMode = ori_key.tangentMode;
                    }
                    animationCurve = new AnimationCurve(keys);
                }
            }
            return animationCurve.Evaluate(time);
        }
    }
}

namespace Logic.Skill.Actions
{
    [Display("创建子弹")]
    [Serializable]
    public class CreateProjectileAction : BaseAction
    {
        [Display("距离")]
        public int distance { get; private set; }

        [Display("时间")]
        public float time { get; private set; }
        [Display("弹跳次数")]
        public int hitCount { get; private set; }

        [Display("碰撞事件效果")]
        public string hitEventEffectPath { get; private set; }
        [Display("死亡事件效果")]
        public string dieEventEffectPath { get; private set; }
        [Display("曲线")]
        public CustomAnimationCurve cac { get; private set; }

        public override void Execute(SceneObject sender, SceneObject reciever, object data)
        {
            var projectile = LogicCore.SP.sceneManager.currentScene.CreateSceneObject<Projectile>();
            projectile.SetValue(this, sender, reciever, data);
            base.Execute(sender, reciever, data);
        }
    }
}
