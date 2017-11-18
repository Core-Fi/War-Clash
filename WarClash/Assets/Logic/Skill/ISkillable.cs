using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Logic.LogicObject;
using Logic.Skill;

namespace Assets.Logic.Skill
{
    public interface ISkillable
    {
        SkillManager SkillManager { get;}
        bool ReleaseSkill(int id, SceneObject target);

    }
}
