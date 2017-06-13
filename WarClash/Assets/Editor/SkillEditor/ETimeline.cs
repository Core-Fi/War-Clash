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
public class ETimeline : IEElement
{
    public ETimelineGroupPanel parent;
    public TimeLine timeline;
    public List<EActionControl> list = new List<EActionControl>();
    public ETimeline(ETimelineGroupPanel parent, TimeLine timeline)
    {
        this.parent = parent;
        this.timeline = timeline;
        for (int i = 0; i < timeline.BaseActions.Count; i++)
        {
            list.Add(new EActionControl(this, timeline.BaseActions[i]));
        }
    }
    public void DrawGenericMenu()
    {
        if (GUILayout.Button(SkillEditTempData.settingTex, EditorStyles.toolbarButton, GUILayout.MaxWidth(20)))
        {
            int cur_tl_index = parent.timglineGroup.TimeLines.IndexOf(timeline);
            GenericMenu toolsMenu = new GenericMenu();
            GenericMenu.MenuFunction moveUpFunc = () =>
            {
                var tempTl = this.timeline;

                parent.timglineGroup.TimeLines[cur_tl_index] = parent.timglineGroup.TimeLines[cur_tl_index - 1];
                parent.timglineGroup.TimeLines[cur_tl_index - 1] = tempTl;
            };
            if (cur_tl_index != 0)
                toolsMenu.AddItem(new GUIContent("向上"), false, moveUpFunc);
            GenericMenu.MenuFunction moveDownFunc = () =>
            {
                var tempTl = parent.timglineGroup.TimeLines[cur_tl_index];
                parent.timglineGroup.TimeLines[cur_tl_index] = parent.timglineGroup.TimeLines[cur_tl_index + 1];
                parent.timglineGroup.TimeLines[cur_tl_index + 1] = tempTl;
            };
            if (cur_tl_index != parent.timglineGroup.TimeLines.Count - 1)
                toolsMenu.AddItem(new GUIContent("向下"), false, moveDownFunc);
            GenericMenu.MenuFunction copyFunc = () =>
            {
                SkillEditTempData.copyItem = SkillEditorUtility.Clone(parent.timglineGroup.TimeLines[cur_tl_index]);
            };
            toolsMenu.AddItem(new GUIContent("复制"), false, copyFunc);

            GenericMenu.MenuFunction pasteFunc = () =>
            {
                timeline.BaseActions.Add(SkillEditTempData.copyItem as BaseAction);
                list.Add(new EActionControl(this, SkillEditTempData.copyItem as BaseAction));
            };
            if (SkillEditTempData.copyItem != null && SkillEditTempData.copyItem is BaseAction)
                toolsMenu.AddItem(new GUIContent("粘贴"), false, pasteFunc);
            for (int j = 0; j < SkillEditorUtility.actionTypes.Count; j++)
            {
                int copyj = j;
                var display = SkillEditorUtility.GetDisplayAttr(SkillEditorUtility.actionTypes[j]);
                if (display != null)
                {
                    GenericMenu.MenuFunction func = () =>
                    {
                        var new_action = Activator.CreateInstance(SkillEditorUtility.actionTypes[copyj]) as BaseAction;
                        timeline.BaseActions.Add(new_action); 
                        this.list.Add(new EActionControl(this, new_action));
                    };
                    if (typeof(DisplayAction).IsAssignableFrom(SkillEditorUtility.actionTypes[j]))
                    {
                        toolsMenu.AddItem(new GUIContent("添加/表现/" + display.DisplayName), false, func);
                    }
                    else
                    {
                        toolsMenu.AddItem(new GUIContent("添加/逻辑/" + display.DisplayName), false, func);
                    }
                }
            }
            GenericMenu.MenuFunction del_func = () =>
            {
                if (SkillEditTempData.editingItem == timeline)
                {
                    SkillEditTempData.editingItem = null;
                }
                parent.RemoveTimeline(this);
            };
            toolsMenu.AddItem(new GUIContent("删除"), false, del_func);
            Event e = Event.current;
            toolsMenu.DropDown(new Rect(e.mousePosition.x, e.mousePosition.y, 0, 0));
            EditorGUIUtility.ExitGUI();
        }
    }
    public void Draw()
    {
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal(GUILayout.MinWidth(150));
        DisplayAttribute displayAttr = SkillEditorUtility.GetDisplayAttr(timeline.GetType());
        GUILayout.Space(30);
        if (GUILayout.Button((displayAttr.DisplayName + "   时长:" + timeline.FrameCount + "  次数:" + timeline.Times), "Label"))
        {
            SkillEditTempData.editingItemCache = timeline;
           // OnSelected.Invoke(timeline);
        }
        DrawGenericMenu();
        GUILayout.EndHorizontal();
        for (int i = 0; i < list.Count; i++)
        {
            if(list[i].action.GetType().Equals(typeof(BaseAction)))
            {
            }else
                list[i].Draw();
        }
        GUILayout.EndVertical();
    }

    public void RemoveAction(EActionControl action)
    {
        this.timeline.BaseActions.Remove(action.action);
        this.list.Remove(action);
    }
}
