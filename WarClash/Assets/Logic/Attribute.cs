using Logic.Components;
using Logic.LogicObject;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Logic
{
    public enum AttributeType
    {
        Hp,
        Maxhp,
        MaxSpeed,
        Speed,
        CharacterEnd,
        IsMovable,
        IsVisible
    }

    public enum Operation
    {
        Absolute,
        Percent
    }

    public struct AttributeMotifier
    {
        public Operation Operation;
        public long Value;
    }

    public class CharacterAttribute
    {
        private readonly LinkedList<AttributeMotifier> _motifiers = new LinkedList<AttributeMotifier>();
        //   public AttributeType attribute { get; private set; }
        public CharacterAttribute(long value)
        {
            BaseValue = value;
            FinalValue = value;
        }

        public long BaseValue { get; private set; }
        public long FinalValue { get; private set; }

        public void SetBase(long value)
        {
            BaseValue = value;
            Caculate();
        }
        public void Add(AttributeMotifier am)
        {
            if (am.Operation == Operation.Percent)
            {
                _motifiers.AddFirst(am);
            }
            else
            {
                _motifiers.AddLast(am);
            }

            Caculate();
        }
        public void Add(Operation operation, long value)
        {
            var attr = new AttributeMotifier
            {
                Operation = operation,
                Value = value
            };
            if (operation == Operation.Percent)
            {
                _motifiers.AddFirst(attr);
            }
            else
            {
                _motifiers.AddLast(attr);
            }

            Caculate();
        }

        public void Remove(AttributeMotifier am)
        {
            if (_motifiers.Count == 0)
                return;
            var rst = _motifiers.Remove(am);
            if (rst)
            {
                Caculate();
            }
            else
            {
                Debug.LogError(am.Operation + " fail to remove");
            }
        }
        public void Remove(Operation operation, long value)
        {
            if (_motifiers.Count == 0)
                return;
            bool removed = false;
            var node = _motifiers.First;
            while (node!=null)
            {
                if (node.Value.Value == value && node.Value.Operation == operation)
                {
                    _motifiers.Remove(node);
                    removed = true;
                    break;
                }
                else
                {
                    node = node.Next;
                }
            }
            if (removed)
            {
                Caculate();
            }
            else
            {
                Debug.LogError(operation+" fail to remove");
            }
        }

        public void Caculate()
        {
            FinalValue = BaseValue;
            foreach (var item in _motifiers)
            {
                if (item.Operation == Operation.Percent)
                {
                    FinalValue *= item.Value;
                }
                else
                {
                    FinalValue += item.Value;
                }
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var item in _motifiers)
            {
                sb.Append(item.Operation);
                sb.Append(item.Value);
            }
            sb.Append(BaseValue);
            sb.Append(FinalValue);
            return sb.ToString();
        }
    }

    public struct AttributeMsg
    {
        public AttributeType At;
        public long OldValue;
        public long NewValue;
    }

    public class AttributeManager : SceneObjectBaseComponent
    {
        public enum Event
        {
            OnAttributechange
        }
        private readonly Dictionary<int, CharacterAttribute> _attributes = new Dictionary<int, CharacterAttribute>();
#if UNITY_EDITOR
        public Dictionary<int, CharacterAttribute> Attributes
        {
            get { return _attributes; }
        }
#endif
        public long this[AttributeType at]
        {
            get
            {
                CharacterAttribute attr;
                if (_attributes.TryGetValue((int) at, out attr))
                {
                    return attr.FinalValue;
                }
                Debug.LogError(at + " Key not exsit");
                return 0;
            }
        }
        public bool HasAttribute(AttributeType at)
        {
            return _attributes.ContainsKey((int) at);
        }

        public CharacterAttribute New(AttributeType at, long value)
        {
            _attributes[(int) at] = new CharacterAttribute(value);
            return _attributes[(int)at];
        }

        public void Remove(AttributeType at, AttributeMotifier am)
        {
            if (_attributes.ContainsKey((int)at))
            {
                var attr = _attributes[(int)at];
                var oldValue = attr.FinalValue;
                attr.Remove(am);
                EventGroup.FireEvent((int)Event.OnAttributechange, this, EventGroup.NewArg<EventSingleArgs<AttributeMsg>, AttributeMsg>(new AttributeMsg()
                {
                    At = at,
                    NewValue = attr.FinalValue,
                    OldValue = oldValue
                }));
            }
            else
            {
                Debug.LogError(at + " Key not exsit");
            }
        }
        public void Remove(AttributeType at, Operation op, long value)
        {
            if (_attributes.ContainsKey((int) at))
            {
                var attr = _attributes[(int) at];
                var oldValue = attr.FinalValue;
                attr.Remove(op, value);
                EventGroup.FireEvent((int)Event.OnAttributechange, this, EventGroup.NewArg<EventSingleArgs<AttributeMsg>, AttributeMsg>(new AttributeMsg()
                {
                    At = at,
                    NewValue = attr.FinalValue,
                    OldValue = oldValue
                }));
            }
            else
            {
                Debug.LogError(at + " Key not exsit");
            }
        }

        public void SetBase(AttributeType at, long value)
        {
            CharacterAttribute attr;
            if(_attributes.TryGetValue((int)at, out attr))
            {
                var oldValue = attr.FinalValue;
                _attributes[(int)at].SetBase(value);
                EventGroup.FireEvent((int)Event.OnAttributechange, this, EventGroup.NewArg<EventSingleArgs<AttributeMsg>, AttributeMsg>(new AttributeMsg()
                {
                    At = at,
                    NewValue = attr.FinalValue,
                    OldValue = oldValue
                }));
            }
            else
            {
                attr = New(at, value);
            }
        }

        public void Add(AttributeType at, AttributeMotifier am)
        {
            if (_attributes.ContainsKey((int)at))
            {
                var attr = _attributes[(int)at];
                var oldValue = attr.FinalValue;
                attr.Add(am);
                EventGroup.FireEvent((int)Event.OnAttributechange, this, EventGroup.NewArg<EventSingleArgs<AttributeMsg>, AttributeMsg>(new AttributeMsg()
                {
                    At = at,
                    NewValue = attr.FinalValue,
                    OldValue = oldValue
                }));
            }
            else
            {
                Debug.LogError(at + " Key not exsit");
            }
        }
        public void Add(AttributeType at, Operation op, long value)
        {
            if (_attributes.ContainsKey((int) at))
            {
                var attr = _attributes[(int) at];
                var oldValue = attr.FinalValue;
                attr.Add(op, value);
                EventGroup.FireEvent((int)Event.OnAttributechange, this, EventGroup.NewArg<EventSingleArgs<AttributeMsg>, AttributeMsg>(new AttributeMsg()
                {
                    At = at,
                    NewValue = attr.FinalValue,
                    OldValue = oldValue
                }));
            }
            else
            {
                Debug.LogError(at + " Key not exsit");
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(this.GetType());
            foreach (var attribute in _attributes)
            {
                sb.Append(attribute.Key);
                sb.Append(attribute.Value);
            }
            return sb.ToString();
        }
    }
}