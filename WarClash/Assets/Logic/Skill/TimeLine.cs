using System;
using System.Collections.Generic;
using System.Text;
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
        public string path { get; private set; }

        public EventTrigger()
        {
            path = "";
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
        public List<BaseAction> BaseActions { get; private set; }
        [Display("监听事件")]
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

    public class RuntimeTimeLine
    {
        public TimeLineStatus m_TimeLineStatus;
        public TimeLine timeLine { get; private set; }
        public int m_Duration { get; private set; }
        public float m_Times { get; private set; }
        private SkillRunningData m_RunningData;
        private int m_curActionIndex;
        public virtual void Init(TimeLine tl, SkillRunningData runningData)
        {
            timeLine = tl;
            m_Duration = 0;
            m_Times = 0;
            this.m_RunningData = runningData;
            m_TimeLineStatus = TimeLineStatus.NotStarted;
            m_curActionIndex = 0;
        }

        public void Enter()
        {
            m_TimeLineStatus = TimeLineStatus.Running;
        }

        public float Breath(float deltaTime)
        {
            this.OnBreath(deltaTime);
            for (int i = m_curActionIndex; i < timeLine.BaseActions.Count; i++)
            {
                if (timeLine.BaseActions[i].ExecuteFrameIndex <= m_Duration)
                {
                    timeLine.BaseActions[i].Execute(m_RunningData.sender, m_RunningData.receiver, m_RunningData.data);
                    m_curActionIndex++;
                }
            }
            m_Duration ++;
            if (m_Duration >= timeLine.FrameCount)
            {
                m_Times++;
                deltaTime = m_Duration - timeLine.FrameCount;
                m_Duration = 0;
                m_curActionIndex = 0;
                if (m_Times >= timeLine.Times)
                {
                    Finish();
                    return deltaTime;
                }
                else
                {
                    OnTimeLineEnd();
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

        public void OnTimeLineEnd()
        {
             for (int i = 0; i < timeLine.BaseActions.Count; i++)
            {
                var action = timeLine.BaseActions[i] as DisplayAction;
                if (action != null)
                {
                    Character so = null;
                    if (action.playTarget == PlayTarget.SENDER)
                    {
                        so = this.m_RunningData.sender as Character;
                    }
                    else if (action.playTarget == PlayTarget.RECEIVER)
                    {
                        so = this.m_RunningData.receiver as Character;
                    }
                    so.EventGroup.FireEvent((int)Character.CharacterEvent.STOPDISPLAYACTION, so, EventGroup.NewArg<EventSingleArgs<DisplayAction>, DisplayAction>(action as DisplayAction));
                }
            }
        }
        public void Finish()
        {
            m_TimeLineStatus = TimeLineStatus.Finished;
            OnFinish();
        }
        public virtual void OnFinish()
        {
           OnTimeLineEnd();
        }
    }
    public enum TimeLineStatus
    {
        NotStarted,
        Running,
        Finished
    }
}
