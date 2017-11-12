using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Logic.LogicObject;


class LogicObjectCorresponding
{
    public static readonly Dictionary<Type, Type> Corresponding =  new Dictionary<Type, Type>()
    {
        {typeof(Npc), typeof(U3DNpc)},
        {typeof(BarackBuilding), typeof(U3DBarackBuilding)},
        {typeof(Player), typeof(U3DPlayer) },
          {typeof(MainPlayer), typeof(U3DMainPlayer) },
        {typeof(StrightProjectile), typeof(U3DProjectile) },
        {typeof(TargetProjectile), typeof(U3DProjectile) },
        {typeof(Tower), typeof(U3DBarackBuilding) }
    };

}
