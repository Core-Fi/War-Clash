using Logic.LogicObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Logic
{
    public class SceneManager
    {
        public Scene currentScene;
        public EventGroup eventGroup;
        public enum SceneManagerEvent
        {
            ONSWITCHSCENE
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
            eventGroup.FireEvent((int)SceneManagerEvent.ONSWITCHSCENE, this, EventGroup.NewArg<EventSingleArgs<Scene>, Scene>(scene));
        }
        public void Update()
        {

        }
    }
}
