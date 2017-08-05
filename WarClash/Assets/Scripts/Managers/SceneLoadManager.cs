using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class SceneLoadManager : Manager
{
    Dictionary<string, ResourceLoader> toBeLoadResources = new Dictionary<string, ResourceLoader>();
    Action<string, UnityEngine.Object> onLoadFinish;
    public SceneLoadManager()
    {
        onLoadFinish = OnLoadFinish;
        ListenEvent((int)EventList.PreLoadResource, PreLoadResource);
    }

    private void PreLoadResource(object sender, EventMsg e)
    {
        EventSingleArgs<string> msg = e as EventSingleArgs<string>;
        var rl = Pool.SP.Get(typeof(ResourceLoader)) as ResourceLoader;
        rl.Start(msg.value, onLoadFinish);
    }
    private void OnLoadFinish(string path, UnityEngine.Object obj)
    {
        if (obj is GameObject)
        {
            Transform t = (obj as GameObject).transform;
            //PoolManager.Pools["objects"].Add(t, path, false, true);
        }
    }
    public override void OnUpdate()
    {
        base.OnUpdate();
        

    }

}

