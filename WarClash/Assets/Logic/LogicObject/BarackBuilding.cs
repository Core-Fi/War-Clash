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
            _timeout = FixedMath.One*5;
        }
        
        internal override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);
        }

        internal override void OnFixedUpdate(long deltaTime)
        {
            _curTime += FixedMath.One/15;
            if (_curTime > _timeout && count<1)
            {
                //_curTime = 0;
                //var createInfo = Pool.SP.Get<NpcCreateInfo>();
                //createInfo.Position = Position;
                //createInfo.NpcId = Conf.ArmyId;
                //createInfo.Team = Team;
                //var npc1 = LogicCore.SP.SceneManager.CurrentScene.CreateSceneObject<Npc>(createInfo);
                //count ++;
            }
            base.OnFixedUpdate(deltaTime);
        }
    }
}