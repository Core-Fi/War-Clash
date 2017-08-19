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
        public EventGroup eventGroup;
        public enum SceneManagerEvent
        {
            Onswitchscene
        }
        public SceneManager()
        {
            eventGroup = new EventGroup();
        }
        public void SwitchScene(Scene scene)
        {
            if(currentScene!=null)
                currentScene.Destroy();
            currentScene = scene;
            currentScene.Init();
            eventGroup.FireEvent((int)SceneManagerEvent.Onswitchscene, this, EventGroup.NewArg<EventSingleArgs<Scene>, Scene>(scene));
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
