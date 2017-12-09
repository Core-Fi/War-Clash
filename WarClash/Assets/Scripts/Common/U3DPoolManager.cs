using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class U3DPoolManager {

    //private static Dictionary<string, Queue<Object>> Dic = new Dictionary<string, Queue<Object>>();

    //public static Object Get(string key)
    //{
    //    Queue <Object> queue;
    //    if (Dic.TryGetValue(key, out queue))
    //    {
    //        if (queue.Count > 1)
    //        {
    //            return queue.Dequeue();
    //        }
    //    }
    //    else
    //    {
    //        AssetResources.LoadAsset(key, OnLoadFinish);

    //    }
    //}

    //private static void OnLoadFinish(string path , UnityEngine.Object obj)
    //{
    //    Queue<Object> queue = new Queue<Object>();
    //    queue.Enqueue(obj);
    //    Dic[path] = (queue);

    //}
}
