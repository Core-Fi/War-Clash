using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Object = UnityEngine.Object;


class U3DPlayer : U3DCharacter
{
    public override void OnInit()
    {
        base.OnInit();
        Resource.LoadAsset("Footman_prefab.prefab", OnLoadedRes);
    }

    public override void OnLoadedRes(string name, Object obj)
    {
        base.OnLoadedRes(name, obj);
    }
    public override void OnUpdate()
    {
        base.OnUpdate();
        if (Go != null)
        {
            Go.transform.position = Character.Position.ToVector3();
            Go.transform.forward = Character.Forward.ToVector3();
        }
    }
}
