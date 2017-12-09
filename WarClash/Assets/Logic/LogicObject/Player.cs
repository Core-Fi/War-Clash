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
    public class Player : Character
    {
        public enum  PlayerEvent
        {
            GuiseStart,
            GuiseEnd,
        }
        public StateMachine StateMachine { get; private set; }

        internal override void OnInit(CreateInfo createInfo)
        {
            base.OnInit(createInfo);
            StateMachine = new StateMachine(this);
            AttributeManager.New(AttributeType.IsVisible, 1);
            AttributeManager.New(AttributeType.IsMovable, 1);
        }

        internal override void ListenEvents()
        {
            base.ListenEvents();
        }

        internal override void OnFixedUpdate(long deltaTime)
        {
            base.OnFixedUpdate(deltaTime);
            StateMachine.FixedUpdate();
        }

        internal override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);
        }
    }
}