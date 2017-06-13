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
        public TcpConnect network;
        public LockFrameMgr lockFrameMgr;
        public EventGroup eventGroup;

        public void Init()
        {
            lockFrameMgr = new LockFrameMgr();
            network = new TcpConnect();
            sceneManager = new SceneManager();
            eventGroup = new EventGroup();
        }
        public void Update(float deltaTime)
        {
            sceneManager.Update();
            network.Update();
            EventManager.Update(deltaTime);
        }
        public void FixedUpdate()
        {
            lockFrameMgr.Update();
        }
    }
}
