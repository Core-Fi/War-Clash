using Lockstep;
using Logic.LogicObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Logic.Components
{
    public class TransformComponent : SceneObjectBaseComponent
    {
        public enum Event {
            OnPositionChange
        }
        public string ResPath;
        public Vector3d Position
        {
            get { return _position; }
            set
            {
                if (_position != value)
                {
                    //GridService.UnTagAsTaken(_position);
                    //GridService.TagAsTaken(value);
                    _position = value;
                    if (EventGroup != null)
                        EventGroup.FireEvent((int)Event.OnPositionChange, this, null);
                }
            }
        }
        private Vector3d _position = new Vector3d(UnityEngine.Vector3.zero);
        public Vector3d Forward
        {
            get { return _forward; }
            internal set
            {
                _forward = value;
            }
        }
        private Vector3d _forward = new Vector3d(UnityEngine.Vector3.forward);
    }
}
