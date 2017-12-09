using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lockstep;
using Logic.LogicObject;
using Newtonsoft.Json;

namespace Logic.Skill.Actions
{
    [Display("改变属性")]
    [Serializable]
    public class ChangeAttributeAction : BaseAction
    {
        [Display("属性")]
        [Newtonsoft.Json.JsonProperty]
        public AttributeType AttributeType { get; private set; }
        [Display("加成")]
        [Newtonsoft.Json.JsonProperty]
        public Operation Operation { get; private set; }

        [Display("加成值(加减单位1，乘除单位100)")]
        [Newtonsoft.Json.JsonProperty]
        public DataBind<int> Value { get; private set; }

        [Display("Buff/技能结束后恢复")]
        [Newtonsoft.Json.JsonProperty]
        public bool RestoreOnFinish { get; private set; }
        [JsonIgnore]
        private bool _executed = false;
        [JsonIgnore]
        private AttributeMotifier _motifier;
        public ChangeAttributeAction()
        {
            Value = new DataBind<int>();
            RestoreOnFinish = true;
        }
        
        public override void Execute(SceneObject sender, SceneObject reciever, object data)
        {
            _executed = true;
            if (Operation == Operation.Absolute)
            {
                _motifier = new AttributeMotifier{Operation = Operation, Value = Value.value*FixedMath.One};
                sender.AttributeManager.Add(AttributeType, _motifier);
            }
            else
            {
                _motifier = new AttributeMotifier { Operation = Operation, Value = Value.value * FixedMath.One/100 };
                sender.AttributeManager.Add(AttributeType, _motifier);
            }
            base.Execute(sender, reciever, data);
        }
        public override void OnSkillFinish(SceneObject sender, SceneObject reciever, object data)
        {
            if (_executed && RestoreOnFinish)
            {
                if (Operation == Operation.Absolute)
                {
                    sender.AttributeManager.Remove(AttributeType, _motifier);
                }
                else
                {
                    sender.AttributeManager.Remove(AttributeType, _motifier);
                }
            }
            _motifier = default(AttributeMotifier);
            base.OnSkillFinish(sender, reciever, data);
        }
    }
}
