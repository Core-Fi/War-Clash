using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Logic.Skill;
using UnityEngine;
using Lockstep;

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
        public Vector3d Position
        {
            get { return _position; }
            internal set
            {
                _position = value;
            }
        }
        private Vector3d _position = new Vector3d(Vector3.zero);
        internal virtual void ListenEvents()
        {

        }
        internal void Init()
        {
            EventGroup = new EventGroup();
            OnInit();
        }
        internal virtual void OnInit()
        {
            
        }
        internal void Update(float deltaTime)
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
