using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Logic
{
    public enum AttributeType
    { 
        HP ,
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
        public AttributeType attribute { get; private set; }
        public CharacterAttribute(long value)
        {
            baseValue = value;
        }
        public void Change(Operation operation, long value)
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
}
