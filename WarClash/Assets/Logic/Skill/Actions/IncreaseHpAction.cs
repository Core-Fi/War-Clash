using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lockstep;
using Logic.LogicObject;

namespace Logic.Skill.Actions
{
    public class IncreaseHpAction : BaseAction
    {
        public DataBind<int> Hp; 
        public override void Execute(SceneObject sender, SceneObject reciever, object data)
        {
            if (reciever != null)
            {
                reciever.Hp += FixedMath.Create(Hp.value);
            }
            base.Execute(sender, reciever, data);
        }
    }
}