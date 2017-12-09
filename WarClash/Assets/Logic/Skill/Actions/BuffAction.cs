using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Logic.LogicObject;

namespace Logic.Skill.Actions
{
    public abstract class BuffAction : BaseAction
    {
        public virtual void OnBuffFinish()
        {
            
        }
        public override void Execute(SceneObject sender, SceneObject reciever, object data)
        {
            base.Execute(sender, reciever, data);
        }
    }
}
