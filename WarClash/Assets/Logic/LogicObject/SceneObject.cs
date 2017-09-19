using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Logic.Skill;
using UnityEngine;
using Lockstep;

namespace Logic.LogicObject
{
    public enum Team
    {
        Team1,
        Team2,
        Team3,
        Team4,
        Neutral
    }

    public class CreateInfo : IPool
    {
        public int Id;
        public Vector3d Position;
        public Vector3d Forward;
        public Team Team;

        public virtual void Reset()
        {
            Position = new Vector3d();
            Forward = new Vector3d();
            Team = Team.Neutral;
            Id = 0;
        }
    }

    public class NpcCreateInfo : CreateInfo
    {
        public int NpcId;
    }
    public class BuildingCreateInfo : CreateInfo
    {
        public int BuildingId;
    }
    public abstract class SceneObject: IUpdate, IFixedUpdate
    {
        public enum SceneObjectEvent
        {
            Positionchange,
            Onattributechange
        }
        public int Id;
        public Team Team;
        public EventGroup EventGroup { get; private set; }
        public Vector3d Position
        {
            get { return _position; }
            internal set
            {
                if (_position != value)
                {
                    _position = value;
                    if(EventGroup!=null)
                        EventGroup.FireEvent((int)SceneObjectEvent.Positionchange, this, null);
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
        public AttributeManager AttributeManager { get; private set; }
        public long Hp
        {
            get
            {
                return AttributeManager[AttributeType.Hp];
            }
            set
            {
                if (value <= 0)
                {
                    Dispose();
                    AttributeManager.SetBase(AttributeType.Hp, 0);
                }
                else
                    AttributeManager.SetBase(AttributeType.Hp, value);
            }
        }

        internal virtual void ListenEvents()
        {

        }
        internal void Init(CreateInfo createInfo)
        {
            EventGroup = new EventGroup();
            AttributeManager = new AttributeManager();
            AttributeManager.New(AttributeType.Hp, Lockstep.FixedMath.One * 100);
            AttributeManager.OnAttributeChange += OnAttributeChange;
            Position = createInfo.Position;
            Team = createInfo.Team;
            if(createInfo.Forward.sqrMagnitude == 0)
                createInfo.Forward = new Vector3d(Vector3.forward);
            Forward = createInfo.Forward;
            OnInit(createInfo);
            Pool.SP.Recycle(createInfo);
            createInfo = null;
        }
        public virtual void OnAttributeChange(AttributeType at, long old, long newValue)
        {
            EventGroup.FireEvent((int)SceneObjectEvent.Onattributechange, this, EventGroup.NewArg<EventSingleArgs<AttributeMsg>, AttributeMsg>(new AttributeMsg()
            {
                At = at,
                NewValue = newValue,
                OldValue = old
            }));
        }
        public long GetAttributeValue(AttributeType at)
        {
            return AttributeManager[at];
        }
        internal virtual void OnInit(CreateInfo createInfo)
        {
            
        }
        public void Update(float deltaTime)
        {
            OnUpdate(deltaTime);
        }

        public void FixedUpdate(long deltaTime)
        {
            OnFixedUpdate(deltaTime);
        }

        internal virtual void OnFixedUpdate(long deltaTime)
        {
            
        }
        internal virtual void OnUpdate(float deltaTime)
        {

        }

        internal void Dispose()
        {
            
        }
        internal virtual void OnDispose()
        {
            
        }

        public override string ToString()
        {
            return this.GetType()+"  "+Id;
        }
        public virtual int GetStatusHash()
        {
            return GetStatusStr().GetHashCode();
        }

        public virtual string GetStatusStr()
        {
            StringBuilder sb = new StringBuilder();
            AppendVector3d(sb, Position);
            AppendVector3d(sb, Forward);
            sb.Append(AttributeManager.ToString());
            return sb.ToString();
        }

        private void AppendVector3d(StringBuilder sb, Vector3d v)
        {
            sb.Append(v.x);
            sb.Append(v.y);
            sb.Append(v.z);
        }
    }
}
