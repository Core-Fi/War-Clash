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
        public static JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
        public static T GetTimelineGroupFullPath<T>(string path) where T : TimeLineGroup
        {
            string text = File.ReadAllText(path);
            T t = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(text, settings);
            return t;
        }
        public static T GetTimelineGroup<T>(string path) where T : TimeLineGroup
        {
            string text = Utility.ReadStringFromStreamingAsset(path);
            T t = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(text, settings);
            return t;
        }
        public static Dictionary<int, string> LoadIndexFile(string fpath)
        {
            var text = Utility.ReadStringFromStreamingAsset(fpath + "/_index.txt");
            return Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, string>>(text, settings);
        }
#if UNITY_EDITOR
        public static void SaveTimelineGroup(TimeLineGroup skill, string path)
        {
            string text = Newtonsoft.Json.JsonConvert.SerializeObject(skill, Formatting.Indented, settings);
            File.WriteAllText(path, text);
        }
     
        public static void SaveToSkillIndexFile(TimeLineGroup tlg, string fpath)
        {
            string indexPath = Application.streamingAssetsPath + "/";
            if (tlg is Skill)
            {
                indexPath += "Skills/_index.txt";
            }
            else if (tlg is Event)
            {
                indexPath += "Events/_index.txt";
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
                dic.Add(tlg.ID, fpath);
                string text = Newtonsoft.Json.JsonConvert.SerializeObject(dic, Formatting.Indented, settings);
                File.WriteAllText(indexPath, text);
            }
        }

#endif
    }
}
