using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Logic.Skill
{
    public class EventManager
    {
        private static Dictionary<string, Logic.Skill.Event> skills = new Dictionary<string, Logic.Skill.Event>();
        private static List<RuntimeEvent> runtimeEvents = new List<RuntimeEvent>();  
        //private static Dictionary<string, RuntimeSkill> runtimeskills = new Dictionary<string, RuntimeSkill>();
        private static Logic.Skill.Event GetEvent(string path)
        {
            Logic.Skill.Event skill = null;
            if (skills.ContainsKey(path))
            {
                skill = skills[path];
            }
            else
            {
                string text = File.ReadAllText(path);
                skill = Newtonsoft.Json.JsonConvert.DeserializeObject<Logic.Skill.Event>(text);
                skills[path] = skill;
            }
            return skill;
        }

        public static void AddEvent(string path, SkillRunningData runnignData)
        {
            if(string.IsNullOrEmpty(path)) return;
            path = Application.streamingAssetsPath + "/Events/" + path;
            var skill = GetEvent(path);
            RuntimeEvent re = new RuntimeEvent();
            re.Init(skill, runnignData, null);
            runtimeEvents.Add(re);
        }
        public static void Update(float deltaTime)
        {
            for (int i = 0; i < runtimeEvents.Count; i++)
            {
                runtimeEvents[i].Breath(deltaTime);
            }
        }
    }

    public class RuntimeEvent : RuntimeTimeLineGroup
    {
        
    }
}
