using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


class EnumOrderAttribute : Attribute
{
    public int Order;
    public EnumOrderAttribute(int order)
    {
        this.Order = order;
    }
}
