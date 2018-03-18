using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Logic;
using Logic.LogicObject;
using Logic.Skill;
using Newtonsoft.Json;
using Logic.Skill.Actions;

class FixBuildingAction : BaseAction
{
    [Display("增加血量")]
    [JsonProperty]
    public int IncreaseHp { get; private set; }

    public override void Execute(SceneObject sender, SceneObject reciever, object data)
    {
        var building = reciever;
        if (building == null) return;
        building.Hp += IncreaseHp;
        base.Execute(sender, reciever, data);
    }
}
