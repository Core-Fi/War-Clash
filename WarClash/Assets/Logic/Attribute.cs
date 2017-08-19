using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Logic
{
    public enum AttributeType
    { 
        HP ,
        MAXHP,
        MAXSPEED,
        SPEED ,
        CHARACTEREND,
    }
    public enum Operation
    { 
        ABSOLUTE,
        PERCENT
    }
    public struct AttributeMotifier
    {
        public Operation operation;
        public long value;
    }
    public class CharacterAttribute
    {
        public long BaseValue { get; private set; }
        private LinkedList<AttributeMotifier> _motifiers = new LinkedList<AttributeMotifier>();
        public long FinalValue { get; private set; }
     //   public AttributeType attribute { get; private set; }
        public CharacterAttribute(long value)
        {
            BaseValue = value;
            FinalValue = value;
        }
        public void SetBase(long value)
        {
            BaseValue = value;
            Caculate();
        }
        public void Add(Operation operation, long value)
        {
            var attr = new AttributeMotifier()
            {
                operation = operation,
                value = value
            };
            if(operation == Operation.PERCENT)
            {
                _motifiers.AddFirst(attr);
            }else
            {
                _motifiers.AddLast(attr);
            }
            
            Caculate();
        }
        public void Remove(Operation operation, long value)
        {
            AttributeMotifier removeTarget = new AttributeMotifier() {operation = operation, value = value};
            bool rst = _motifiers.Remove(removeTarget);
            if (rst)
            {
                Caculate();
            }
            else
            {
                Debug.LogError("fail to remove");   
            }
        }
        public void Caculate()
        {
            FinalValue = BaseValue;
            foreach (var item in _motifiers)
            {
                if(item.operation == Operation.PERCENT)
                {
                    FinalValue *= item.value;
                }
                else
                {
                    FinalValue += item.value;
                }
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in _motifiers)
            {
                sb.Append(item.operation);
                sb.Append(item.value);
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
    public class AttributeManager
    {
        private Dictionary<AttributeType, CharacterAttribute> _attributes = new Dictionary<AttributeType, CharacterAttribute>();
        public Action<AttributeType, long, long> OnAttributeChange;
        public long this[AttributeType at]
        {
            get {
                if (!HasAttribute(at))
                {
                    Debug.LogError(at + " Key not exsit");
                    return 0;
                }
                return _attributes[at].FinalValue;
            }
        }
        public bool HasAttribute(AttributeType at)
        {
            return _attributes.ContainsKey(at);
        }
        public void New(AttributeType at, long value)
        {
            _attributes[at] = new CharacterAttribute(value);
        }
        public void Remove(AttributeType at, Operation op, long value)
        {
            if (_attributes.ContainsKey(at))
            {
                var attr = _attributes[at];
                long oldValue = attr.FinalValue;
                attr.Remove(op, value);
                if (OnAttributeChange != null)
                {
                    OnAttributeChange.Invoke(at, oldValue, attr.FinalValue);
                }
            }
            else
            {
                Debug.LogError(at + " Key not exsit");
            }
        }
        public void SetBase(AttributeType at, long value)
        {
            var attr = _attributes[at];
            long oldValue = attr.FinalValue;
            _attributes[at].SetBase(value);
            if (OnAttributeChange != null)
            {
                OnAttributeChange.Invoke(at, oldValue, attr.FinalValue);
            }
        }
        public void Add(AttributeType at, Operation op, long value)
        {
            if(_attributes.ContainsKey(at))
            {
                var attr = _attributes[at];
                long oldValue = attr.FinalValue;
                attr.Add(op, value);
                if(OnAttributeChange != null)
                {
                    OnAttributeChange.Invoke(at, oldValue, attr.FinalValue);
                }
            }else
            {
                Debug.LogError(at+ " Key not exsit");
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var attribute in _attributes)
            {
                sb.Append(attribute.Key);
                sb.Append(attribute.Value.ToString());
            }
            return sb.ToString();
        }
    }

}
