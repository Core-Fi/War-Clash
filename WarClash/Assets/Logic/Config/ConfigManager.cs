using System.Collections.Generic;
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
   {}
   else{DLog.LogError(typeof(T)+ "的id" +id+ "不存在" );}
   return item;
}
public static void LoadArmyConf()
{
    var txtAsset = AssetResources.LoadAssetImmediatly( "armyconf.bytes" ) as TextAsset;
   using (MemoryStream ms = new MemoryStream(txtAsset.bytes)){
   ArmyConf_ARRAY array = ProtoBuf.Serializer.Deserialize<ArmyConf_ARRAY>(ms);
   foreach (var conf in array.items){
   ConfigMap<ArmyConf>.ConfDic.Add(conf.Id, conf);}
   };
}
public static void LoadBuildingConf()
{
    var txtAsset = AssetResources.LoadAssetImmediatly( "buildingconf.bytes" ) as TextAsset;
   using (MemoryStream ms = new MemoryStream(txtAsset.bytes)){
   BuildingConf_ARRAY array = ProtoBuf.Serializer.Deserialize<BuildingConf_ARRAY>(ms);
   foreach (var conf in array.items){
   ConfigMap<BuildingConf>.ConfDic.Add(conf.Id, conf);}
   };
}

}
}
