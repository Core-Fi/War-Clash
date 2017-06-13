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
        public string animaitonName { get; private set; }
    }

    [Display("特效")]
    [Serializable]
    public class PlayFXAction : DisplayAction
    {
        [Display("特效名")]
        public string FXName { get; private set; }
    }
}
