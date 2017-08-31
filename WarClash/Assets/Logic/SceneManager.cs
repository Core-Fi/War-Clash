using Logic.LogicObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lockstep;
using UnityEngine;

namespace Logic
{
    public class SceneManager
    {
        public Scene currentScene;
        public EventGroup EventGroup;
        public enum SceneManagerEvent
        {
            OnSwitchScene
        }
        public SceneManager()
        {
            EventGroup = new EventGroup();
        }
        public void SwitchScene(Scene scene)
        {
            if(currentScene!=null)
                currentScene.Destroy();
            currentScene = scene;
            currentScene.Init();
            var arg = EventGroup.NewArg<EventSingleArgs<Scene>>();
            arg.value = scene;
            EventGroup.FireEvent((int)SceneManagerEvent.OnSwitchScene, this, arg);
        }
        public void Update()
        {
            currentScene.Update(Time.deltaTime);
        }

        public void FixedUpdate()
        {
            currentScene.FixedUpdate(FixedMath.One/15);
        }
    }
}
