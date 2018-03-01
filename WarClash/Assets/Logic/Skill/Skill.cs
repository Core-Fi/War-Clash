using System;
using System.Collections.Generic;
using Logic.LogicObject;


namespace Logic.Skill
{
    [Display("技能")]
    [Serializable]
    public class Skill : TimeLineGroup
    {
        [Display("技能时面向目标")]
        [Newtonsoft.Json.JsonProperty]
        public bool ForceFaceToTarget { get; private set; }
        public Skill()
        {
            ForceFaceToTarget = true;
        }
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



    [Display("技能")]
    [Serializable]
    public class TestSkill : Skill
    {
        [Display("Test")]
        [Newtonsoft.Json.JsonProperty]
        public string Test { get; private set; }
        public TestSkill()
        {
        }
    }
}
