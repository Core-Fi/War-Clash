using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lockstep;
using Logic.Skill;
using UnityEngine;

namespace Logic.LogicObject
{
    class Tower : Building
    {
        private long _previousTime;
        private long _timeout = FixedMath.One;
        public SkillManager SkillManager { get; private set; }
        internal override void OnInit(CreateInfo createInfo)
        {
            base.OnInit(createInfo);
            SkillManager = new SkillManager(this);
        }
        internal override void ListenEvents()
        {
            base.ListenEvents();
        }

        internal override void OnFixedUpdate(long deltaTime)
        {
            if (LogicCore.SP.LockFrameMgr.LocalFrameCount >(int)((_previousTime + _timeout) / LockFrameMgr.FixedFrameTime))
            {
                _previousTime = LogicCore.SP.LockFrameMgr.LocalFrameCount * LockFrameMgr.FixedFrameTime;
                SkillManager.ReleaseSkill(3);
            }
            SkillManager.FixedUpdate();
            base.OnFixedUpdate(deltaTime);
        }

        
    }
}
