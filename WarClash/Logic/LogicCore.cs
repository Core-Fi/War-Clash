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
        public Scene current_scene;
        public void Init()
        {
            current_scene = new Scene();
            current_scene.Init();
            current_scene.ListenEvents();
            //so = new Character();
            //so.Init();
        }
        public void Update(float deltaTime)
        {
            current_scene.Update(deltaTime);
            EventManager.Update(deltaTime);
        }
    }
}
