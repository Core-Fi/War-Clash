using UnityEngine;

class U3DBarackBuilding : U3DBuilding
{
    public override void OnInit()
    {
        base.OnInit();
        Resource.LoadAsset("Barrack.prefab", OnLoadedRes);
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
        Go = Object.Instantiate(obj) as GameObject;
        Go.transform.position = Building.Position.ToVector3();
        base.OnLoadedRes(name, obj);
    }
}
