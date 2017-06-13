using System;
using System.Collections.Generic;
using System.Text;

namespace Logic.Skill
{
    public class DisplayActionAttribute : Attribute
    {
        public Type type;

        public DisplayActionAttribute(Type _type)
        {
            this.type = _type;
        }
    }
}
