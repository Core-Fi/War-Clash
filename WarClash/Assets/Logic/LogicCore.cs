using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Logic.LogicObject;
using Logic.Skill;

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
           // sceneManager.Update();
            EventManager.Update(deltaTime);
        }
        private int fixedCount = 0;
        public void FixedUpdate()
        {
            fixedCount++;
            if (fixedCount % 4 != 0)
            {
                return;
            }
            sceneManager.Update();
            lockFrameMgr.Update();
        }
    }
}
