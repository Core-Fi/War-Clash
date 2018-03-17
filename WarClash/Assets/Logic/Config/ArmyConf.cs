///Code Generated Automatically, it would be dangerous if code changed
using System;
using System.Text;
using UnityEngine;
using System.Collections.Generic;
public class ArmyConf : BaseConfig
{
   public int Id;
   public int Hp;
   public string ResPath;
   public string BT;
   public int AtkRange;
   public int WaringRange;
   public string str;
   public int[] array;
   public void Desearize(byte[] bytes, ref int startIndex)
   {
      Id = ToInt(bytes, ref startIndex);
      Hp = ToInt(bytes, ref startIndex);
      ResPath = ToString(bytes, ref startIndex);
      BT = ToString(bytes, ref startIndex);
      AtkRange = ToInt(bytes, ref startIndex);
      WaringRange = ToInt(bytes, ref startIndex);
      str = ToString(bytes, ref startIndex);
      array = ToIntArray(bytes, ref startIndex);
   }
   private static void Deserialize(byte[] bytes)
   {
       int startIndex = 0;
       while (startIndex<bytes.Length)
       {
           var conf = new ArmyConf();
           conf.Desearize(bytes, ref startIndex);
           Configs.Add(conf.Id, conf);
       }
   }
   public static void Init()
   {
       var txt = AssetResources.LoadAssetImmediatly("armyconf.bytes" ) as TextAsset;
       Deserialize(txt.bytes);
   }
   public static ArmyConf Get(int id)
   {
       ArmyConf conf;
       if(Configs.TryGetValue(id, out conf))
       {
           return conf;
       }
       Debug.LogError(id + " Not Exsit In ArmyConf");
       return null;
   }
    private static Dictionary<int, ArmyConf> Configs = new Dictionary<int, ArmyConf>();
}
