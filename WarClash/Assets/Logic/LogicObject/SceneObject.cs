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

    public sealed class SceneObject: IUpdate, IFixedUpdate, IFixedAgent, IEventDispatcher
    {
        public static SceneObject MainPlayer
        {
            get
            {
                var bs = LogicCore.SP.SceneManager.CurrentScene as BattleScene;
                return bs.GetObject<SceneObject>();
            }
        }
        private static SceneObject _mainPlayer;
        public long Radius;
        private List<SceneObjectBaseComponent> _components = new List<SceneObjectBaseComponent>();
        public T AddComponent<T>() where T : SceneObjectBaseComponent
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
            FireEvent((int)SceneObject.SceneObjectEvent.OnRemoveComponent, this, EventGroup.NewArg<EventSingleArgs<T>, T>(t));
            return t;
        }
        public void RemoveComponent<T>() where T :  SceneObjectBaseComponent
        {
            for (int i = 0; i < _components.Count; i++)
            {
                if (_components[i] is T)
                {
                    FireEvent((int)SceneObject.SceneObjectEvent.OnRemoveComponent, this, EventGroup.NewArg<EventSingleArgs<T>, T>((T)_components[i]));
                    _components[i].OnRemove();
                    _components.RemoveAt(i);
                }
            }
        }
        public bool HasComponent<T>() where T : SceneObjectBaseComponent
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
        public List<SceneObjectBaseComponent> GetAllComponents()
        {
            return _components;
        }
        public T GetComponent<T>() where T :  SceneObjectBaseComponent
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
            OnBeforeAttackOther,
            OnBeforeBeAttacked,
            OnAfterAttackOther,
            OnAfterBeAttacked,
            Executedisplayaction,
            Stopdisplayaction,
            OnAddComponent,
            OnRemoveComponent,
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
        internal void Init(CreateInfo createInfo)
        {
            EventGroup = new EventGroup();
            Position = createInfo.Position;
            Team = createInfo.Team;
            if(createInfo.Forward.sqrMagnitude == 0)
                createInfo.Forward = new Vector3d(Vector3.forward);
            Forward = createInfo.Forward;
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
        public void Update(float deltaTime)
        {
            //for (int i = 0; i < _components.Count; i++)
            //{
            //    _components[i].OnUpdate();
            //}
        }

        public void FixedUpdate(long deltaTime)
        {
            for (int i = 0; i < _components.Count; i++)
            {
                _components[i].OnFixedUpdate();
            }
        }


        public void Dispose()
        {
            for (int i = 0; i < _components.Count; i++)
            {
                _components[i].OnDispose();
            }
        }
        
        public void ListenEvent(int id, EventMsgHandler e)
        {
            EventGroup.ListenEvent(id, e);
        }

        public void DelEvent(int id, EventMsgHandler e)
        {
            EventGroup.DelEvent(id, e);
        }

        public void FireEvent(int id, object sender, EventMsg m)
        {
            EventGroup.FireEvent(id, sender, m);
        }
        public override string ToString()
        {
            return this.GetType()+"  "+Id;
        }
        public int GetStatusHash()
        {
            return GetStatusStr().GetHashCode();
        }

        public string GetStatusStr()
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
