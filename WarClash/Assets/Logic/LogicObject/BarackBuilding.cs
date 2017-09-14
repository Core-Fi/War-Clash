using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lockstep;

namespace Logic.LogicObject
{
    class BarackBuilding : Building
    {
        private long _timeout;
        private long _curTime;
        private int count;
        internal override void OnInit()
        {
            base.OnInit();
            _timeout = FixedMath.One/2;
        }
        
        internal override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);
        }

        internal override void OnFixedUpdate(long deltaTime)
        {
            _curTime += deltaTime;
            if (_curTime > _timeout && count<1)
            {
                _curTime = 0;
                var npc1 = LogicCore.SP.SceneManager.currentScene.CreateSceneObject<Npc>();
                npc1.Team = Team.Team2;
                npc1.Position = Position;
                count ++;
            }
            base.OnFixedUpdate(deltaTime);
        }
    }
}