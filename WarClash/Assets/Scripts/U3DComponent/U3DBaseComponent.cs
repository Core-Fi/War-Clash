using Logic.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public abstract class U3DBaseComponent : BaseComponent
{
    public SceneObjectBaseComponent Component;
    public U3DSceneObject U3DSceneObject;
    public virtual void OnAdd(SceneObjectBaseComponent c)
    {
        Component = c;
    }
    public virtual void OnUpdate() { }

}
