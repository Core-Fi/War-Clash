using Logic.LogicObject;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logic;
using System;

public abstract class U3DSceneObject : IUpdate {
    
    public SceneObject So;
    protected GameObject Outer;
    protected Transform OuterTransform;
    protected GameObject Go;
    protected Transform Transform;
    protected LogicObject LogicObject;
	public void Init(SceneObject so)
    {
        this.So = so;
        Outer = new GameObject(so.ToString());
        OuterTransform = Outer.transform;
        LogicObject = Outer.GetComponent<LogicObject>(true);
        LogicObject.ID = So.Id;
        OnInit();
    }

    public virtual void OnLoadedRes(string name, UnityEngine.Object obj)
    {
        Go = UnityEngine.Object.Instantiate(obj) as GameObject;
        Go.name = name;
        Go.transform.position = So.Position.ToVector3();
        Transform = Go.transform;
        Transform.SetParent(OuterTransform);
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

    public void HideMainGo()
    {
        Go.SetActive(false);
    }

    public void ShowMainGo()
    {
        Go.SetActive(true);
    }

    public void SetOuterAsParent(Transform t)
    {
        t.SetParent(OuterTransform);
    }
}
