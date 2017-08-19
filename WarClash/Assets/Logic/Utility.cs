using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public static class Utility
{
    public static StringBuilder stringbuilder = new StringBuilder();
        
    public static void Clear(this StringBuilder builder)
    {
        if(builder!=null)
        {
            builder.Length = 0;
            builder.Capacity = 0;
        }
    }

    public static T GetComponent<T>(this GameObject go, bool add) where T : Component
    {
        var comp = go.GetComponent<T>();
        if (comp == null && add)
        {
            comp = go.AddComponent<T>();
        }
        return comp;
    }
    public static byte[] ReadByteFromStreamingAsset(string path)
    {
        stringbuilder.Clear();
        stringbuilder.Append(Application.streamingAssetsPath);
        stringbuilder.Append("/");
        stringbuilder.Append(path);
        byte[] bytes = File.ReadAllBytes(stringbuilder.ToString());
        return bytes;
    }
    public static string ReadStringFromStreamingAsset(string path)
    {
        stringbuilder.Clear();
        stringbuilder.Append(Application.streamingAssetsPath);
        stringbuilder.Append("/");
        stringbuilder.Append(path);
        string str = File.ReadAllText(stringbuilder.ToString());
        return str;
    }

}
