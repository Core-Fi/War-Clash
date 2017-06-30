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
        SPEED ,
        CHARACTEREND,
    }
    public enum Operation
    { 
        ABSOLUTE,
        PERCENT
    }
    public class AttributeMotifier
    {
        public Operation operation;
        public long value;
    }
    public class CharacterAttribute
    {
        public long baseValue { get; private set; }
        private LinkedList<AttributeMotifier> motifiers = new LinkedList<AttributeMotifier>();
        public long finalValue { get; private set; }
     //   public AttributeType attribute { get; private set; }
        public CharacterAttribute(long value)
        {
            baseValue = value;
            finalValue = value;
        }
        public void SetBase(long value)
        {
            baseValue = value;
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
                motifiers.AddFirst(attr);
            }else
            {
                motifiers.AddLast(attr);
            }
            
            Caculate();
        }
        public void Remove(Operation operation, long value)
        {
            AttributeMotifier removeTarget = null;
            foreach (var item in motifiers)
            {
                if (item.operation == Operation.PERCENT && item.value == value)
                {
                    removeTarget = item;
                    break;
                }
            }
            motifiers.Remove(removeTarget);
            Caculate();

        }
        public void Caculate()
        {
            finalValue = baseValue;
            foreach (var item in motifiers)
            {
                if(item.operation == Operation.PERCENT)
                {
                    finalValue *= item.value;
                }
                else
                {
                    finalValue += item.value;
                }
            }
        }
    }
    public struct AttributeMsg
    {
        public AttributeType at;
        public long oldValue;
        public long newValue;
    }
    public class AttributeManager
    {
        private Dictionary<AttributeType, CharacterAttribute> attributes = new Dictionary<AttributeType, CharacterAttribute>();
        public Action<AttributeType, long, long> OnAttributeChange;
        public long this[AttributeType at]
        {
            get {
                if (!HasAttribute(at))
                {
                    Debug.LogError(at + " Key not exsit");
                    return 0;
                }
                return attributes[at].finalValue;
            }
        }
        public bool HasAttribute(AttributeType at)
        {
            return attributes.ContainsKey(at);
        }
        public void New(AttributeType at, long value)
        {
            attributes[at] = new CharacterAttribute(value);
        }
        public void Remove(AttributeType at, Operation op, long value)
        {
            if (attributes.ContainsKey(at))
            {
                var attr = attributes[at];
                long oldValue = attr.finalValue;
                attr.Remove(op, value);
                if (OnAttributeChange != null)
                {
                    OnAttributeChange.Invoke(at, oldValue, attr.finalValue);
                }
            }
            else
            {
                Debug.LogError(at + " Key not exsit");
            }
        }
        public void SetBase(AttributeType at, long value)
        {
            attributes[at].SetBase(value);
        }
        public void Add(AttributeType at, Operation op, long value)
        {
            if(attributes.ContainsKey(at))
            {
                var attr = attributes[at];
                long oldValue = attr.finalValue;
                attr.Add(op, value);
                if(OnAttributeChange != null)
                {
                    OnAttributeChange.Invoke(at, oldValue, attr.finalValue);
                }
            }else
            {
                Debug.LogError(at+ " Key not exsit");
            }
        }

    }

}
