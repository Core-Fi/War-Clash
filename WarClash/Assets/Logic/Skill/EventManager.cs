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

            if (Application.isPlaying)
            {
                event_index = Logic.Skill.SkillUtility.LoadIndexFile("event_index.bytes");
            }
            else
            {
                event_index = Logic.Skill.SkillUtility.LoadIndexFile("Events/event_index.bytes");
            }

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
        public static void TriggerEvent(int id, RuntimeData runnignData)
        {
            if(id==0)
                return;
            var skill = GetEvent(id);
            RuntimeEvent re = new RuntimeEvent();
            re.Init(skill, runnignData);
            re.FinishAction = OnFinish;
            runtimeEvents.Add(re);
            DLog.Log("trigger event "+id);
        }
        public static void Update(float deltaTime)
        {
            for (int i = 0; i < runtimeEvents.Count; i++)
            {
                runtimeEvents[i].Breath(deltaTime);
                if (!runtimeEvents[i].isRunning)
                {
                    RemoveEvent(i);
                    i--;
                }
            }
        }

        public static void FixedUpdate()
        {
            for (int i = 0; i < runtimeEvents.Count; i++)
            {
                runtimeEvents[i].FixedBreath();
                if (!runtimeEvents[i].isRunning)
                {
                    RemoveEvent(i);
                    i--;
                }
            }
        }
        private static void RemoveEvent(int index)
        {
            runtimeEvents.RemoveAt(index);
        }

        public static void RemoveEvent(RuntimeEvent re)
        {
            runtimeEvents.Remove(re);
        }
        private static void OnFinish()
        {
            
        }
    }

    public class RuntimeEvent : RuntimeTimeLineGroup
    {
        
    }
}
