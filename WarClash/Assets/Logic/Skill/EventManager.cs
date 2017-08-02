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
        private static Dictionary<string, Logic.Skill.Event> events = new Dictionary<string, Logic.Skill.Event>();
        private static List<RuntimeEvent> runtimeEvents = new List<RuntimeEvent>();
        private static Dictionary<int, string> event_index = new Dictionary<int, string>();

        public static void LoadSkillIndexFiles()
        {
            event_index = Logic.Skill.SkillUtility.LoadIndexFile("/Events");
        }
        //private static Dictionary<string, RuntimeSkill> runtimeskills = new Dictionary<string, RuntimeSkill>();
        private static Logic.Skill.Event GetEvent(string path)
        {
            Logic.Skill.Event _event = null;
            if (events.ContainsKey(path))
            {
                _event = events[path];
            }
            else
            {
                _event = SkillUtility.GetTimelineGroup<Event>("Events/" + path);
                events[path] = _event;
            }
            return _event;
        }
        public static void AddEvent(int id)
        {

        }
        public static void AddEvent(string path, RuntimeData runnignData)
        {
            if(string.IsNullOrEmpty(path)) return;
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
