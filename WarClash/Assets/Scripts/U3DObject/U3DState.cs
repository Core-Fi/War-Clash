using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Logic;
using Logic.LogicObject;
using UnityEngine;
using Object = UnityEngine.Object;


public abstract class U3DState : IPool
{
    public static readonly Dictionary<System.Type, System.Type> CorrospondingTypes = new Dictionary<Type, Type>
    {
        { typeof(IdleState), typeof(U3DIdleState)},
        { typeof(MoveState), typeof(U3DMoveState)},
        { typeof(GuiseState), typeof(U3DGuiseState)}

    };
    public U3DSceneObject U3DSceneObject;
    public State State;
    public void Start()
    {
        OnStart();
    }
    public void Update()
    {
        OnUpdate();
    }

    public void Stop()
    {
        OnStop();
    }
    protected abstract void OnStart();
    protected abstract void OnUpdate();
    protected abstract void OnStop();
    protected abstract void OnReset();
    public void Reset()
    {
        OnReset();
    }
}

public class U3DIdleState : U3DState
{
    protected override void OnStart()
    {
    }

    protected override void OnStop()
    {
    }

    protected override void OnUpdate()
    {
    }

    protected override void OnReset()
    {
        
    }
}

public class U3DMoveState : U3DState
{
    protected override void OnStart()
    {
    }

    protected override void OnStop()
    {
    }

    protected override void OnUpdate()
    {
    }
    protected override void OnReset()
    {

    }
}
public class U3DGuiseState : U3DState
{
    private GuiseState _guiseState;
    private GameObject guiseObj;
    protected override void OnStart()
    {
       _guiseState = State as GuiseState;
        AssetResources.LoadAsset(_guiseState.Path, OnLoad);
    }
    private void OnLoad(string path, Object obj)
    {
        if (State.SceneObject!=null)
        {
            //if ((State.SceneObject).StateMachine.state is GuiseState)
            //{
            //    if (guiseObj != null)
            //    {
            //        Object.Destroy(guiseObj);
            //    }
            //    base.U3DSceneObject.HideMainGo();
            //    guiseObj = Object.Instantiate(obj) as GameObject;
            //    base.U3DSceneObject.SetOuterAsParent(guiseObj.transform);
            //}
        }
    }
    protected override void OnStop()
    {
        if (guiseObj != null)
        {
            Object.Destroy(guiseObj);
            base.U3DSceneObject.ShowMainGo();
        }
    }

    protected override void OnUpdate()
    {
        
    }
    protected override void OnReset()
    {
        _guiseState = null;
        if (guiseObj != null)
        {
            Object.Destroy(guiseObj);
        }
    }
}