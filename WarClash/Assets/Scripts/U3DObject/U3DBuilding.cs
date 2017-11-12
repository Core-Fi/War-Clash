using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


class U3DBuilding : U3DSceneObject
{
    protected Building Building;
    public override void OnInit()
    {
        base.OnInit();
        this.Building = So as Building;
        AssetResources.LoadAsset(Building.Conf.ResPath, OnLoadedRes);
    }
    
    public override void ListenEvents()
    {
        base.ListenEvents();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
    }
}
