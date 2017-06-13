using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using Logic.LogicObject;

namespace Logic.Skill
{
   
    public class SkillManager 
    {
        private static Dictionary<string, Skill> skills =new Dictionary<string, Skill>();
        public static Skill GetSkill(string path)
        {
            Skill skill = null;
            if (skills.ContainsKey(path))
            {
                skill = skills[path];
            }
            else
            {
                string text = File.ReadAllText(path);
                skill = Newtonsoft.Json.JsonConvert.DeserializeObject<Logic.Skill.Skill>(text);
                skills[path] = skill;
            }
            return skill;
        }

        public RuntimeSkill runningSkill { get; private set; }
        private Character so;

        public bool IsRunningSkill
        {
            get { return runningSkill != null; }
        }

        internal void CancelSkill()
        {
            this.so.EventGroup.FireEvent((int)Character.CharacterEvent.CANCELSKILL, so, EventGroup.NewArg<EventSingleArgs<string>, string>(runningSkill.sourceData.path));
            runningSkill = null;
        }
        public SkillManager(Character so)
        {
            this.so = so;
        }
        internal void ReleaseSkill(string path)
        {
            var skill = GetSkill(path);
            SkillRunningData srd = new SkillRunningData(so, null, null);
            runningSkill = new RuntimeSkill();
            runningSkill.Init(skill, srd, OnFinish);
            this.so.EventGroup.FireEvent((int)Character.CharacterEvent.STARTSKILL, so, EventGroup.NewArg<EventSingleArgs<string>, string>(path));
        }
        internal void Update(float deltaTime)
        {
            if (runningSkill != null)
            {
                runningSkill.Breath(deltaTime);
            }
        }

        internal void OnFinish()
        {
            so.EventGroup.FireEvent((int)Character.CharacterEvent.ENDSKILL, so, EventGroup.NewArg<EventSingleArgs<string>, string>(runningSkill.sourceData.path));
            runningSkill = null;
        }

    }
}
