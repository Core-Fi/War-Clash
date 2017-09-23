using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Logic.LogicObject;
using Lockstep;

namespace Logic.Skill.Actions
{
    [Display("伤害节点")]
    [Serializable]
    public class DamageAction : BaseAction
    {
        [Display("伤害")]
        public DataBind<int> Damage { get; private set; }
        [Display("选择范围", "范围类型", UIControlType.MutiSelection, typeof(Range))]
        [Newtonsoft.Json.JsonProperty]
        public int range { get; private set; }
    
        [Display("范围", "范围类型")]
        [Newtonsoft.Json.JsonProperty]
        public Range r { get; private set; }

        public DamageAction()
        {
            Damage = new DataBind<int>();
        }
        public override void Execute(SceneObject sender, SceneObject reciever, object data)
        {
            base.Execute(sender, reciever, data);
            var character = reciever as Character;
            if (character == null)
            {
                if (r == Range.RECT)
                {
                    character = LogicCore.SP.SceneManager.currentScene.ForEachDo<Player>((p) =>
                    {
                        if (p!=sender && p.Hp>0 && Utility.PositionIsInRect(new Utility.FixedRect() {center = new Vector2d(),
                            width = FixedMath.One*2,
                            height = FixedMath.One*2}, sender.Position, FixedQuaternion.identity,
                            p.Position))
                        {
                            return true;
                        }
                        return false;
                    });
                }
                if(character!=null)
                    character.Hp = character.Hp.Sub(Damage.value*FixedMath.One);
            }
            else
            {
                character.Hp = character.Hp.Sub(Damage.value * FixedMath.One);
            }
        }

    }
}
