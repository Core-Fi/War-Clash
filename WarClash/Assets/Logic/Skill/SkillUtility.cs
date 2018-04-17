using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Logic.Skill
{
    public class SkillUtility
    {
        public static string GetRequiredConfigsPath()
        {
            return Application.dataPath + "/RequiredResources/TextConfigs/";
        }
        public static JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All, Formatting = Formatting.Indented};
        public static T GetTimelineGroupFullPath<T>(string path) where T : TimeLineGroup
        {
            string text = File.ReadAllText(path);
            T t = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(text, settings);
            return t;
        }
        public static T GetTimelineGroup<T>(string path) where T : TimeLineGroup
        {
            if (Application.isPlaying)
            {
                var text = AssetResources.LoadAssetImmediatly(path) as TextAsset;
                T t = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(text.text, settings);
                return t;
            }
            else
            {
                string text = Utility.ReadStringFromAbsolutePath(GetRequiredConfigsPath()+path);
                T t = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(text, settings);
                return t;
            }
        }
        public static Dictionary<int, string> LoadIndexFile(string fpath)
        {
            if (Application.isPlaying)
            {
                var text = AssetResources.LoadAssetImmediatly(fpath) as TextAsset;
                return Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, string>>(text.text, settings);
              
            }
            else
            {
                var text = Utility.ReadStringFromAbsolutePath(GetRequiredConfigsPath() + fpath);
                return Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, string>>(text, settings);
            }
        }
#if UNITY_EDITOR
        public static void SaveTimelineGroup(TimeLineGroup skill, string path)
        {
            string text = Newtonsoft.Json.JsonConvert.SerializeObject(skill, Formatting.Indented, settings);
            File.WriteAllText(path, text);
        }
     
        public static void SaveToSkillIndexFile(TimeLineGroup tlg, string fpath)
        {
            string indexPath = GetRequiredConfigsPath();
            if (tlg is Skill)
            {
                indexPath += "Skills/skill_index.bytes";
            }
            else if (tlg is Event)
            {
                indexPath += "Events/event_index.bytes";
            }
            else
            {
                indexPath += "Buffs/buff_index.bytes";
            }
            Dictionary<int, string> dic = null;
            if (File.Exists(indexPath))
            {
                var text = File.ReadAllText(indexPath);
                dic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, string>>(text, settings);
            }
            else
            {
                File.Create(indexPath).Dispose();
                dic = new Dictionary<int, string>();
            }
            
            if (!dic.ContainsKey(tlg.ID))
            {
                fpath = fpath.Replace(@"\\", @"/");
                fpath = Path.GetFileName(fpath);
                if (fpath[0].Equals('/'))
                    fpath.Remove(0);
                dic.Add(tlg.ID, fpath);
                string text = Newtonsoft.Json.JsonConvert.SerializeObject(dic, Formatting.Indented, settings);
                File.WriteAllText(indexPath, text);
            }
        }

#endif
    }
}
