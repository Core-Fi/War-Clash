
using Lockstep;
using Logic;
using Logic.Components;
using Logic.LogicObject;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RogueSharp;
using RogueSharp.Random;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using UnityEngine;

public class TestBundleUnload : MonoBehaviour
{
    // Use this for initialization
    void OnEnable ()
	{
        int width = 300, height = 100;
        IRandom random = new DotNetRandom(12);
        IMapCreationStrategy<Map> mapCreationStrategy = new RandomRoomsMapCreationStrategy<Map>(width, height, 30, 30, 10, random);
      //  mapCreationStrategy = new CaveMapCreationStrategy<Map>(width, height, 30, 20, 5, random);
        IMap actualMap = Map.Create(mapCreationStrategy);
        Color[] colors = new Color[width * height];
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                var cell = actualMap.GetCell(j, i);
                if(cell.IsWalkable)
                {
                    colors[width * i + j] = Color.red;
                }
                else
                {
                    colors[width * i + j] = Color.blue;
                }
            }
        }
        Texture2D tex = new Texture2D(width , height);
        tex.SetPixels(colors);
        tex.Apply();
        var bytes = tex.EncodeToPNG();
        File.WriteAllBytes(Application.dataPath+"/randomtxt.png", bytes);
        //var str = actualMap.ToString();
        //str = str.Replace(".", "<color=#C5C1407B>#</color>");
        //DLog.Log(str);
        //int[,] map = MapGenerate(20, 20, 3, 5, 3, 8);
        //string s = "";
        //for (int i = 0; i < map.GetLength(0); i++)
        //{
        //    for (int j = 0; j < map.GetLength(1); j++)
        //    {
        //        if(map[i,j] == 0)
        //        {
        //            s += "#";
        //        }
        //        else
        //        {
        //            s += "<color=#111111>#</color>";
        //        }
        //    }
        //    s += "\r\n";
        //}
      //  DLog.Log(s);
    }
    void NewMap()
    {
        int w = 300, h = 300;

    }


    int[,] MapGenerate(int w, int h, int min, int max, int distanceMin, int distanceMax)
    {
        int[,] map = new int[w, h];
        int lasth = 0;
        while(lasth<h)
        {
            int lastw = 0;
            var starty = UnityEngine.Random.Range(min, max);
            lasth += starty;
            if (lasth >= h)
            {
                continue;
            }
            while (lastw < w)
            {
                var startx = UnityEngine.Random.Range(distanceMin, distanceMax);
                lastw += startx;
                var rw = UnityEngine.Random.Range(distanceMin, distanceMax);
                if (lastw+rw >= w)
                    continue;
                for (int i = 0; i < rw; i++)
                {
                    map[lasth, lastw+i] = 1;
                }
                lastw += rw;
            }

        }
        return map;
    }
   
    // Update is called once per frame
    void OnDisable () {
		//AssetResources.UnloadAsset("WK_archer.prefab");
  //      GC.Collect();
	 //   Resources.UnloadUnusedAssets();
	}
    public class ShouldSerializeContractResolver : DefaultContractResolver
    {
        public new static readonly ShouldSerializeContractResolver Instance = new ShouldSerializeContractResolver();

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);
            var attributes = member.GetCustomAttributes(false);
            property.ShouldSerialize = instance => {
                return attributes.Length > 0 && attributes[0] is JsonPropertyAttribute;
            };

            return property;
        }
    }
}
