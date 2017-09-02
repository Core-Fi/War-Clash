using System;
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
       
        public enum LogicCoreEvent
        {
            OnJoystickStart,
            OnJoystickMove,
            OnJoystickEnd
            
        }
        public void Init()
        {
            LockFrameMgr = new LockFrameMgr();
            SceneManager = new SceneManager();
            EventGroup = new EventGroup();
            timeStep = 1f/LockFrameMgr.FixedFrameRate;
            Debug.LogError(timeStep);
        }
        public void Update(float deltaTime)
        {
        }

        public bool WriteToLog = true;
        public float fixedtime = 0;
        public int RealFixedFrame = 0;
        public long TotalTime;
        private float timeStep;
        public StringBuilder Writer = new StringBuilder();
        StringBuilder sb = new StringBuilder();
        public void FixedUpdate()
        {
            fixedtime += Time.fixedDeltaTime;
            if (fixedtime < timeStep)
            {
                return;
            }
            fixedtime = fixedtime - timeStep;
            SceneManager.Update();
            SceneManager.FixedUpdate();
            LockFrameMgr.FixedUpdate();
            EventManager.Update(Time.deltaTime);
            RealFixedFrame++;
            TotalTime += FixedMath.One/LockFrameMgr.FixedFrameRate;
           
            //sceneManager.currentScene.ForEachDo((c)=> {
            //    w.Write(c.GetStatusStr());
            //    sb.Append(c.GetStatusStr()+"\n\r");
            //});
            //if(fixedCount == 600)
            //{
            //    var bytes = w.Canvas.ToArray();
            //    File.WriteAllBytes(Application.dataPath+"/data1.bytes", bytes);
            //    File.WriteAllText(Application.dataPath + "/data1_str.txt", sb.ToString());
            //}

        }
    }
}

