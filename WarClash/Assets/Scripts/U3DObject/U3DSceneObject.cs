using Logic.LogicObject;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logic;
using System;

public abstract class U3DSceneObject : IUpdate {
    
    public SceneObject So;
    protected GameObject Go;
    protected LogicObject LogicObject;
	public void Init(SceneObject so)
    {
        this.So = so;
        OnInit();
    }

    public virtual void OnLoadedRes(string name, UnityEngine.Object obj)
    {
        LogicObject = Go.GetComponent<LogicObject>(true);
        LogicObject.ID = So.Id;
    }
    public virtual void OnInit()
    {
        
    }
    public virtual void ListenEvents()
    {

    }
    public void Update(float deltaTime)
    {
        OnUpdate();
    }
    public virtual void OnUpdate()
    {

    }
    public void Destroy()
    {
        OnDestroy();
    }
    public virtual void OnDestroy()
    {

    }
}
