using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Logic.LogicObject;
using Lockstep;

namespace Logic.Skill.Actions
{
    public enum TargetType
    {
        Player = 1,
        Npc = 2,
        Tower = 4
    }
    public abstract class BaseSelectionAction : BaseAction
    {
        [Display("目标类型", "目标类型", UIControlType.MutiSelection, typeof(TargetType))]
        [Newtonsoft.Json.JsonProperty]
        public int Types { get; private set; }
        [Display("事件id")]
        [Newtonsoft.Json.JsonProperty]
        public int EventId { get; private set; }
        [Display("x偏移(单位厘米)")]
        [Newtonsoft.Json.JsonProperty]
        public int Offsetx { get; private set; }
        [Display("z偏移(单位厘米)")]
        [Newtonsoft.Json.JsonProperty]
        public int OffsetZ { get; private set; }
        public bool IsTarget(object so)
        {
            //if (so is Player && ((int)TargetType.Player & Types) != 0)
            //{
            //    return true;
            //}
            //else if (so is Npc && ((int) TargetType.Npc & Types) != 0)
            //{
            //    return true;
            //}
            //else if (so is Tower && ((int)TargetType.Tower & Types) != 0)
            //{
            //    return true;
            //}
            return false;
        }
    }
    [Display("范围选择/扇形")]
    public class FanSelectioNAction : BaseSelectionAction
    {
        [Display("角度")]
        [Newtonsoft.Json.JsonProperty]
        public int Angle { get; private set; }
        [Display("半径(单位厘米)")]
        [Newtonsoft.Json.JsonProperty]
        public int Radius { get; private set; }

        public override void Execute(SceneObject sender, SceneObject reciever, object data)
        {
            var r = FixedMath.Create(Radius) / 100;
            var q = FixedQuaternion.LookRotation(sender.Forward, Vector3d.up);
            Utility.List.Clear();
            var _battleScene = LogicCore.SP.SceneManager.CurrentScene as BattleScene;
            _battleScene.FixedQuadTree.Query(sender as IFixedAgent, r, Utility.List);
            for (int i = 0; i < Utility.List.Count; i++)
            {
                if (IsTarget(Utility.List[i]))
                {
                    if (Utility.PositionIsInFan(sender.Position, FixedMath.Create(Radius) / 100, Angle, q, Utility.List[i].Position))
                    {
                        EventManager.TriggerEvent(EventId, new RuntimeData(sender, Utility.List[i] as SceneObject, null));
                    }
                }
            }
;            base.Execute(sender, reciever, data);
        }
    }

    [Display("范围选择/矩形")]
    public class RectSelectionAction : BaseSelectionAction
    {
        [Display("宽(单位厘米)")]
        [Newtonsoft.Json.JsonProperty]
        public int Width { get; private set; }
        [Display("高(单位厘米)")]
        [Newtonsoft.Json.JsonProperty]
        public int Height { get; private set; }

        public override void Execute(SceneObject sender, SceneObject reciever, object data)
        {
            var _battleScene = LogicCore.SP.SceneManager.CurrentScene as BattleScene;
            long fw = FixedMath.Create(Width) / 100;
            long fh = FixedMath.Create(Height) / 100;
            long fr = FixedMath.Sqrt((fw / 2).Mul(fw / 2) + fh.Mul(fh));
            var center = new Vector2d(0, fh / 2);
            var q = FixedQuaternion.LookRotation(sender.Forward, Vector3d.up);
            Utility.FixedRect rect = new Utility.FixedRect(center, fw, fh);
            Utility.List.Clear();
            _battleScene.FixedQuadTree.Query(sender as IFixedAgent, fr, Utility.List);
            for (int i = 0; i < Utility.List.Count; i++)
            {
                if (IsTarget(Utility.List[i]))
                {
                    if (Utility.PositionIsInRect(rect,  sender.Position, q, Utility.List[i].Position))
                    {
                        EventManager.TriggerEvent(EventId, new RuntimeData(sender, Utility.List[i] as SceneObject, null));
                    }
                }
            }
            base.Execute(sender, reciever, data);
        }
    }

    [Display("伤害节点")]
    [Serializable]
    public class DamageAction : BaseAction
    {
        [Display("伤害")]
        public DataBind<int> Damage { get; private set; }
        public DamageAction()
        {
            Damage = new DataBind<int>();
        }
        public override void Execute(SceneObject sender, SceneObject reciever, object data)
        {
            base.Execute(sender, reciever, data);
            var character = reciever;
            if (character == null)
            {
            }
            else
            {
                var damage = Damage.value * FixedMath.One;
                sender.EventGroup.FireEvent(SceneObject.SceneObjectEvent.OnBeforeAttackOther.ToInt(), sender, EventGroup.NewArg<EventTwoArgs<SceneObject, long>, SceneObject, long>(reciever, damage));
                reciever.EventGroup.FireEvent(SceneObject.SceneObjectEvent.OnBeforeBeAttacked.ToInt(), sender, EventGroup.NewArg<EventSingleArgs< long>, long>(damage));
                character.Hp = character.Hp.Sub(damage);
                sender.EventGroup.FireEvent(SceneObject.SceneObjectEvent.OnAfterAttackOther.ToInt(), sender, EventGroup.NewArg<EventTwoArgs<SceneObject, long>, SceneObject, long>(reciever, damage));
                reciever.EventGroup.FireEvent(SceneObject.SceneObjectEvent.OnAfterBeAttacked.ToInt(), sender, EventGroup.NewArg<EventSingleArgs<long>, long>(damage));
            }
        }

    }
}
