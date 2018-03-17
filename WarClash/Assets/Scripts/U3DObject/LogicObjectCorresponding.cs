using Logic.Components;
using System;
using System.Collections.Generic;



class ComponentCorresponding
{
    public static readonly Dictionary<Type, Type> Corresponding =  new Dictionary<Type, Type>()
    {
        { typeof(TransformComponent), typeof(U3DTransformComponent) },
    };

}
