﻿using System;
using System.Collections.Generic;
using System.Text;
using FastCollections;
using Logic.LogicObject;

using Logic.Skill.Actions;

namespace Logic.Skill
{

    [Serializable]
    public enum EventType
    {
        MISSIONHIT,
        MISSIONDIE, 
        MEELEEWEAPONHIT = 1000,
        TEST
    }
    [Display("事件")]
    public class EventTrigger
    {
        [Display("事件类型")]
        public EventType e { get; private set; }
        [Display("事件效果路径")]
        public int EventId { get; private set; }
        public EventTrigger()
        {
        }
    }
    [Display("时间轴")]
    [Serializable]
    public class TimeLine
    {
        [Display("循环次数")]
        [Newtonsoft.Json.JsonProperty]
        public int Times { get; private set; }
        [Display("持续帧数")]
        [Newtonsoft.Json.JsonProperty]
        public int FrameCount { get; private set; }
        [Newtonsoft.Json.JsonProperty]
        public List<BaseAction> BaseActions { get; private set; }
        [Display("监听事件")]
        [Newtonsoft.Json.JsonProperty]
        public List<EventTrigger> eventTriggers { get; private set; }

#if UNITY_EDITOR
       // public Rect scrollPosiInEditor;
#endif
        public TimeLine()
        {
            BaseActions = new List<BaseAction>();
            eventTriggers = new List<EventTrigger>();
            Times = 1;
        }
      
    }

    public class RuntimeTimeLine : IPool
    {
        public static Dictionary<Type, Type> TimelineDataAndLogic = new BiDictionary<Type, Type>
        {
            {typeof(TimeLine), typeof(RuntimeTimeLine)},
            {typeof(TriAtkTimeline), typeof(TriAtkRuntimeTimeline)},
        };
        public TimeLineStatus m_TimeLineStatus;
        public TimeLine SourceData { get; private set; }
        public float m_Duration { get; private set; }
        public float m_Times { get; private set; }
        protected RuntimeData m_RunningData;
        private int m_curActionIndex;
        public void Init(TimeLine tl, RuntimeData runningData)
        {
            SourceData = tl;
            m_Duration = 0;
            m_Times = 0;
            this.m_RunningData = runningData;
            m_TimeLineStatus = TimeLineStatus.NotStarted;
            m_curActionIndex = 0;
        }

        protected virtual void OnEnter()
        {
            
        }
        public void Enter()
        {
            m_TimeLineStatus = TimeLineStatus.Running;
            OnEnter();
        }
        public void FixedBreath()
        {
            this.OnFixedBreath();
            for (int i = m_curActionIndex; i < SourceData.BaseActions.Count; i++)
            {
                if (SourceData.BaseActions[i].ExecuteFrameIndex <= m_Duration)
                {
                    SourceData.BaseActions[i].Execute(m_RunningData.sender, m_RunningData.receiver, m_RunningData.data);
                    m_curActionIndex++;
                }
            }
            m_Duration++;
            if (m_Duration >= SourceData.FrameCount)
            {
                m_Times++;
                m_Duration = 0;
                m_curActionIndex = 0;
                if (m_Times >= SourceData.Times)
                {
                    Finish();
                }
                else
                {
                    return;
                }
            }
            else
            {
                return;
            }
        }

        protected virtual void OnFixedBreath()
        {

        }

        public float Breath(float deltaTime)
        {
            this.OnBreath(deltaTime);
            for (int i = m_curActionIndex; i < SourceData.BaseActions.Count; i++)
            {
                if (SourceData.BaseActions[i].ExecuteFrameIndex <= m_Duration)
                {
                    SourceData.BaseActions[i].Execute(m_RunningData.sender, m_RunningData.receiver, m_RunningData.data);
                    m_curActionIndex++;
                }
            }
            m_Duration+=deltaTime;
            if (m_Duration >= SourceData.FrameCount)
            {
                m_Times++;
                deltaTime = m_Duration - SourceData.FrameCount;
                m_Duration = 0;
                m_curActionIndex = 0;
                if (m_Times >= SourceData.Times)
                {
                    Finish();
                    return deltaTime;
                }
                else
                {
                    if (deltaTime > 0)
                        return Breath(deltaTime);
                    else
                    {
                        return 0;
                    }
                }
            }
            else
            {
                return 0;
            }
        }

        public virtual void OnBreath(float deltaTime)
        {

        }

        public void Finish()
        {
            m_TimeLineStatus = TimeLineStatus.Finished;
            OnFinish();
        }
        public virtual void OnFinish()
        {
            for (int i = 0; i < SourceData.BaseActions.Count; i++)
            {
                SourceData.BaseActions[i].OnTimeLineFinish(m_RunningData.sender, m_RunningData.receiver, m_RunningData.data);
            }
        }

        public void Reset()
        {
            SourceData = null;
            m_Duration = 0;
            m_Times = 0;
            this.m_RunningData = default(RuntimeData);
            m_TimeLineStatus = TimeLineStatus.NotStarted;
            m_curActionIndex = 0;
        }
    }
    public enum TimeLineStatus
    {
        NotStarted,
        Running,
        Finished
    }
}
