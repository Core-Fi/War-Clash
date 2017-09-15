using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Logic.LogicObject;
using Logic.Skill;
using Lockstep;
using System.IO;
using Config;
using Logic.Config;
using UnityEngine;

namespace Logic
{
    public class LogicCore : Singleton<LogicCore>
    {
        public SceneManager SceneManager;
        public LockFrameMgr LockFrameMgr;
        public EventGroup EventGroup;
        public bool WriteToLog = true;
        public StringBuilder Writer = new StringBuilder();
        public int RegularFrameCount;
        private float _fixedtime = 0;
        private float _timeStep;

        public enum LogicCoreEvent
        {
            OnJoystickStart,
            OnJoystickMove,
            OnJoystickEnd
        }
        public void Init()
        {
            ConfigMap<BuildingConf_ARRAY>.LoadBuildingConf();
            LockFrameMgr = new LockFrameMgr();
            SceneManager = new SceneManager();
            EventGroup = new EventGroup();
            _timeStep = 1f/LockFrameMgr.FixedFrameRate;
        }
        public void Update(float deltaTime)
        {
            SceneManager.Update();
           // EventManager.Update(Time.deltaTime);
        }
    
        public void FixedUpdate()
        {
            _fixedtime += Time.fixedDeltaTime;
            if (_fixedtime < _timeStep)
            {
                return;
            }
            _fixedtime = _fixedtime - _timeStep;
            RegularFrameCount++;
            LockFrameMgr.FixedUpdate();
        }
    }
}

