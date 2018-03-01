using Logic.LogicObject;
using Logic.Skill.Actions;

using System;
using System.Collections.Generic;
using System.Text;

namespace Logic.Skill
{
    [Serializable]
    public class TimeLineGroup
    {
        [Display("名字")]
        [Newtonsoft.Json.JsonProperty]
        public string Name { get; private set; }

        [Display("ID")]
        [Newtonsoft.Json.JsonProperty]
        public int ID { get; private set; }
        public List<TimeLine> TimeLines { get; private set; }

        [Newtonsoft.Json.JsonProperty]
        public string path { get; private set; }
        public TimeLineGroup()
        {
            TimeLines = new List<TimeLine>();
        }

    }

    public class RuntimeTimeLineGroup : IPool
    {
        public TimeLineGroup SourceData;
        public RuntimeData m_RunningData;
        public List<RuntimeTimeLine> timelines = new List<RuntimeTimeLine>(); 

        public Action FinishAction;
        public Action BreakAction;
        public bool isRunning;
        private int m_TimeLineCount
        {
            get { return SourceData.TimeLines.Count; }
        }
        private int m_CurrentTLIndex;

        public void Init(TimeLineGroup skill, RuntimeData srd)
        {
            this.SourceData = skill;
            this.m_RunningData = srd;
            m_CurrentTLIndex = -1;
            isRunning = true;
            for (int i = 0; i < m_TimeLineCount; i++)
            {
                var rtl = Pool.SP.Get(RuntimeTimeLine.TimelineDataAndLogic[SourceData.TimeLines[i].GetType()]) as RuntimeTimeLine;
                timelines.Add(rtl);
                timelines[i].Init(SourceData.TimeLines[i], srd);
            }
        }
        public void Breath(float deltaTime)
        {
            if (m_CurrentTLIndex == -1)
            {
                EnterNextTimeLine();
            }
            var timeLine = timelines[m_CurrentTLIndex];
            if (timeLine.m_TimeLineStatus == TimeLineStatus.Finished)
            {
                if (m_CurrentTLIndex != m_TimeLineCount - 1)
                {
                    EnterNextTimeLine();
                    Breath(deltaTime);
                }
            }
            else
            {
                deltaTime = timeLine.Breath(deltaTime);
                if (m_CurrentTLIndex == m_TimeLineCount - 1 && timeLine.m_TimeLineStatus == TimeLineStatus.Finished)
                {
                    Finish();
                }
                else
                {
                    if (m_CurrentTLIndex < m_TimeLineCount - 1 && deltaTime > 0)
                    {
                        EnterNextTimeLine();
                        Breath(deltaTime);
                    }
                }
            }
        }
        internal void FixedBreath()
        {
            if (m_CurrentTLIndex == -1)
            {
                EnterNextTimeLine();
            }
            var timeLine = timelines[m_CurrentTLIndex];
            timeLine.FixedBreath();
            if (timeLine.m_TimeLineStatus == TimeLineStatus.Finished)
            {
                if (m_CurrentTLIndex == m_TimeLineCount - 1)
                {
                    Finish();
                }
                else
                {
                    EnterNextTimeLine();
                }
            }
        }

        public void Cancel()
        {
            for (int i = 0; i < timelines.Count; i++)
            {
                var tl = timelines[i];
                for (int j = 0; j < tl.SourceData.BaseActions.Count; j++)
                {
                    tl.SourceData.BaseActions[j].OnCancel(m_RunningData.sender, m_RunningData.receiver, m_RunningData.data);
                }
            }
            if (BreakAction != null)
            {
                BreakAction.Invoke();
            }
            isRunning = false;
            OnCancel();
            for (int i = 0; i < this.timelines.Count; i++)
            {
                Pool.SP.Recycle(timelines[i]);
            }
        }

        public virtual void OnCancel()
        {
          
        }
        public void Finish()
        {
            for (int i = 0; i < timelines.Count; i++)
            {
                var tl = timelines[i];
                for (int j = 0; j < tl.SourceData.BaseActions.Count; j++)
                {
                    tl.SourceData.BaseActions[j].OnSkillFinish(m_RunningData.sender, m_RunningData.receiver, m_RunningData.data);
                }
            }
            if (FinishAction != null)
            {
                FinishAction.Invoke();
            }
           
            isRunning = false;
            OnFinish();
            for (int i = 0; i < this.timelines.Count; i++)
            {
                Pool.SP.Recycle(timelines[i]);
            }
        }
        protected virtual void OnFinish()
        {
        }
        public void EnterNextTimeLine()
        {
            m_CurrentTLIndex++;
            timelines[m_CurrentTLIndex].Enter();
        }

        public void Reset()
        {
            SourceData = null;
            FinishAction = null;
            m_CurrentTLIndex = -1;
            isRunning = false;
            timelines.Clear();
        }
    }
}
