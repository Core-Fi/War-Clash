using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Brainiac;
using UnityEngine;

namespace Logic.LogicObject
{
    public class Npc : Character
    {
      
        internal override void OnInit()
        {
            base.OnInit();
            BTAsset btAsset = Resources.Load("Test") as BTAsset;
            AiAgent = new AIAgent(this, btAsset);
            AiAgent.Start();
        }
        internal override void ListenEvents()
        {
            base.ListenEvents();
        }

        internal override void OnFixedUpdate(long deltaTime)
        {
            base.OnFixedUpdate(deltaTime);
            if (!IsDeath())
            {
                AiAgent.Tick();
            }
        }

        internal override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);
          
        }
    }
}
