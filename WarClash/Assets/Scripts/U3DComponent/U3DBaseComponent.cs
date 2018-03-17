using Logic.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public abstract class U3DBaseComponent 
{
    public BaseComponent Component;
    public U3DSceneObject U3DSceneObject;
    public virtual void OnAdd(BaseComponent c)
    {
        Component = c;
    }
    public virtual void OnRemove() { }
    public virtual void OnUpdate() { }

}
