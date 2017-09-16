using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FastCollections;

namespace Logic.Map
{
    public class Map
    {
        public Dictionary<Type, Dictionary<int, MapItem>> MapDic = new Dictionary<Type, Dictionary<int, MapItem>>();

        public static Map Deserialize(string name)
        {
            var str = Utility.ReadStringFromStreamingAsset("Map/" + name + ".map");
            if (!string.IsNullOrEmpty(str))
            {
                var map = Newtonsoft.Json.JsonConvert.DeserializeObject<Map>(str);
                return map;
            }
            return null;
        }
    }
}
