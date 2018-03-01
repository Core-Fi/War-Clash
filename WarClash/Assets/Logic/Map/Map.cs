using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FastCollections;
using Logic.Skill;
using UnityEngine;

namespace Logic.Map
{
    public class Map
    {
        public Dictionary<int, MapItem> MapDic = new Dictionary<int, MapItem>();

        public static Map Deserialize(string name)
        {
            var str = AssetResources.LoadAssetImmediatly(name + ".bytes") as TextAsset;// Utility.ReadStringFromStreamingAsset("Map/" + name + ".map");
            if (!string.IsNullOrEmpty(str.text))
            {
                Map map = new Map
                {
                    MapDic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, MapItem>>(str.text, SkillUtility.settings)
                };

                return map;
            }
            return null;
        }
    }
}
