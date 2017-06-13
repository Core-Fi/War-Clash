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
public class ETimelineGroupPanel : IEElement
{
    public TimeLineGroup timglineGroup { get; private set; }
    private List<ETimeline> eTimelines = new List<ETimeline>();
    private Texture2D setting;
    public ETimelineGroupPanel()
    {
        this.timglineGroup = SkillEditTempData.editingSkill;
        for (int i = 0; i < timglineGroup.TimeLines.Count; i++)
        {
            eTimelines.Add(new ETimeline(this, timglineGroup.TimeLines[i]));
        }
    }

    private void DrawGenericMenu()
    {
        if (GUILayout.Button(SkillEditTempData.settingTex, EditorStyles.toolbarButton, GUILayout.Width(20)))
        {
            GenericMenu toolsMenu = new GenericMenu();
            for (int j = 0; j < SkillEditorUtility.timeLineTypes.Count; j++)
            {
                int copyj = j;
                var display = SkillEditorUtility.GetDisplayAttr(SkillEditorUtility.timeLineTypes[j]);
                if (display != null)
                {
                    GenericMenu.MenuFunction func = () =>
                    {
                        var tl = Activator.CreateInstance(SkillEditorUtility.timeLineTypes[copyj]) as TimeLine;
                        timglineGroup.TimeLines.Add(tl);
                        eTimelines.Add(new ETimeline(this, tl));
                    };
                    toolsMenu.AddItem(new GUIContent("添加/" + display.DisplayName), false, func);
                    GenericMenu.MenuFunction pasteFunc = () =>
                    {
                        var tl = SkillEditTempData.copyItem as TimeLine;
                        timglineGroup.TimeLines.Add(tl);
                        eTimelines.Add(new ETimeline(this, tl));
                    };
                    if (SkillEditTempData.copyItem != null && SkillEditTempData.copyItem is TimeLine)
                        toolsMenu.AddItem(new GUIContent("粘贴"), false, pasteFunc);
                }
            }
            var e = UnityEngine.Event.current;
            toolsMenu.DropDown(new Rect(e.mousePosition.x, e.mousePosition.y, 0, 0));
            EditorGUIUtility.ExitGUI();
        }
    }

    public void RemoveTimeline(ETimeline eTimeline)
    {
        this.timglineGroup.TimeLines.Remove(eTimeline.timeline);
        this.eTimelines.Remove(eTimeline);
    }
    public void Draw()
    {
        Sort();
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal(GUILayout.MinWidth(50));
        if (GUILayout.Button((timglineGroup.Name + "__" + timglineGroup.ID), "Label"))
        {
            SkillEditTempData.editingItemCache = timglineGroup;
        }
        DrawGenericMenu();
        GUILayout.EndHorizontal();
        for (int i = 0; i < eTimelines.Count; i++)
        {
            eTimelines[i].Draw();
        }
        GUILayout.EndVertical();
    }

    private void Sort()
    {
        eTimelines.Sort((a, b) =>
        {
            return this.timglineGroup.TimeLines.IndexOf(a.timeline) - this.timglineGroup.TimeLines.IndexOf(b.timeline);
        });
    }
}
