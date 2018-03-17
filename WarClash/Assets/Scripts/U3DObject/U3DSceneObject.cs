using Logic.LogicObject;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logic;
using System;
using Logic.Components;

public class U3DSceneObject : IUpdate {

    public SceneObject SceneObject;
    private List<U3DBaseComponent> _components = new List<U3DBaseComponent>();
    public U3DTransformComponent U3DTransformComponent
    {
        get
        {
            if (_u3dTransformComp == null)
                _u3dTransformComp = GetComponent<U3DTransformComponent>();
            return _u3dTransformComp;
        }
    }
    private U3DTransformComponent _u3dTransformComp;
    public T AddComponent<T>(BaseComponent comp) where T : U3DBaseComponent
    {
        for (int i = 0; i < _components.Count; i++)
        {
            if (_components[i] is T)
                return null;
        }
        T t = Activator.CreateInstance<T>();
        t.OnAdd(comp);
        _components.Add(t);
        return t;
    }
    public U3DBaseComponent AddComponent(BaseComponent bc)
    {
        var t = bc.GetType();
        for (int i = 0; i < _components.Count; i++)
        {
            if (_components[i].GetType() == t)
                return null;
        }
        var nt = ComponentCorresponding.Corresponding[t];
        U3DBaseComponent c = Activator.CreateInstance(nt) as U3DBaseComponent;
        c.OnAdd(bc);
        _components.Add(c);
        return c;
    }
    public void RemoveComponent<T>() where T : BaseComponent
    {
        for (int i = 0; i < _components.Count; i++)
        {
            if (_components[i] is T)
            {
                _components[i].OnRemove();
                _components.RemoveAt(i);
            }
        }
    }
    public bool HasComponent<T>() where T : U3DBaseComponent
    {
        for (int i = 0; i < _components.Count; i++)
        {
            if (_components[i] is T)
            {
                return true;
            }
        }
        return false;
    }
    public List<U3DBaseComponent> GetAllComponents()
    {
        return _components;
    }
    public T GetComponent<T>() where T : U3DBaseComponent
    {
        for (int i = 0; i < _components.Count; i++)
        {
            if (_components[i] is T)
            {
                return (T)_components[i];
            }
        }
        return null;
    }
    public void Init(SceneObject so)
    {
        this.SceneObject = so;
        var comps = SceneObject.GetAllComponents();
        for (int i = 0; i < comps.Count; i++)
        {
            AddComponent(comps[i]);
        }
    }

    public void Update(float deltaTime)
    {
        for (int i = 0; i < _components.Count; i++)
        {
            _components[i].OnUpdate();
        }
    }
    public void Destroy()
    {
        OnDestroy();
    }
    public virtual void OnDestroy()
    {
       // UnityEngine.Object.Destroy(Go);
    }

    public void HideMainGo()
    {
       // Go.SetActive(false);
    }

    public void ShowMainGo()
    {
        //Go.SetActive(true);
    }

    public void SetOuterAsParent(Transform t)
    {
       // t.SetParent(OuterTransform);
    }
}
