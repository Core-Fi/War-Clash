using Lockstep;
using Newtonsoft.Json;
using System.Text;

namespace Logic.Components
{
    public class TransformComponent : SceneObjectBaseComponent
    {
        public enum Event {
            OnPositionChange
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
                    //GridService.UnTagAsTaken(_position);
                    //GridService.TagAsTaken(value);
                    _position = value;
                    if (EventGroup != null)
                        EventGroup.FireEvent((int)Event.OnPositionChange, this, null);
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
        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();
            Position += Velocity / 15;
        }

    }
}
