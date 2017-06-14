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
            sceneManager.Update();
            EventManager.Update(deltaTime);
        }
        public void FixedUpdate()
        {
            lockFrameMgr.Update();
        }
    }
}
