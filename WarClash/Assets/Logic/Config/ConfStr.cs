///Code Generated Automatically, it would be dangerous if code changed
using System;
using System.Text;
using UnityEngine;
using System.Collections.Generic;
public class ConfStr : BaseConfig
{
   public string Id;
   public int ArmyId;
   public string ResPath;
   public int Param1;
   public string Param2;
   public enum Test
   {
     a,
     b,
   }
   public Test ToTest(byte[] bytes, ref int startIndex)
   {
       if (bytes[startIndex] == 0)
       {
           return 0;
       }
       startIndex++;
       var v = BitConverter.ToInt32(bytes, startIndex);
       startIndex += 4;
       return (Test)v;
   }
   public Test t;
   public enum Condition
   {
     win,
     lose,
   }
   public Condition ToCondition(byte[] bytes, ref int startIndex)
   {
       if (bytes[startIndex] == 0)
       {
            startIndex++;
            return 0;
       }
       startIndex++;
       var v = BitConverter.ToInt32(bytes, startIndex);
       startIndex += 4;
       return (Condition)v;
   }
   public Condition condition;
   public void Desearize(byte[] bytes, ref int startIndex)
   {
      Id = ToString(bytes, ref startIndex);
      ArmyId = ToInt(bytes, ref startIndex);
      ResPath = ToString(bytes, ref startIndex);
      Param1 = ToInt(bytes, ref startIndex);
      Param2 = ToString(bytes, ref startIndex);
      t = ToTest(bytes, ref startIndex);
      condition = ToCondition(bytes, ref startIndex);
   }
   private static void Deserialize(byte[] bytes)
   {
       int startIndex = 0;
       while (startIndex<bytes.Length)
       {
           var conf = new ConfStr();
           conf.Desearize(bytes, ref startIndex);
           Configs.Add(conf.Id, conf);
       }
   }
   public static void Init()
   {
       var txt = AssetResources.LoadAssetImmediatly("confstr.bytes" ) as TextAsset;
       Deserialize(txt.bytes);
   }
   public static ConfStr Get(string id)
   {
       ConfStr conf;
       if(Configs.TryGetValue(id, out conf))
       {
           return conf;
       }
       Debug.LogError(id + " Not Exsit In ConfStr");
       return null;
   }
    private static Dictionary<string, ConfStr> Configs = new Dictionary<string, ConfStr>();
}
