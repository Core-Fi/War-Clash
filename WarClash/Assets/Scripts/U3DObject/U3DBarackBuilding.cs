using Logic.LogicObject;
using UnityEngine;

class U3DBarackBuilding : U3DBuilding
{
    private BarackBuilding barackBuilding;
    public override void OnInit()
    {
        base.OnInit();
        barackBuilding = So as BarackBuilding;
        AssetResources.LoadAsset(barackBuilding.Conf.ResPath, OnLoadedRes);
    }
    public override void ListenEvents()
    {
        base.ListenEvents();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        if(Go!=null)
            Go.transform.position = Building.Position.ToVector3();
    }

    public override void OnLoadedRes(string name, Object obj)
    {
        base.OnLoadedRes(name, obj);
    }
}
