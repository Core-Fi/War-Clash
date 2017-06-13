using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Logic.Skill;
using UnityEngine;

namespace Logic.LogicObject
{
    public abstract class SceneObject
    {
        public enum SceneObjectEvent
        {
            POSITIONCHANGE,
        }
        public int ID;
        public EventGroup EventGroup { get; private set; }
        public Vector3 Position
        {
            get { return _position; }
            set
            {
                _position = value;

                //this.EventGroup.FireEvent((int)SceneObjectEvent.POSITIONCHANGE, this, EventGroup.NewArg<EventSingleArgs<Vector3>, Vector3>(value));
            }
        }
        private Vector3 _position;
        internal void ListenEvents()
        {
            EventGroup = new EventGroup();
            OnListenEvents();
        }
        internal virtual void OnListenEvents()
        {

        }
        internal  void Init()
        {
           OnInit();
        }
        internal virtual void OnInit()
        {
            
        }
        internal  void Update(float deltaTime)
        {
            OnUpdate(deltaTime);
        }
        internal virtual void OnUpdate(float deltaTime)
        {

        }

        public override string ToString()
        {
            return this.GetType()+"  "+ID;
        }
    }
}
