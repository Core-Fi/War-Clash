using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Manager : IManager
{
    public Manager()
    {
       
    }
    public void ListenEvent(int id, EventMsgHandler e)
    {
        EventDispatcher.ListenEvent(id, e);
    }

    public void DelEvent(int id, EventMsgHandler e)
    {
        EventDispatcher.DelEvent(id, e);
    }

    public void FireEvent(int id, object sender, EventMsg m)
    {
        EventDispatcher.FireEvent(id, sender, m);
    }

    public void Update()
    {
        OnUpdate();
    }

    public virtual void OnUpdate() { }

    public void Dispose()
    {
     OnDispose();   
    }

    public virtual void OnDispose()
    {
        
    }
}