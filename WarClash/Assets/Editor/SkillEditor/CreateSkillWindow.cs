using UnityEngine;
using System.Collections;
using Logic.Skill;
using UnityEditor;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Logic.Skill.Actions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

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
    private int selectedIndex;
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
        var popups = new List<string>();
        for (int i = 0; i < SkillEditorUtility.skillTypes.Count; i++)
        {
            var type = SkillEditorUtility.skillTypes[i];
            popups.Add(type.FullName);
        }
        for (int i = 0; i < SkillEditorUtility.buffTypes.Count; i++)
        {
            var type = SkillEditorUtility.buffTypes[i];
            popups.Add(type.FullName);
        }
        for (int i = 0; i < SkillEditorUtility.eventTypes.Count; i++)
        {
            var type = SkillEditorUtility.eventTypes[i];
            popups.Add(type.FullName);
        }
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
        selectedIndex = EditorGUILayout.Popup(selectedIndex, popups.ToArray());
      //  tgType = (TimelingGroupType)EditorGUILayout.EnumPopup(tgType, GUILayout.MinWidth(100));
        GUILayout.EndHorizontal();
        int id_int = 0;
        if (!string.IsNullOrEmpty(id) && int.TryParse(id, out id_int) && !string.IsNullOrEmpty(skillname) && !string.IsNullOrEmpty(skillpath) && (GUILayout.Button("创建")))
        {
            string path = "";
            TimeLineGroup tg = null;
            JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            Type t = null;
            if (selectedIndex < SkillEditorUtility.skillTypes.Count)
            {
                tgType = TimelingGroupType.SKILL;
                t = SkillEditorUtility.skillTypes[selectedIndex];
            }
            else if (selectedIndex < (SkillEditorUtility.skillTypes.Count + SkillEditorUtility.buffTypes.Count))
            {
                tgType = TimelingGroupType.BUFF;
                t = SkillEditorUtility.buffTypes[selectedIndex - SkillEditorUtility.skillTypes.Count];
            }
            else
            {
                tgType = TimelingGroupType.EVENT;
                t = SkillEditorUtility.eventTypes[selectedIndex - SkillEditorUtility.skillTypes.Count - SkillEditorUtility.buffTypes.Count];
            }
            tg = Create(t);
            if (tgType == TimelingGroupType.SKILL)
            {
                path += "Skills/";
                path += "skill_"+skillpath+".bytes";
            }
            else if (tgType == TimelingGroupType.BUFF)
            {
                path += "Buffs/";
                path += "buff_" + skillpath + ".bytes";

            }
            else if (tgType == TimelingGroupType.EVENT)
            {
                path += "Events/";
                path += "event_"+skillpath + ".bytes";
            }
            string finalPath = SkillUtility.GetRequiredConfigsPath() + path;
            SkillUtility.SaveTimelineGroup(tg, finalPath);
            SkillUtility.SaveToSkillIndexFile(tg, path);
            skillWindow.OnCreate();
            Close();
        }
        GUILayout.EndVertical();
    }

    public TimeLineGroup Create(Type t)
    {
        TimeLineGroup tg = Activator.CreateInstance(t) as TimeLineGroup;
        tg.TimeLines.Add(new TimeLine());
        tg.TimeLines.Add(new TimeLine());
        SetName(tg);
        SetID(tg);
        SetPath(tg);
        return tg;
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
