using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Object = UnityEngine.Object;


public class U3DMainPlayer : U3DPlayer
{
    public override void OnInit()
    {
        base.OnInit();
    }

    public override void OnLoadedRes(string name, Object obj)
    {
        base.OnLoadedRes(name, obj);
     //   EventDispatcher.FireEvent(UIEventList.DrawPlayerHud.ToInt(), this, null);
    }
}
