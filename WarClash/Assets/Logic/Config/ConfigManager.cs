using System;
using System.Collections.Generic;
using System.IO;
using Config;

namespace Logic.Config
{
   
    class ConfigMap<T> where T : class 
    {
        private static Dictionary<uint, T> ConfDic = new Dictionary<uint, T>();
        public static void LoadBuildingConf()
        {
            var data = Utility.ReadByteFromStreamingAsset("Config/BuildingConf.data");
            using (MemoryStream ms = new MemoryStream(data))
            {
                BuildingConf_ARRAY array = ProtoBuf.Serializer.Deserialize<BuildingConf_ARRAY>(ms);
                foreach (var buildingConf in array.items)
                {
                    ConfigMap<BuildingConf>.ConfDic.Add(buildingConf.Id, buildingConf);
                }
            }
        }

    }
}
