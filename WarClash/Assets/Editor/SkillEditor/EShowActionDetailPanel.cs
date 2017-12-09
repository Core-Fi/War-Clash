using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Lockstep;
using Logic;
using Logic.Skill;
using UnityEditor;
using UnityEngine;
public class EShowActionDetailPanel : IEElement
{
    private int space = 30;
    public TimeLine GetTimeLine()
    {
        var editingSkill = SkillEditTempData.editingSkill;
        object obj = SkillEditTempData.editingItem;
        for (int i = 0; i < editingSkill.TimeLines.Count; i++)
        {
            TimeLine _timeLine = editingSkill.TimeLines[i];
            for (int j = 0; j < _timeLine.BaseActions.Count; j++)
            {
                if (_timeLine.BaseActions[j] == obj)
                {
                    return _timeLine;
                }
            }
        }
        return null;
    }
    public void Draw()
    {
        if (SkillEditTempData.editingItem != null)
        {
            Dictionary<string, List<PropertyInfo>> group_items = new Dictionary<string, List<PropertyInfo>>();
            PropertyInfo[] fields = SkillEditTempData.editingItem.GetType().GetProperties();
            for (int i = 0; i < fields.Length; i++)
            {
                var displayAttributes = fields[i].GetCustomAttributes(typeof(DisplayAttribute), true) as DisplayAttribute[];//.FieldType.GetCustomAttributes<DisplayAttribute>();
                foreach (var displayAttribute in displayAttributes)
                {
                    if (!group_items.ContainsKey(displayAttribute.GroupName))
                    {
                        group_items.Add(displayAttribute.GroupName, new List<PropertyInfo>());
                    }
                    group_items[displayAttribute.GroupName].Add(fields[i]);
                }
            }
            foreach (var groupItem in group_items)
            {
                GUILayout.Label(groupItem.Key);
                for (int i = 0; i < groupItem.Value.Count; i++)
                {
                    PropertyInfo fi = groupItem.Value[i];
                    var displayAttr = SkillEditorUtility.GetDisplayAttr(fi);
                    GUILayout.BeginHorizontal(GUILayout.MinWidth(100));
                    GUILayout.Space(space);
                    GUILayout.Label(displayAttr.DisplayName);
                    GUILayout.Space(space);
                    if (displayAttr.ControlType == UIControlType.Range)
                    {
                        TimeLine tl = GetTimeLine();
                        var val = fi.GetValue(SkillEditTempData.editingItem, null);
                        var new_val = GUILayout.HorizontalSlider((int)val, 0, tl.FrameCount, GUILayout.MinWidth(100), GUILayout.MaxWidth(200), GUILayout.ExpandWidth(false));
                        fi.SetValue(SkillEditTempData.editingItem, (int)new_val, null);
                        GUILayout.Label(new_val.ToString("0.00"));
                    }
                    else if (displayAttr.ControlType == UIControlType.MutiSelection)
                    {
                        System.Array enumValues = System.Enum.GetValues(displayAttr.data as Type);
                        GUILayout.BeginVertical();
                        foreach (var item in enumValues)
                        {
                            int val = (int)item;
                            int storedVal = (int)fi.GetValue(SkillEditTempData.editingItem, null);
                            bool check = (storedVal & val) != 0;
                            bool new_check = GUILayout.Toggle(check, item.ToString(), GUILayout.MinWidth(70));
                            if (new_check != check)
                            {
                                if (new_check)
                                {
                                    storedVal += val;
                                }
                                else
                                {
                                    storedVal -= val;
                                }
                                fi.SetValue(SkillEditTempData.editingItem, storedVal, null);
                            }
                        }
                        GUILayout.EndVertical();
                    }
                    else if (displayAttr.ControlType == UIControlType.Default)
                    {
                        if (fi.PropertyType == typeof(string))
                        {
                            string val = fi.GetValue(SkillEditTempData.editingItem, null) as string;
                            val = val == null ? "" : val;
                            string new_val = GUILayout.TextField(val, GUILayout.MinWidth(70));
                            fi.SetValue(SkillEditTempData.editingItem, new_val, null);
                        }
                        else if (fi.PropertyType == typeof(int))
                        {
                            int val = (int)fi.GetValue(SkillEditTempData.editingItem, null);
                            string new_val = GUILayout.TextField(val.ToString(), GUILayout.MinWidth(70));
                            if (int.TryParse(new_val, out val))
                            {
                                fi.SetValue(SkillEditTempData.editingItem, val, null);
                            }
                        }
                        else if (fi.PropertyType == typeof(bool))
                        {
                            bool val = (bool)fi.GetValue(SkillEditTempData.editingItem, null);
                            bool new_val = GUILayout.Toggle(val, "", GUILayout.MinWidth(70));
                            fi.SetValue(SkillEditTempData.editingItem, new_val, null);

                        }
                        else if (fi.PropertyType == typeof(float))
                        {
                            float val = (float)fi.GetValue(SkillEditTempData.editingItem, null);
                            string new_val = GUILayout.TextField(val.ToString(), GUILayout.MinWidth(70));
                            if (new_val != val.ToString() && float.TryParse(new_val, out val))
                            {
                                fi.SetValue(SkillEditTempData.editingItem, val, null);
                            }
                        }
                        else if (fi.PropertyType.IsEnum)
                        {
                            var val = fi.GetValue(SkillEditTempData.editingItem, null) as Enum;
                            var new_val = EditorGUILayout.EnumPopup("", val, GUILayout.MinWidth(70));
                            fi.SetValue(SkillEditTempData.editingItem, new_val, null);

                        }
                        else if (fi.PropertyType == typeof(FixedAnimationCurve))
                        {
                            var val = fi.GetValue(SkillEditTempData.editingItem, null) as FixedAnimationCurve;
                            if (val == null)
                            {
                                val = new FixedAnimationCurve();
                            }
                            AnimationCurve ac = new AnimationCurve();
                            if (val.Keyframes.Count>0)
                            {
                                Keyframe[] keyframes = new Keyframe[val.Keyframes.Count];
                                for (int j = 0; j < val.Keyframes.Count; j++)
                                {
                                    var ori_key = val.Keyframes[j];
                                    keyframes[j] = new Keyframe(ori_key.time.ToFloat(), ori_key.value.ToFloat(), ori_key.inTangent.ToFloat(), ori_key.outTangent.ToFloat());
                                    keyframes[j].tangentMode = ori_key.tangentMode;
                                }
                                ac.keys = keyframes;
                            }
                            var new_val = EditorGUILayout.CurveField("", ac);
                            val.Keyframes.Clear();
                            List<FixedKeyFrame> ckfs = new List<FixedKeyFrame>();
                            for (int j = 0; j < new_val.keys.Length; j++)
                            {
                                Keyframe kf = new_val.keys[j];
                                val.AddKeyFrame( new FixedKeyFrame(){ time = kf.time.ToLong(), value = kf.value.ToLong(), inTangent = kf.inTangent.ToLong(), outTangent = kf.outTangent.ToLong(),
                                    tangentMode = kf.tangentMode });
                            }
                            fi.SetValue(SkillEditTempData.editingItem, val, null);
                        }
                        else if (fi.PropertyType == typeof(List<EventTrigger>))
                        {
                            GUILayout.BeginVertical();
                            var val = fi.GetValue(SkillEditTempData.editingItem, null) as List<EventTrigger>;
                            if (GUILayout.Button("添加事件", GUILayout.MaxWidth(70)))
                            {
                                val.Add(new EventTrigger());
                            }
                            for (int j = 0; j < val.Count; j++)
                            {
                                GUILayout.BeginHorizontal();
                                DisplayAttribute event_display_attr = SkillEditorUtility.GetDisplayAttr(SkillEditorUtility.GetPropertyInfo(val[j], "e"));
                                GUILayout.Label(event_display_attr.DisplayName);
                                var res = (Logic.Skill.EventType)EditorGUILayout.EnumPopup("", val[j].e, GUILayout.MinWidth(70));
                                if ((int)res < (int)Logic.Skill.EventType.MEELEEWEAPONHIT)
                                {
                                    res = Logic.Skill.EventType.MEELEEWEAPONHIT;
                                }
                                SkillEditorUtility.SetValue(val[j], "e", res);
                                GUILayout.EndHorizontal();
                                GUILayout.BeginHorizontal();
                                DisplayAttribute path_display_attr = SkillEditorUtility.GetDisplayAttr(SkillEditorUtility.GetPropertyInfo(val[j], "path"));
                                GUILayout.Label(path_display_attr.DisplayName);
                                var path = GUILayout.TextField(val[j].EventId.ToString(), GUILayout.MinWidth(70));
                                SkillEditorUtility.SetValue(val[j], "path", path);
                                GUILayout.Space(space);
                                if (GUILayout.Button("删除"))
                                {
                                    val.RemoveAt(j);
                                }
                                GUILayout.EndHorizontal();
                            }
                            fi.SetValue(SkillEditTempData.editingItem, val, null);
                            GUILayout.EndVertical();
                        }
                        else if (fi.PropertyType.Name.Contains("DataBind"))
                        {
                            object val = fi.GetValue(SkillEditTempData.editingItem, null);
                            Type db_type = val.GetType();
                            var db_properites = db_type.GetProperties((BindingFlags)int.MaxValue);
                            bool needBind = false;
                            for (int j = 0; j < db_properites.Length; j++)
                            {
                                if (db_properites[j].Name.Contains("needDataBind"))
                                {
                                    needBind = (bool)(db_properites[j].GetValue(val, null));
                                    GUILayout.Label("数据绑定");
                                    bool new_needBind = GUILayout.Toggle(needBind, "");
                                    if (new_needBind != needBind)
                                    {
                                        db_properites[j].SetValue(val, new_needBind, null);
                                    }
                                    break;
                                }
                            }
                            if (needBind)
                            {
                                for (int j = 0; j < db_properites.Length; j++)
                                {
                                    if (db_properites[j].Name.Contains("bindFrom"))
                                    {
                                        GUILayout.Label("数据源字段");
                                        string bindFrom = (string)(db_properites[j].GetValue(val, null));
                                        if (bindFrom == null)
                                        {
                                            bindFrom = "";
                                        }
                                        string new_bindFrom = GUILayout.TextField(bindFrom, GUILayout.Width(50));
                                        if (bindFrom != new_bindFrom)
                                        {
                                            db_properites[j].SetValue(val, new_bindFrom, null);
                                        }
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                for (int j = 0; j < db_properites.Length; j++)
                                {
                                    if (db_properites[j].Name.Contains("value_SetDirectly"))
                                    {
                                        GUILayout.Label("数据");
                                        var genericTypes = fi.PropertyType.GetGenericArguments();
                                        if (genericTypes[0] == typeof(int))
                                        {
                                            int bindFrom = (int)(db_properites[j].GetValue(val, null));
                                            string new_bindFrom_str = GUILayout.TextField(bindFrom.ToString(),
                                                GUILayout.Width(50));
                                            int new_bindFrom;
                                            if (int.TryParse(new_bindFrom_str, out new_bindFrom))
                                            {
                                                if (bindFrom != new_bindFrom)
                                                {
                                                    db_properites[j].SetValue(val, new_bindFrom, null);
                                                }
                                            }
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    GUILayout.EndHorizontal();
                }
            }
        }   
    }
}
