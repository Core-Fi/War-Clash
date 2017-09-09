using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lockstep;
using Logic;
using Logic.LogicObject;
using UnityEngine;

namespace Logic.LogicObject
{
    class Player : Character
    {
        public StateMachine StateMachine { get; private set; }

        internal override void OnInit()
        {
            base.OnInit();
            StateMachine = new StateMachine(this);
        }

        internal override void ListenEvents()
        {
            base.ListenEvents();
        }

        internal override void OnFixedUpdate(long deltaTime)
        {
            base.OnFixedUpdate(deltaTime);
        }

        internal override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);
        }
    }
}