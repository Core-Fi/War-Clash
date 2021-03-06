﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Logic.LogicObject;
using Logic.Skill;
using Lockstep;
using System.IO;
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
        private float _fixedtime = 0;
        private float _timeStep;

        public enum LogicCoreEvent
        {
            OnJoystickStart,
            OnJoystickMove,
            OnJoystickEnd
        }
        public void Write(object o)
        {
            if (WriteToLog)
            {
                Writer.AppendLine(o.ToString());
            }
        }
        public void Write(string str)
        {
            if (WriteToLog)
            {
                Writer.AppendLine(str);
            }
        }
        public void Init()
        {

            InitConfig();
            LockFrameMgr = new LockFrameMgr();
            SceneManager = new SceneManager();
            EventGroup = new EventGroup();
            _timeStep = 1f/LockFrameMgr.FixedFrameRate;
        }
        private void InitConfig()
        {
            ArmyConf.Init();
            BuildingConf.Init();
        }
        public void Update(float deltaTime)
        {
            SceneManager.Update();
            LockFrameMgr.FixedFrameRate = (int)(1f / deltaTime);
            LockFrameMgr.FixedFrameTime = deltaTime.ToLong();
            LockFrameMgr.FixedUpdate();
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
          //  LockFrameMgr.FixedUpdate();
        }
    }
}

