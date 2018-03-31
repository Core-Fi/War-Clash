using Logic.LogicObject;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logic;
using System;
using Logic.Components;
using Logic.Skill.Actions;

public class U3DSceneObject : IUpdate {

    public SceneObject SceneObject;
    private List<U3DBaseComponent> _components = new List<U3DBaseComponent>();
    private List<U3DDisplayAction> _displayActions = new List<U3DDisplayAction>();
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
    public T AddComponent<T>(SceneObjectBaseComponent comp) where T : U3DBaseComponent
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
    public U3DBaseComponent AddComponent(SceneObjectBaseComponent bc)
    {
        var t = bc.GetType();
        for (int i = 0; i < _components.Count; i++)
        {
            if (_components[i].GetType() == t)
                return null;
        }
        Type nt = null;
        if (ComponentCorresponding.Corresponding.TryGetValue(t, out nt))
        {
            U3DBaseComponent c = Activator.CreateInstance(nt) as U3DBaseComponent;
            c.U3DSceneObject = this;
            c.OnAdd(bc);
            _components.Add(c);
            return c;
        }
        return null;
    }
    public void RemoveComponent(SceneObjectBaseComponent bc)
    {
        for (int i = 0; i < _components.Count; i++)
        {
            if (_components[i].Component == bc)
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
        so.ListenEvent((int)SceneObject.SceneObjectEvent.OnAddComponent, OnSceneObjectAddComponent);
        so.ListenEvent((int)SceneObject.SceneObjectEvent.OnRemoveComponent, OnSceneObjectRemoveComponent);
        so.ListenEvent((int)SceneObject.SceneObjectEvent.Executedisplayaction, PlayDisplayAction);
        so.ListenEvent((int)SceneObject.SceneObjectEvent.Stopdisplayaction, StopDisplayAction);

    }
    private void OnSceneObjectAddComponent(object sender, EventMsg msg)
    {
        var so = sender as SceneObject;
        var e = msg as EventSingleArgs<SceneObjectBaseComponent>;
        AddComponent(e.value);
    }
    private void OnSceneObjectRemoveComponent(object sender, EventMsg msg)
    {
        var so = sender as SceneObject;
        var e = msg as EventSingleArgs<SceneObjectBaseComponent>;
        RemoveComponent(e.value);
    }
    private void PlayDisplayAction(object sender, EventMsg e)
    {
        var msg = e as EventSingleArgs<DisplayAction>;
        Type t = U3DDisplayAction.LogicDisplayActions[msg.value.GetType()];
        var u3dAction = Pool.SP.Get(t) as U3DDisplayAction;
        u3dAction.Action = msg.value;
        _displayActions.Add(u3dAction);
        u3dAction.Execute(this, null, null);
    }
    private void StopDisplayAction(object sender, EventMsg e)
    {
        var msg = e as EventSingleArgs<DisplayAction>;
        for (int i = 0; i < _displayActions.Count; i++)
        {
            if(_displayActions[i].Action == msg.value)
            {
                _displayActions[i].Stop();
                _displayActions.RemoveAt(i);
                break;
            }
        }
    }
    public void Update(float deltaTime)
    {
        for (int i = 0; i < _displayActions.Count; i++)
        {
            _displayActions[i].Update();
        }
        for (int i = 0; i < _components.Count; i++)
        {
            _components[i].OnUpdate();
        }
    }
    public void Destroy()
    {
        SceneObject.DelEvent((int)SceneObject.SceneObjectEvent.OnAddComponent, OnSceneObjectAddComponent);
        SceneObject.DelEvent((int)SceneObject.SceneObjectEvent.OnRemoveComponent, OnSceneObjectRemoveComponent);
        SceneObject.DelEvent((int)SceneObject.SceneObjectEvent.Executedisplayaction, PlayDisplayAction);
        SceneObject.DelEvent((int)SceneObject.SceneObjectEvent.Stopdisplayaction, StopDisplayAction);
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
