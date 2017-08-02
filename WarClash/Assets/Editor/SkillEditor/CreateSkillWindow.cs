using UnityEngine;
using System.Collections;
using Logic.Skill;
using UnityEditor;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Logic.Skill.Actions;
using Newtonsoft.Json;
using System;

public enum TimelingGroupType
{
    SKILL,
    BUFF,
    EVENT
}
public class CreateSkillWindow : EditorWindow
{
    private string skillname = "";
    private string skillpath="";
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
                properties[i].SetValue(obj, skillpath, null);
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
        skillpath = GUILayout.TextField(skillpath, GUILayout.MinWidth(100));
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("创建类型");
        tgType = (TimelingGroupType)EditorGUILayout.EnumPopup(tgType, GUILayout.MinWidth(100));
        GUILayout.EndHorizontal();
        int id_int = 0;
        if (!string.IsNullOrEmpty(id) && int.TryParse(id, out id_int) && !string.IsNullOrEmpty(skillname) && !string.IsNullOrEmpty(skillpath) && (GUILayout.Button("创建")))
        {
            string path = "";
            TimeLineGroup tg = null;
            JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            if (tgType == TimelingGroupType.SKILL)
            {
                path += "/Skills/";
                path += skillpath+".skill";
                tg = Create<Skill>(path);
            }
            else if (tgType == TimelingGroupType.BUFF)
            {
                path += "/Buffs/";
                path += skillpath+".buff";
                tg = Create<Buff>(path);

            }
            else if (tgType == TimelingGroupType.EVENT)
            {
                path += "/Events/";
                path += skillpath + ".event";
                tg = Create<Logic.Skill.Event>(path);
            }
            string finalPath = Application.streamingAssetsPath + path;
            SkillUtility.SaveTimelineGroup(tg, finalPath);
            SkillUtility.SaveToSkillIndexFile(tg, path);
            skillWindow.OnCreate();
            Close();
        }
        GUILayout.EndVertical();
    }
    public T Create<T>(string path) where T : TimeLineGroup
    {
        T tg = Activator.CreateInstance<T>();
        tg.TimeLines.Add(new TimeLine());
        tg.TimeLines.Add(new TimeLine());
        SetName(tg);
        SetID(tg);
        SetPath(tg);
        return tg;
    }

}
