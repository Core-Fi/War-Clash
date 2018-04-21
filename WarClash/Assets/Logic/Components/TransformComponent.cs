using Lockstep;
using Logic.LogicObject;
using Newtonsoft.Json;
using System;
using System.Text;
using UnityEngine;

namespace Logic.Components
{
    public class TransformComponent : SceneObjectBaseComponent
    {
        public enum Event {
            OnPositionChange,
            OnHitGround
        }
        [JsonProperty]
        public string ResPath;
        public Vector3d Position
        {
            get { return _position; }
            set
            {
                if (_position != value)
                {
                    //bool hitx, hity;
                    //var p = scene.MapConfig.GetProperPosi(_position, value, out hitx, out hity);
                    //if (hity)
                    //{
                    //    Velocity.y = 0;
                    //    EventGroup.FireEvent((int)Event.OnHitGround, this, null);
                    //}
                    //if (hitx)
                    //{
                    //    Velocity.x = 0;
                    //}
                    var oldPosi = _position;
                    _position = value;
                    if (EventGroup != null)
                        EventGroup.FireEvent((int)Event.OnPositionChange, this, EventGroup.NewArg<EventTwoArgs<Vector3d, Vector3d>, Vector3d, Vector3d>(_position, oldPosi));
                }
            }
        }
        [JsonProperty]
        private Vector3d _position = new Vector3d(UnityEngine.Vector3.zero);
        public Vector3d Forward
        {
            get { return _forward; }
            internal set
            {
                _forward = value;
            }
        }
        [JsonProperty]
        private Vector3d _forward = new Vector3d(UnityEngine.Vector3.forward);
        public  Vector3d Velocity;
        private BattleScene scene;
        public override void OnAdd()
        {
            base.OnAdd();
            scene = (LogicCore.SP.SceneManager.CurrentScene as BattleScene);
        }
        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();
            Position += Velocity * LockFrameMgr.FixedFrameTime;
        }

    }
}
