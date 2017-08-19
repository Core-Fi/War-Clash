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
        public SceneManager sceneManager;
        public LockFrameMgr lockFrameMgr;
        public EventGroup eventGroup;
        public enum LogicCoreEvent
        {
        }
        public void Init()
        {
            lockFrameMgr = new LockFrameMgr();
            sceneManager = new SceneManager();
            eventGroup = new EventGroup();
        }
        public void Update(float deltaTime)
        {
        }
        Writer w = new Writer();
        StringBuilder sb = new StringBuilder();
        private int fixedCount = 0;
        public int realFixedFrame = 0;
        public long TotalTime;
        private bool a;
        public void FixedUpdate()
        {
            fixedCount++;
            if (fixedCount % 4 != 0)
            {
                return;
            }
            sceneManager.Update();
            sceneManager.FixedUpdate();
            lockFrameMgr.Update();
            EventManager.Update(Time.deltaTime);
            realFixedFrame++;
            TotalTime += FixedMath.One/(15);
            if (!a && TotalTime > FixedMath.One * 3)
            {
                var b2 = LogicCore.SP.sceneManager.currentScene.CreateSceneObject<BarackBuilding>();
                b2.Team = Team.Team2;
                b2.Position = new Lockstep.Vector3d(new Vector3(10, 0, 0));
                a = true;
            }
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
