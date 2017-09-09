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
        private static readonly Dictionary<string, Skill> skills =new Dictionary<string, Skill>();
        private static Dictionary<int, string> skill_index = new Dictionary<int, string>();

        public static void LoadSkillIndexFiles()
        {
            skill_index = Logic.Skill.SkillUtility.LoadIndexFile("/Skills");
        }
        public static Skill GetSkill(string path)
        {
            Skill skill = null;
            if (Skills.ContainsKey(path))
            {
                skill = Skills[path];
            }
            else
            {
                skill = Logic.Skill.SkillUtility.GetTimelineGroup<Skill>(path);
                Skills[path] = skill;
            }
            return skill;
        }
        public RuntimeSkill RunningSkill { get; private set; }
        private Character so;

        public bool IsRunningSkill
        {
            get { return RunningSkill != null; }
        }

        public static Dictionary<string, Skill> Skills
        {
            get
            {
                return Skills1;
            }
        }

        public static Dictionary<string, Skill> Skills1
        {
            get
            {
                return skills;
            }
        }

        internal void CancelSkill()
        {
            this.so.EventGroup.FireEvent((int)Character.CharacterEvent.Cancelskill, so, EventGroup.NewArg<EventSingleArgs<string>, string>(RunningSkill.sourceData.path));
            Pool.SP.Recycle(RunningSkill);
            RunningSkill = null;
        }
        public SkillManager(Character so)
        {
            this.so = so;
        }
        internal void ReleaseSkill(int id)
        {
            if(skill_index.Count==0)
                LoadSkillIndexFiles();
            RuntimeData srd = new RuntimeData(so, null, null);
            string path = skill_index[id];
            ReleaseSkill(path, srd);
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
            RunningSkill = Pool.SP.Get(typeof(RuntimeSkill)) as RuntimeSkill;
            RunningSkill.Init(skill, srd, OnFinish);
            this.so.EventGroup.FireEvent((int)Character.CharacterEvent.Startskill, so, EventGroup.NewArg<EventSingleArgs<string>, string>(path));
        }
        internal void Update(float deltaTime)
        {
            if (RunningSkill != null)
            {
                RunningSkill.Breath(deltaTime);
            }
        }

        internal void FixedUpdate()
        {
            if (RunningSkill != null)
            {
                RunningSkill.FixedBreath();
            }
        }
        internal void OnFinish()
        {
            so.EventGroup.FireEvent((int)Character.CharacterEvent.Endskill, so, EventGroup.NewArg<EventSingleArgs<string>, string>(RunningSkill.sourceData.path));
            Pool.SP.Recycle(RunningSkill);
            RunningSkill = null;
        }
    }
}
