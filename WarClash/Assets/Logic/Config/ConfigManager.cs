using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Config;
using UnityEngine;

namespace Logic.Config
{
   
    class ConfigMap<T> where T : class 
    {
        private static readonly Dictionary<int, T> ConfDic = new Dictionary<int, T>();

        public static T Get(int id)
        {
            T item = null;
            if (ConfDic.TryGetValue(id, out item))
            {
            }
            return item;
        }
        public static void LoadBuildingConf()
        {
            var data = Utility.ReadByteFromStreamingAsset("Config/buildingconf.data");
            using (MemoryStream ms = new MemoryStream(data))
            {
                BuildingConf_ARRAY array = ProtoBuf.Serializer.Deserialize<BuildingConf_ARRAY>(ms);
                foreach (var conf in array.items)
                {
                    ConfigMap<BuildingConf>.ConfDic.Add(conf.Id, conf);
                }
            }
        }
        public static void LoadArmyConf()
        {
            var data = Utility.ReadByteFromStreamingAsset("Config/armyconf.data");
            using (MemoryStream ms = new MemoryStream(data))
            {
                ArmyConf_ARRAY array = ProtoBuf.Serializer.Deserialize<ArmyConf_ARRAY>(ms);
                foreach (var conf in array.items)
                {
                    ConfigMap<ArmyConf>.ConfDic.Add(conf.Id, conf);
                }
            }
        }
    }
}
