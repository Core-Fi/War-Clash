using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
}
