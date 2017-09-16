using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
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
            Point p = new Point();
            p.column = 5;
            var bs = Serialize(p, typeof(Point));

        }
        public static void LoadGoodInfo()
        {
            var data = Utility.ReadByteFromStreamingAsset("Config/goods_info.data");
            using (MemoryStream ms = new MemoryStream(data))
            {
                GOODS_INFO_ARRAY array = ProtoBuf.Serializer.Deserialize<GOODS_INFO_ARRAY>(ms);
                foreach (var conf in array.items)
                {
                    ConfigMap<GOODS_INFO>.ConfDic.Add((int)conf.goods_id, conf);
                }
            }
        }
        /// <summary>
        /// serialize struct
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="T"></param>
        /// <returns></returns>
        public static byte[] Serialize(object obj, Type T)
        {
            var buffer = new byte[Marshal.SizeOf(T)];
            var gch = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            var pBuffer = gch.AddrOfPinnedObject();
            Marshal.StructureToPtr(obj, pBuffer, true);
            gch.Free();
            return buffer;
        }
        /// <summary>
        /// deserialize struct
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="T"></param>
        /// <returns></returns>
        public static T Deserialize<T>(byte[] data)
        {
            var gch = GCHandle.Alloc(data, GCHandleType.Pinned);
            var obj = (T)Marshal.PtrToStructure(gch.AddrOfPinnedObject(), typeof(T));
            gch.Free();
            return obj;
        }
    }
}
