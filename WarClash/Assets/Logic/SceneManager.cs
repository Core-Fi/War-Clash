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
        public IScene CurrentScene;
        public EventGroup EventGroup;
        public enum SceneManagerEvent
        {
            OnSwitchScene
        }
        public SceneManager()
        {
            EventGroup = new EventGroup();
        }
        public void SwitchScene(IScene battleScene)
        {
            if(CurrentScene!=null)
                CurrentScene.Destroy();
            CurrentScene = battleScene;
            CurrentScene.Init();
            var arg = EventGroup.NewArg<EventSingleArgs<IScene>>();
            arg.value = battleScene;
            EventGroup.FireEvent((int)SceneManagerEvent.OnSwitchScene, this, arg);
        }
        public void Update()
        {
            if (CurrentScene is BattleScene)
            {
                var bs = (BattleScene)CurrentScene ;
                bs.Update(Time.deltaTime);
            }
            else
            {
                CurrentScene.OnUpdate(Time.deltaTime);
            }
        }

        public void FixedUpdate()
        {
            if (CurrentScene is BattleScene)
            {
                var bs = (BattleScene)CurrentScene;
                bs.FixedUpdate(FixedMath.One / 15);
            }
            else
            {
                CurrentScene.OnFixedUpdate(FixedMath.One / 15);
            }
        }
    }
}
