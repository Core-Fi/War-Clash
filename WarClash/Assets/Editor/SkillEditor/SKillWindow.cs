using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using Event = UnityEngine.Event;
using EventType = UnityEngine.EventType;
using System;
using System.Reflection;
using Logic;
using Logic.LogicObject;
using Logic.Skill;
using Logic.Skill.Actions;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;

public class FileInfo
{
    public bool foldout;
    public string name;
    public FileInfo parent;
    public List<FileInfo> files = new List<FileInfo>();
}
public enum FileType
{
    Skill,
    Buff,
    Event,
    Invalid
}

public class SKillWindow : EditorWindow
{
    [MenuItem("Window/技能编辑")]
    static void Init()
    {
        SKillWindow window = (SKillWindow)EditorWindow.GetWindow(typeof(SKillWindow));
        window.Show();
    }
    private Vector2 posLeft = Vector2.zero;
    private Vector2 posUpperRight;
    private Vector2 posDownerRight;
    private List<FileInfo> fileInfos = new List<FileInfo>();
    private float splitterWidth = 4;
    private float leftSplitterPos;
    private float rightSplitterPos;
    private Rect leftSplitterRect;
    private Rect rightSplitterRect;
    private bool verticalLeftdragging;
    private bool verticalRightdragging;
    private TimeLineGroup editingSkill;
    /// <summary>
    /// 技能节点的拖拽
    /// </summary>
    private object dragingItem;
    private object copyItem;
    private FileInfo editingFileInfo;
    private int space = 30;
    private bool isPlaying;
    private ETimelineGroupPanel _timelineGroupPanel;
    private ERuntimeOperatePanel _runtimeOperatePanel;
    private EShowActionDetailPanel _actionDetailPanel;
   
    public void Initiate()
    {
        Type[] typelist = SkillEditorUtility.GetAllClasses();
        SkillEditorUtility.timeLineTypes.Clear();
        SkillEditorUtility.actionTypes.Clear();
        foreach (Type t in typelist)
        {
            if (typeof(BaseAction).IsAssignableFrom(t))
            {
                SkillEditorUtility.actionTypes.Add(t);
            }
            else if (typeof(TimeLine).IsAssignableFrom(t))
            {
                SkillEditorUtility.timeLineTypes.Add(t);
            }
        }
        fileInfos.Clear();
        _runtimeOperatePanel = new ERuntimeOperatePanel();
        _actionDetailPanel = new EShowActionDetailPanel();
        GetFiles(Application.streamingAssetsPath , fileInfos, null);
        SkillEditTempData.settingTex = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Editor/Image/setting.png");
    }

   
    void OnGUI()
    {
        if (Event.current.type == EventType.Layout && SkillEditTempData.editingItemCache != null)//unity bug 在非layout情况下添加新控件报错
        {
            SkillEditTempData.editingItem = SkillEditTempData.editingItemCache;
            SkillEditTempData.editingItemCache = null;
        }
        GUILayout.BeginHorizontal();
        posLeft = GUILayout.BeginScrollView(posLeft, GUILayout.Width(leftSplitterPos), GUILayout.Height(this.position.height)
            ,GUILayout.MinWidth(150));
        GUILayout.BeginHorizontal();
        ShowBar();
        GUILayout.EndHorizontal();
        ShowFiles(fileInfos);
        GUILayout.EndScrollView();
        GUILayout.Box("",GUILayout.Width(splitterWidth),GUILayout.MaxWidth(splitterWidth),GUILayout.MinWidth(splitterWidth),GUILayout.ExpandHeight(true));
        leftSplitterRect = GUILayoutUtility.GetLastRect();
        int middleWidth = (int)(rightSplitterPos - leftSplitterPos);
        middleWidth = Mathf.Clamp(middleWidth, 300, 600);
        GUILayout.BeginVertical(GUILayout.Width(middleWidth));
        GUILayout.BeginHorizontal(GUILayout.MinWidth(200));
        if (EditorApplication.isPlaying && !EditorApplication.isPaused && Selection.activeGameObject != null)
        {
            ShowOperateBar();
        } 
        GUILayout.EndHorizontal();
       
        posUpperRight = GUILayout.BeginScrollView(posUpperRight, GUILayout.Height(this.position.height / 2),GUILayout.ExpandWidth(true));
        if (_timelineGroupPanel != null )
        {
            _timelineGroupPanel.Draw();
        }
        GUILayout.EndScrollView();
        GUILayout.Box("", GUILayout.Width(splitterWidth), GUILayout.Height(splitterWidth), GUILayout.ExpandWidth(true));
        posDownerRight = GUILayout.BeginScrollView(posDownerRight, GUILayout.Height(this.position.height / 2), GUILayout.ExpandWidth(true));
        if (SkillEditTempData.editingItem != null && _actionDetailPanel!=null)
        {
            _actionDetailPanel.Draw();
        }
        GUILayout.EndScrollView();
        GUILayout.EndVertical();
        GUILayout.Box("", GUILayout.Width(splitterWidth), GUILayout.MaxWidth(splitterWidth), GUILayout.MinWidth(splitterWidth), GUILayout.ExpandHeight(true));
        rightSplitterRect = GUILayoutUtility.GetLastRect();
        GUILayout.BeginVertical(GUILayout.MinWidth(100));
        if (_runtimeOperatePanel != null)
        {
            _runtimeOperatePanel.Draw();
        }
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
        #region MouseEvent
        if (Event.current != null)
        {
            switch (Event.current.type)
            {
                case EventType.MouseDown:
                    if (leftSplitterRect.Contains(Event.current.mousePosition))
                    {
                        verticalLeftdragging = true;
                    }
                    else if (rightSplitterRect.Contains(Event.current.mousePosition))
                    {
                        verticalRightdragging = true;
                    }
                    break;
                case EventType.MouseDrag:
                    if (verticalLeftdragging)
                    {
                        leftSplitterPos = Event.current.mousePosition.x;
                        leftSplitterPos = Mathf.Clamp(leftSplitterPos, 150, 280);
                        Repaint();
                    }
                    else if (verticalRightdragging)
                    {
                        rightSplitterPos = Event.current.mousePosition.x;
                        Repaint();
                       
                    }
                    break;
                case EventType.MouseUp:
                    if (verticalLeftdragging)
                    {
                        leftSplitterPos = Event.current.mousePosition.x;
                        leftSplitterPos = Mathf.Clamp(leftSplitterPos, 150, 280);
                        verticalLeftdragging = false;
                    }
                    if (verticalRightdragging)
                    {
                        rightSplitterPos = Event.current.mousePosition.x;
                        verticalRightdragging = false;
                    }
                    break;
            }
        }
        #endregion
    }

    public void OnCreate()
    {
        Initiate();
        SkillEditTempData.editingSkill = null;
        SkillEditTempData.editingItem = null;
        editingFileInfo = null;
    }
    private void ShowOperateBar()
    {
        if (SkillEditTempData.editingSkill != null && GUILayout.Button("Play"))
        {
            var oi = Selection.activeGameObject.GetComponent<LogicObject>();
            if (oi)
            {
                var so = LogicCore.SP.SceneManager.currentScene.GetObject<Character>(oi.ID);
                so.ReleaseSkill(editingFileInfo.name);
            }
        }
        if (SkillEditTempData.editingSkill != null && GUILayout.Button("Pause"))
        {
            var oi = Selection.activeGameObject.GetComponent<LogicObject>();
            if (oi)
            {
                var so = LogicCore.SP.SceneManager.currentScene.GetObject<Character>(oi.ID);
                so.CancelSkill();
            }
        }
    }
    private void ShowBar()
    {
        if (GUILayout.Button("刷新"))
        {
            Initiate();
        }
        if(GUILayout.Button("新建"))
        {
            CreateSkillWindow window = (CreateSkillWindow)EditorWindow.GetWindow(typeof(CreateSkillWindow));
            window.Show();
            window.skillWindow = this;
        }
        if (SkillEditTempData.editingSkill != null && GUILayout.Button("保存"))
        {
            Save();
        }
    }
    private void HierachyCout(FileInfo fi, ref int count)
    {
        if(fi.parent!=null)
        {
            count++;
            HierachyCout(fi.parent, ref count);
        }
    }
    private void ShowFiles(List<FileInfo> fileInfos)
    {
        for (int i = 0; i < fileInfos.Count; i++)
        {
            int count = 0;
            HierachyCout(fileInfos[i], ref count);
            if (IsSkillFile(fileInfos[i]))
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(10*count);
                if (editingFileInfo == fileInfos[i])
                {
                    GUI.color = Color.gray;
                }
                if (GUILayout.Button(Path.GetFileName(fileInfos[i].name), "Label"))
                {
                    SkillEditTempData.editingSkill = Logic.Skill.SkillUtility.GetTimelineGroupFullPath<TimeLineGroup>(fileInfos[i].name);
                    _timelineGroupPanel = new ETimelineGroupPanel();
                    editingFileInfo = fileInfos[i];
                    SkillEditTempData.editingItem = null;
                }
                GUI.color = Color.white;
                GUILayout.EndHorizontal();
            }
            else
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(10 * count);
                fileInfos[i].foldout = EditorGUILayout.Foldout(fileInfos[i].foldout, Path.GetFileName(fileInfos[i].name));
                GUILayout.EndHorizontal();
                if (fileInfos[i].foldout)
                {
                    ShowFiles(fileInfos[i].files);
                }
            }
        }
    }
    bool IsSkillFile(FileInfo fi)
    {
        return fi.files.Count == 0 && IsSkillFile(fi.name);
    }
    bool IsSkillFile(string path)
    {
        string ext = Path.GetExtension(path);
        return (ext == ".skill" || ext == ".buff" || ext == ".event");
    }
    void GetFiles(string basePath, List<FileInfo> fileInfos, FileInfo parent)
    {
        var buff_directories = Directory.GetDirectories(basePath);
        var files = Directory.GetFiles(basePath);
        for (int i = 0; i < buff_directories.Length; i++)
        {
            FileInfo fi = new FileInfo() {
                foldout = false,
                name = buff_directories[i]
            };
            fileInfos.Add(fi);
            fi.parent = parent;
            GetFiles(buff_directories[i], fi.files, fi);
        }
        for (int j = 0; j < files.Length; j++)
        {
            if (IsSkillFile(files[j]))
            {
                FileInfo fi = new FileInfo()
                {
                    foldout = false,
                    name = files[j]
                };
                fi.parent = parent;
                fileInfos.Add(fi);
            }
        }
    }
    private void Save()
    {
        string ext = Path.GetExtension(editingFileInfo.name);
        for (int i = 0; i < SkillEditTempData.editingSkill.TimeLines.Count; i++)
        {
            var tl = SkillEditTempData.editingSkill.TimeLines[i];
            tl.BaseActions.Sort((a, b) => { return (int)(a.ExecuteFrameIndex * 100) - (int)(b.ExecuteFrameIndex * 100); });
        }
        Logic.Skill.SkillUtility.SaveToSkillIndexFile(SkillEditTempData.editingSkill, editingFileInfo.name.Replace(Application.streamingAssetsPath,""));
        Logic.Skill.SkillUtility.SaveTimelineGroup(SkillEditTempData.editingSkill, editingFileInfo.name);
        this.ShowNotification(new GUIContent("保存成功"));
    }
}
