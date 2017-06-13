using UnityEngine;
using System.Collections;
using Logic.Skill;
using UnityEditor;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Logic.Skill.Actions;
using Newtonsoft.Json;

public enum TimelingGroupType
{
    SKILL,
    BUFF,
    EVENT
}
public class CreateSkillWindow : EditorWindow
{
    private string skillname = "";
    private string folder="";
    private string id = "";
    private TimelingGroupType tgType;
    public SKillWindow skillWindow;

    void SetName(object obj)
    {
        var properties = obj.GetType().GetProperties();
        for (int i = 0; i < properties.Length; i++)
        {
            if (properties[i].Name == "Name")
            {
                properties[i].SetValue(obj, skillname, null);
            }
        }
    }
    void SetID(object obj)
    {
        var properties = obj.GetType().GetProperties();
        for (int i = 0; i < properties.Length; i++)
        {
            if (properties[i].Name == "ID")
            {
                properties[i].SetValue(obj, int.Parse(id), null);
            }
        }
    }
    void SetPath(object obj)
    {
        var properties = obj.GetType().GetProperties();
        for (int i = 0; i < properties.Length; i++)
        {
            if (properties[i].Name == "path")
            {
                properties[i].SetValue(obj, folder, null);
            }
        }
    }
    void OnGUI()
    {
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        GUILayout.Label("名字");
        skillname = GUILayout.TextField(skillname, GUILayout.MinWidth(100));
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("ID");
        id = GUILayout.TextField(id, GUILayout.MinWidth(100));
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("路径");
        folder = GUILayout.TextField(folder, GUILayout.MinWidth(100));
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("创建类型");
        tgType = (TimelingGroupType)EditorGUILayout.EnumPopup(tgType, GUILayout.MinWidth(100));
        GUILayout.EndHorizontal();
        int id_int = 0;
        if (!string.IsNullOrEmpty(id) && int.TryParse(id, out id_int) && !string.IsNullOrEmpty(skillname) && !string.IsNullOrEmpty(folder) && (GUILayout.Button("创建")))
        {
            string path = Application.streamingAssetsPath;
            TimeLineGroup tg = null;
            JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            if (tgType == TimelingGroupType.SKILL)
            {
                path += "/Skills/";
                tg = new Skill();
                tg.TimeLines.Add(new TimeLine());
                tg.TimeLines[0].BaseActions.Add(new DamageAction());
                tg.TimeLines.Add(new TimeLine());
                path += folder;
                path += ".skill";
                SetName(tg);
                SetID(tg);
                SetPath(tg);
                string text = Newtonsoft.Json.JsonConvert.SerializeObject(tg, Newtonsoft.Json.Formatting.Indented, settings);
                File.WriteAllText(path, text);
            }
            else if (tgType == TimelingGroupType.BUFF)
            {
                path += "/Buffs/";
                tg = new Buff();
                tg.TimeLines.Add(new TimeLine());
                tg.TimeLines.Add(new TimeLine());
                path += folder;
                path += ".buff";
                SetName(tg);
                SetID(tg);
                SetPath(tg);
                string text = Newtonsoft.Json.JsonConvert.SerializeObject(tg, Newtonsoft.Json.Formatting.Indented, settings);
                File.WriteAllText(path, text);
               
            }
            else if (tgType == TimelingGroupType.EVENT)
            {
                path += "/Events/";
                tg = new Logic.Skill.Event();
                tg.TimeLines.Add(new TimeLine());
                tg.TimeLines.Add(new TimeLine());
                path += folder;
                path += ".event";
                SetName(tg);
                SetID(tg);
                SetPath(tg);
                string text = Newtonsoft.Json.JsonConvert.SerializeObject(tg, Newtonsoft.Json.Formatting.Indented, settings);
                File.WriteAllText(path, text);
            }
            skillWindow.OnCreate();
            Close();
        }
        GUILayout.EndVertical();

    }
}
