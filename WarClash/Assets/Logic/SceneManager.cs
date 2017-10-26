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
        public Scene CurrentScene;
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
            if(CurrentScene!=null)
                CurrentScene.Destroy();
            CurrentScene = scene;
            CurrentScene.Init();
            var arg = EventGroup.NewArg<EventSingleArgs<Scene>>();
            arg.value = scene;
            EventGroup.FireEvent((int)SceneManagerEvent.OnSwitchScene, this, arg);
        }
        public void Update()
        {
            CurrentScene.Update(Time.deltaTime);
        }

        public void FixedUpdate()
        {
            CurrentScene.FixedUpdate(FixedMath.One/15);
        }
    }
}
