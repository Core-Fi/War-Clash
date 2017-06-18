﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Logic.LogicObject;

namespace Logic.Skill.Actions
{
    [Display("创建事件")]
    [Serializable]
    public class ReleaseEvent : BaseAction
    {
        [Display("事件效果")]
        public string path { get; private set; }

        public override void Execute(SceneObject sender, SceneObject reciever, object data)
        {
            EventManager.AddEvent(path, new SkillRunningData(sender, reciever, data));
            base.Execute(sender, reciever, data);
        }
    }
}