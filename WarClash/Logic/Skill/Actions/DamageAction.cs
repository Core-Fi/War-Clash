using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Logic.Skill.Actions
{
    [Display("伤害节点")]
    [Serializable]
    public class DamageAction : BaseAction
    {
        [Display("伤害")]
        public DataBind<int> Damage { get; private set; }
        [Display("选择范围", "范围类型", UIControlType.MutiSelection, typeof(Range))]
        public int range { get; private set; }
        [Display("文本", "范围类型")]
        public string text { get; private set; }
        [Display("范围", "范围类型")]
        public Range r { get; private set; }

        public DamageAction()
        {
            Damage = new DataBind<int>();
        }
    }
}
