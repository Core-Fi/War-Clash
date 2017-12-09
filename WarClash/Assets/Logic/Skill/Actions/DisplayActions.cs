using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Logic.LogicObject;

namespace Logic.Skill.Actions
{
    [Display("动作")]
    [Serializable]
    public class PlayAnimationAction : DisplayAction
    {
        [Display("动作名")]
        [JsonProperty]
        public string animaitonName { get; private set; }
    }

    [Display("特效")]
    [Serializable]
    public class PlayFXAction : DisplayAction
    {
        [Display("特效名")]
        [JsonProperty]
        public string FXName { get; private set; }
    }

    public class GuiseAction : DisplayAction
    {
        [JsonIgnore]
        public string Path { get; private set; }

        public override void Execute(SceneObject sender, SceneObject reciever, object data)
        {
            base.Execute(sender, reciever, data);
        }
    }
}
