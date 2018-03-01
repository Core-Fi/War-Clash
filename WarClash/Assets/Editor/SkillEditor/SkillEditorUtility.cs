using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using Event = UnityEngine.Event;
using EventType = UnityEngine.EventType;
using System;
using System.Reflection;
using System.Linq;
using Logic;
using Logic.LogicObject;
using Logic.Skill;
using Logic.Skill.Actions;

public class SkillEditorUtility
{
    public static List<Type> actionTypes = new List<Type>();
    public static List<Type> timeLineTypes = new List<Type>();
    public static List<Type> skillTypes = new List<Type>();
    public static List<Type> buffTypes = new List<Type>();
    public static List<Type> eventTypes = new List<Type>();
    public static string HandleCopyPaste(int controlID)
    {
        if (controlID == GUIUtility.keyboardControl)
        {
            if (Event.current.type == EventType.KeyUp && (Event.current.modifiers == EventModifiers.Control || Event.current.modifiers == EventModifiers.Command))
            {
                if (Event.current.keyCode == KeyCode.C)
                {
                    Event.current.Use();
                    TextEditor editor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
                    editor.Copy();
                }
                else if (Event.current.keyCode == KeyCode.V)
                {
                    Event.current.Use();
                    TextEditor editor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
                    editor.Paste();
                    return editor.text;
                }
            }
        }
        return null;
    }
    private static Type[] GetTypesInNamespace(System.Reflection.Assembly assembly, string nameSpace)
    {
        var types = assembly.GetTypes();
        return assembly.GetTypes().Where((t) => {
            if(t!=null && t.Namespace!=null)
                return t.Namespace.Contains(nameSpace);
            return false;
        }).ToArray();
    }
    public static Type[] GetAllClasses()
    {
        var assembly = System.Reflection.Assembly.GetAssembly(typeof(Logic.Skill.Skill));

        Type[] typelist = GetTypesInNamespace(assembly, "Logic.Skill");
        return typelist;
    }
    public static DisplayAttribute GetDisplayAttr(PropertyInfo fi)
    {
        var attrs = fi.GetCustomAttributes(typeof(DisplayAttribute), true);
        return attrs[0] as DisplayAttribute;
    }
    public static DisplayAttribute GetDisplayAttr(Type fi)
    {
        var attrs = fi.GetCustomAttributes(typeof(DisplayAttribute), true);
        if (attrs != null && attrs.Length > 0)
            return attrs[0] as DisplayAttribute;
        return null;
    }
    public static PropertyInfo GetPropertyInfo(object obj, string str)
    {
        var properties = obj.GetType().GetProperties();
        for (int i = 0; i < properties.Length; i++)
        {
            if (properties[i].Name == str)
            {
                return properties[i];
            }
        }
        return null;
    }
  
    public static void SetValue(object obj, string str, object val)
    {
        var properties = obj.GetType().GetProperties();
        for (int i = 0; i < properties.Length; i++)
        {
            if (properties[i].Name == str)
            {
                properties[i].SetValue(obj, val, null);
            }
        }
    }

    public static bool IsRunningSkill()
    {
        if (EditorApplication.isPlaying && !EditorApplication.isPaused && Selection.activeGameObject != null)
        {
            var oi = Selection.activeGameObject.GetComponent<LogicObject>();
            if (oi != null)
            {
                return true;
            }
        }
        return false;
    }
    public static object Clone(object t)
    {
        object t_clone = Activator.CreateInstance(t.GetType());
        Type type = t.GetType();
        Type type_clone = t_clone.GetType();
        FieldInfo[] infos = type.GetFields();
        PropertyInfo[] propertyInfos = type.GetProperties();
        for (int i = 0; i < propertyInfos.Length; i++)
        {
            PropertyInfo info = propertyInfos[i];
            object value = info.GetValue(t, null);
            if (value == null)
                continue;
            object value_clone = null;
            Type value_type = value.GetType();
            if (value is IList && value_type.IsGenericType)
            {
                Type[] genericTypes = value_type.GetGenericArguments();
                if (genericTypes.Length > 0)
                {
                    value_clone = Activator.CreateInstance(value_type);
                    IList valueList = value as IList;
                    foreach (var v in valueList)
                    {
                        (value_clone as IList).Add(Clone(v));
                    }
                }
            }
            else if (value is Array)
            {
                Array array = value as Array;
                value_clone = Activator.CreateInstance(value_type, array.Length);
                for (int j = 0; j < array.Length; j++)
                {
                    object val = array.GetValue(j);
                    (value_clone as Array).SetValue(val, j);
                }
            }
            else
            {
                value_clone = info.GetValue(t, null);
            }
            PropertyInfo info_clone = type_clone.GetProperty(info.Name);
            info_clone.SetValue(t_clone, value_clone, null);
        }
        for (int i = 0; i < infos.Length; i++)
        {
            FieldInfo info = infos[i];
            object value = info.GetValue(t);
            if (value == null)
                continue;
            object value_clone = null;
            Type value_type = value.GetType();
            if (value is IList && value_type.IsGenericType)
            {
                Type[] genericTypes = value_type.GetGenericArguments();
                if (genericTypes.Length > 0)
                {
                    value_clone = Activator.CreateInstance(value_type);
                    IList valueList = value as IList;
                    foreach (var v in valueList)
                    {
                        (value_clone as IList).Add(Clone(v));
                    }
                }
            }
            else
            {
                value_clone = info.GetValue(t);
            }
            FieldInfo info_clone = type_clone.GetField(info.Name);
            info_clone.SetValue(t_clone, value_clone);
        }
        return t_clone;
    }
}
