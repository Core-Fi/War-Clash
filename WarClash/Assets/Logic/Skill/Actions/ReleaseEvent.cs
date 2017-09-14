
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
        public int EventId { get; private set; }

        public override void Execute(SceneObject sender, SceneObject reciever, object data)
        {
            EventManager.AddEvent(EventId, new RuntimeData(sender, reciever, data));
            base.Execute(sender, reciever, data);
        }
    }
}
