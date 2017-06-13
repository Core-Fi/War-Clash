using Logic.LogicObject;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logic;
using System;

public abstract class U3DSceneObject {

    public SceneObject so;

	public void Init(SceneObject so)
    {
        this.so = so;
        OnInit();
    }
    public virtual void OnInit()
    {
        so.EventGroup.AddEvent((int)SceneObject.SceneObjectEvent.POSITIONCHANGE, OnPositionChange);
    }
    private void OnPositionChange(object sender, EventMsg e)
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
