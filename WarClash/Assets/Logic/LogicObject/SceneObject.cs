using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Logic.Skill;
using UnityEngine;
using Lockstep;
using Logic.Components;

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

    public class SceneObject: IUpdate, IFixedUpdate, IFixedAgent
    {
        public long Radius;
        private List<BaseComponent> _components = new List<BaseComponent>();
        public T AddComponent<T>() where T : BaseComponent
        {
            for (int i = 0; i < _components.Count; i++)
            {
                if (_components[i] is T)
                    return null;
            }
            T t = Activator.CreateInstance<T>();
            t.SceneObject = this;
            t.OnAdd();
            _components.Add(t);
            return t;
        }
        public void RemoveComponent<T>() where T :  BaseComponent
        {
            for (int i = 0; i < _components.Count; i++)
            {
                if (_components[i] is T)
                {
                    _components[i].OnRemove();
                    _components.RemoveAt(i);
                }
            }
        }
        public bool HasComponent<T>() where T : BaseComponent
        {
            for (int i = 0; i < _components.Count; i++)
            {
                if (_components[i] is T)
                {
                    return true;
                }
            }
            return false;
        }
        public List<BaseComponent> GetAllComponents()
        {
            return _components;
        }
        public T GetComponent<T>() where T :  BaseComponent
        {
            for (int i = 0; i < _components.Count; i++)
            {
                if (_components[i] is T)
                {
                    return (T)_components[i];
                }
            }
            return null;
        }
        public enum SceneObjectEvent
        {
            Executedisplayaction,
            Stopdisplayaction,
            OnDispose
        }
        public int Id;
        public Team Team;
        public EventGroup EventGroup { get; private set; }
        public Vector3d Position
        {
            get { return TransformComp.Position; }
            set
            {
                TransformComp.Position = value;
            }
        }
        private Vector3d _position = new Vector3d(UnityEngine.Vector3.zero);
        public Vector3d Forward
        {
            get { return TransformComp.Forward; }
            set
            {
                TransformComp.Forward = value;
            }
        }
        private Vector3d _forward = new Vector3d(UnityEngine.Vector3.forward);
        private TransformComponent _transformComp;
        public TransformComponent TransformComp
        {
            get
            {
                if (_transformComp == null)
                    _transformComp = GetComponent<TransformComponent>();
                return _transformComp;
            }
        }
        public AttributeManager AttributeManager { get {
                if (_attributeManager == null)
                    _attributeManager = GetComponent<AttributeManager>();
                return _attributeManager;
            } }
        private AttributeManager _attributeManager;
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

        public IFixedAgent Next
        {
            get;set;
        }

        long IFixedAgent.Radius
        {
            get
            {
                return FixedMath.One;
            }
        }

        internal virtual void ListenEvents()
        {

        }
        internal void Init(CreateInfo createInfo)
        {
            EventGroup = new EventGroup();
            Position = createInfo.Position;
            Team = createInfo.Team;
            if(createInfo.Forward.sqrMagnitude == 0)
                createInfo.Forward = new Vector3d(Vector3.forward);
            Forward = createInfo.Forward;
            OnInit(createInfo);
            Pool.SP.Recycle(createInfo);
            createInfo = null;
        }
        public long GetAttributeValue(AttributeType at)
        {
            return AttributeManager[at];
        }

        public bool GetStatus(AttributeType at)
        {
            if (AttributeManager[at] == 1)
            {
                return true;
            }
            else return false;
        }
        internal virtual void OnInit(CreateInfo createInfo)
        {
            
        }
        public void Update(float deltaTime)
        {
            OnUpdate(deltaTime);
            for (int i = 0; i < _components.Count; i++)
            {
                _components[i].OnUpdate();
            }
        }

        public void FixedUpdate(long deltaTime)
        {
            OnFixedUpdate(deltaTime);
            for (int i = 0; i < _components.Count; i++)
            {
                _components[i].OnFixedUpdate();
            }
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
