using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using Logic.LogicObject;
using Newtonsoft.Json;
using UnityEngine;

namespace Logic.Skill
{
   
    public class SkillManager 
    {
        private static Dictionary<string, Skill> skills =new Dictionary<string, Skill>();
        private static Dictionary<int, string> skill_index = new Dictionary<int, string>();

        public static void LoadSkillIndexFiles()
        {
            skill_index = Logic.Skill.SkillUtility.LoadIndexFile("/Skills");
        }
        public static Skill GetSkill(string path)
        {
            Skill skill = null;
            if (skills.ContainsKey(path))
            {
                skill = skills[path];
            }
            else
            {
                skill = Logic.Skill.SkillUtility.GetTimelineGroup<Skill>("Skills/" + path);
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
            Pool.SP.Recycle(runningSkill);
            runningSkill = null;
        }
        public SkillManager(Character so)
        {
            this.so = so;
        }
        internal void ReleaseSkill(string path)
        {
            RuntimeData srd = new RuntimeData(so, null, null);
            ReleaseSkill(path, srd);
        }
        internal void ReleaseSkill(string path, SceneObject target)
        {
            RuntimeData srd = new RuntimeData(so, target, null);
            ReleaseSkill(path, srd);
        }
        private void ReleaseSkill(string path, RuntimeData srd)
        {
            var skill = GetSkill(path);
            runningSkill = Pool.SP.Get(typeof(RuntimeSkill)) as RuntimeSkill;
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
            Pool.SP.Recycle(runningSkill);
            runningSkill = null;
        }

    }
}
