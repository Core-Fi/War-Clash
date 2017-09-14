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
        private static Dictionary<int, Logic.Skill.Event> events = new Dictionary<int, Logic.Skill.Event>();
        private static List<RuntimeEvent> runtimeEvents = new List<RuntimeEvent>();
        private static Dictionary<int, string> event_index = new Dictionary<int, string>();

        public static void LoadEventIndexFiles()
        {
            event_index = Logic.Skill.SkillUtility.LoadIndexFile("/Events");
        }

        private static string GetEventPath(int id)
        {
            if (event_index.Count == 0)
            {
                LoadEventIndexFiles();
            }
            var path = event_index[id];
            return path;
        }
        //private static Dictionary<string, RuntimeSkill> runtimeskills = new Dictionary<string, RuntimeSkill>();
        private static Logic.Skill.Event GetEvent(int id)
        {
            Event e;
            if (events.TryGetValue(id, out e))
            {
                return e;
            }
            var path = GetEventPath(id);
            e = SkillUtility.GetTimelineGroup<Event>(path);
            events[id] = e;
            return e;
        }
        public static void AddEvent(int id, RuntimeData runnignData)
        {
            var skill = GetEvent(id);
            RuntimeEvent re = new RuntimeEvent();
            re.Init(skill, runnignData, OnFinish);
            runtimeEvents.Add(re);
        }
        public static void Update(float deltaTime)
        {
            for (int i = 0; i < runtimeEvents.Count; i++)
            {
                runtimeEvents[i].Breath(deltaTime);
                if (!runtimeEvents[i].isRunning)
                {
                    runtimeEvents.RemoveAt(i);
                    i--;
                }
            }
        }

        public static void FixedUpdate()
        {
            for (int i = 0; i < runtimeEvents.Count; i++)
            {
                runtimeEvents[i].FixedBreath();
            }
        }

        private static void OnFinish()
        {
            
        }
    }

    public class RuntimeEvent : RuntimeTimeLineGroup
    {
        
    }
}
