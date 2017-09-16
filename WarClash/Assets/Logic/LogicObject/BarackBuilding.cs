using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Config;
using Lockstep;
using Logic.Config;

namespace Logic.LogicObject
{
    class BarackBuilding : Building
    {
        private long _timeout;
        private long _curTime;
        private int count;
        internal override void OnInit(CreateInfo createInfo)
        {
            base.OnInit(createInfo);
            _timeout = FixedMath.One*3;
        }
        
        internal override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);
        }

        internal override void OnFixedUpdate(long deltaTime)
        {
            _curTime += FixedMath.One/15;
            if (_curTime > _timeout)
            {
                _curTime = 0;
                var npc1 = LogicCore.SP.SceneManager.currentScene.CreateSceneObject<Npc>(new NpcCreateInfo
                {
                    Position = Position,
                    NpcId = Conf.ArmyId,
                    Team = Team
                });
                count ++;
            }
            base.OnFixedUpdate(deltaTime);
        }
    }
}