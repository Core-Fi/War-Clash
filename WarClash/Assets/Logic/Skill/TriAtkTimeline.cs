using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Logic.Skill
{
    [Display("三连击时间轴")]
    [Serializable]
    public class TriAtkTimeline : TimeLine
    {
        [Display("第一次触发帧数")]
        [Newtonsoft.Json.JsonProperty]
        public int FirstTriggerFrame { get; private set; }
        [Display("第一次触发事件")]
        [Newtonsoft.Json.JsonProperty]
        public int FirstTriggerId { get; private set; }
        [Display("第二次触发帧数")]
        [Newtonsoft.Json.JsonProperty]
        public int SecondTriggerFrame { get; private set; }
        [Display("第二次触发事件")]
        [Newtonsoft.Json.JsonProperty]
        public int SecondTriggerId { get; private set; }
        [Display("第三次触发帧数")]
        [Newtonsoft.Json.JsonProperty]
        public int ThirdTriggerFrame { get; private set; }
        [Display("第三次触发事件")]
        [Newtonsoft.Json.JsonProperty]
        public int ThirdTriggerId { get; private set; }
    }

    public class TriAtkRuntimeTimeline : RuntimeTimeLine
    {
        private int _curFrameCount = 0;
        private int _curTriggerCount = 0;
        private int _preTriggerFrame = 0;
        private TriAtkTimeline _triAtkTimeline;
        protected override void OnEnter()
        {
            base.OnEnter();
            _triAtkTimeline = SourceData as TriAtkTimeline;
            _curFrameCount = 0;
            _curTriggerCount = 0;
            EventDispatcher.ListenEvent(UIEventList.OnSkillBtnClick.ToInt(), OnBtnClick);
        }

        private void OnBtnClick(object sender, EventMsg e)
        {
            if (_curTriggerCount > 3 || _preTriggerFrame+20 > LogicCore.SP.LockFrameMgr.LocalFrameCount) return;
            if (_curFrameCount < _triAtkTimeline.FirstTriggerFrame)
            {
                EventManager.TriggerEvent(_triAtkTimeline.FirstTriggerId, new RuntimeData(base.m_RunningData.sender, m_RunningData.receiver, m_RunningData.data));
            }
            else if(_curFrameCount < _triAtkTimeline.SecondTriggerFrame)
            {
                EventManager.TriggerEvent(_triAtkTimeline.SecondTriggerId, new RuntimeData(base.m_RunningData.sender, m_RunningData.receiver, m_RunningData.data));
            }
            else
            {
                EventManager.TriggerEvent(_triAtkTimeline.ThirdTriggerId, new RuntimeData(base.m_RunningData.sender, m_RunningData.receiver, m_RunningData.data));
            }
            _curTriggerCount++;
            _preTriggerFrame = LogicCore.SP.LockFrameMgr.LocalFrameCount;
        }

        public override void OnFinish()
        {
            EventDispatcher.DelEvent(UIEventList.OnSkillBtnClick.ToInt(), OnBtnClick);
            base.OnFinish();
        }

        protected override void OnFixedBreath()
        {
            _curFrameCount++;
            base.OnFixedBreath();
        }
    }
}
