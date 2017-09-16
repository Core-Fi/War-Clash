using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FastCollections;
using Logic.Skill;

namespace Logic.Map
{
    public class Map
    {
        public Dictionary<int, MapItem> MapDic = new Dictionary<int, MapItem>();

        public static Map Deserialize(string name)
        {
            var str = Utility.ReadStringFromStreamingAsset("Map/" + name + ".map");
            if (!string.IsNullOrEmpty(str))
            {
                Map map = new Map
                {
                    MapDic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, MapItem>>(str, SkillUtility.settings)
                };

                return map;
            }
            return null;
        }
    }
}
