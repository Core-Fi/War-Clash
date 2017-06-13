using System;
using UnityEngine;
using System.Collections;
using Logic.Skill.Actions;
using UnityEditor;

public class EActionControl : EControl
{
    public ETimeline parent;
    public BaseAction action;
    public EActionControl(ETimeline parent, BaseAction action)
    {
        this.parent = parent;
        this.action = action;
    }

    public void DrawGenericMenu()
    {
        if (GUILayout.Button(SkillEditTempData.settingTex, EditorStyles.toolbarButton, GUILayout.MaxWidth(20)))
        {
            GenericMenu toolsMenu = new GenericMenu();
            GenericMenu.MenuFunction func = () =>
            {
                if (action == SkillEditTempData.editingItem)
                {
                    SkillEditTempData.editingItem = null;
                }
                parent.RemoveAction(this);
            };
            toolsMenu.AddItem(new GUIContent("删除"), false, func);
            GenericMenu.MenuFunction copyFunc = () =>
            {
                SkillEditTempData.copyItem = SkillEditorUtility.Clone(action);
            };
            toolsMenu.AddItem(new GUIContent("复制"), false, copyFunc);
            Event e = Event.current;
            toolsMenu.DropDown(new Rect(e.mousePosition.x, e.mousePosition.y, 0, 0));
            EditorGUIUtility.ExitGUI();
        }
    }
    public override void Draw()
    {
        GUILayout.BeginHorizontal(GUILayout.MinWidth(400));
        GUILayout.Space(30*2);
        GUI.color = Color.white;
        int val = (int)GUILayout.HorizontalSlider((float)action.ExecuteFrameIndex, 0, parent.timeline.FrameCount, GUILayout.MinWidth(150));
        if (val != action.ExecuteFrameIndex)
        {
            SkillEditorUtility.SetValue(action, "ExecuteFrameIndex", val);
        }
        GUI.color = Color.white;
        DrawGenericMenu();
        GUI.color = Color.white;
        var attr = SkillEditorUtility.GetDisplayAttr(action.GetType());
        if (GUILayout.Button(attr.DisplayName + "  调用帧数" + action.ExecuteFrameIndex.ToString("000"), "Label"))
        {
            SkillEditTempData.editingItemCache = action;
        }
        GUI.color = Color.white;
        GUILayout.EndHorizontal();
        base.Draw();
    }
}
