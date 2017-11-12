using Logic.LogicObject;
using UnityEngine;

class U3DBarackBuilding : U3DBuilding
{
    private BarackBuilding _barackBuilding;
    public override void OnInit()
    {
        base.OnInit();
        _barackBuilding = So as BarackBuilding;
    }
    public override void ListenEvents()
    {
        base.ListenEvents();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
       
    }

    public override void OnLoadedRes(string name, Object obj)
    {
        base.OnLoadedRes(name, obj);
    }
}
