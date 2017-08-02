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
        private static JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
#if UNITY_EDITOR
        public static void SaveTimelineGroup(TimeLineGroup skill, string path)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            string text = Newtonsoft.Json.JsonConvert.SerializeObject(skill, Formatting.Indented, settings);
            File.WriteAllText(path, text);
        }
#endif
        public static T GetTimelineGroupFullPath<T>(string path) where T : TimeLineGroup
        {
            string text = File.ReadAllText(path);
            T t = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(text, settings);
            return t;
        }
        public static T GetTimelineGroup<T>(string path) where T : TimeLineGroup
        {
            string text = Utility.ReadStringFromStreamingAsset(path);
            T t = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(text, settings);
            return t;
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
                skill = GetTimelineGroup<Skill>("Skills/" + path);
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
            SkillRunningData srd = new SkillRunningData(so, null, null);
            ReleaseSkill(path, srd);
        }
        internal void ReleaseSkill(string path, SceneObject target)
        {
            SkillRunningData srd = new SkillRunningData(so, target, null);
            ReleaseSkill(path, srd);
        }
        private void ReleaseSkill(string path, SkillRunningData srd)
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
