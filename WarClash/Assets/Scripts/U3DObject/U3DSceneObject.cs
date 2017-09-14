using Logic.LogicObject;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logic;
using System;

public abstract class U3DSceneObject : IUpdate {
    
    public SceneObject So;
    protected GameObject Go;
    protected Transform Transform;
    protected LogicObject LogicObject;
	public void Init(SceneObject so)
    {
        this.So = so;
        OnInit();
    }

    public virtual void OnLoadedRes(string name, UnityEngine.Object obj)
    {
        Go = UnityEngine.Object.Instantiate(obj) as GameObject;
        Go.name = So.ToString();
        Go.transform.position = So.Position.ToVector3();
        LogicObject = Go.GetComponent<LogicObject>(true);
        LogicObject.ID = So.Id;
        Transform = Go.transform;
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
        UnityEngine.Object.Destroy(Go);
    }
}
