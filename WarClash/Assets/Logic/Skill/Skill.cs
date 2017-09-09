using System;
using System.Collections.Generic;
using Logic.LogicObject;


namespace Logic.Skill
{
    [Display("技能")]
    [Serializable]
    public class Skill : TimeLineGroup
    {

    }

    public class RuntimeSkill : RuntimeTimeLineGroup
    {
        
    }
    public struct RuntimeData
    {
        public RuntimeData(SceneObject sender, SceneObject receiver, object data)
        {
            this.sender = sender;
            this.receiver = receiver;
            this.data = data;
        }
        public SceneObject sender;
        public SceneObject receiver;
        public object data;
    }
}
