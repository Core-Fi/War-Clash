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
        public EventGroup eventGroup;
        public void Init()
        {
            sceneManager = new SceneManager();
            eventGroup = new EventGroup();
        }
        public void Update(float deltaTime)
        {
            sceneManager.Update();
            EventManager.Update(deltaTime);
        }
    }
}
