using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.WSA;
using Application = UnityEngine.Application;


public class CreateUIClassEditor : EditorWindow
{
    [MenuItem("Tools/CreateUIClass")]
    static void Init()
    {
        CreateUIClassEditor window = (CreateUIClassEditor)EditorWindow.GetWindow(typeof(CreateUIClassEditor));
        window.Show();

    }

    private string Path = "/Scripts/UI/TemplateUI.cs";
    private StringBuilder declaration;
    private StringBuilder assignValue;
    private StringBuilder classText;
    void OnGUI()
    {
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
       // GUILayout.Label("路径");
        //Path = GUILayout.TextField(Path, GUILayout.MinWidth(100), GUILayout.MinHeight(30));
        GUILayout.EndHorizontal();
        if (GUILayout.Button("Generate"))
        {
            CreateClass(out declaration, out assignValue, out classText);
            //if (File.Exists(Application.dataPath + Path))
            //{
            //    if (EditorUtility.DisplayDialog("", "已经存在文件是否覆盖", "好的"))
            //    {
            //        CreateClass();
            //    }
            //}
            //else
            //{
            //    CreateClass();
            //}
        }
        if (declaration != null)
        {
            GUILayout.TextField(declaration.ToString());
            if (GUILayout.Button("复制"))
            {
                EditorGUIUtility.systemCopyBuffer = declaration.ToString();
            }
            GUILayout.TextField(assignValue.ToString());
            if (GUILayout.Button("复制"))
            {
                EditorGUIUtility.systemCopyBuffer = assignValue.ToString();
            }
            GUILayout.TextField(classText.ToString());
            if (GUILayout.Button("复制"))
            {
                EditorGUIUtility.systemCopyBuffer = classText.ToString();
            }
        }
        GUILayout.EndVertical();
    }
    public void CreateClass(out StringBuilder declaration, out StringBuilder assignValue, out StringBuilder sb)
    {
        var go =  Selection.activeObject as GameObject;
        declaration = new StringBuilder();
        assignValue = new StringBuilder();
        sb = new StringBuilder();
        if (go != null)
        {
            Dictionary<Transform, string> c_path = new Dictionary<Transform, string>();
            GetChildren(go.transform, c_path, "");
            HashSet<string> types = new HashSet<string>();
            string className = System.IO.Path.GetFileNameWithoutExtension(Path);
            types.Add(typeof(Text).FullName);
            types.Add(typeof(Image).FullName);
            types.Add(typeof(Button).FullName);
            types.Add(typeof(Toggle).FullName);
            types.Add(typeof(Slider).FullName);
            types.Add(typeof(HorizontalLayoutGroup).FullName);
            types.Add(typeof(VerticalLayoutGroup).FullName);

            sb.AppendLine("using UnityEngine;");
            sb.AppendLine("using UnityEngine.UI;");

            sb.AppendLine("public class "+ className + " : UITemplate");
            sb.AppendLine("{");
            sb.AppendLine("public " + className + "(GameObject go) : base(go)");
            sb.AppendLine("{}");
            foreach (var cp in c_path)
            {
                if (!string.IsNullOrEmpty(cp.Value))
                {
                    sb.AppendLine("public GameObject " + cp.Key.name + "_go;");
                    declaration.AppendLine("public GameObject " + cp.Key.name + "_go;");
                    if (cp.Value.Contains("template_"))
                    {
                        continue;
                    }
                    var comps = cp.Key.gameObject.GetComponents<Component>();
                    for (int i = 0; i < comps.Length; i++)
                    {
                        var c = comps[i];
                        var t = c.GetType();
                        if (!types.Contains(t.FullName))
                        {
                            continue;
                        }
                        var s = t.FullName.Split('.');
                        sb.AppendLine("public " + t.FullName + " " + cp.Key.name + "_" + s[s.Length - 1].ToLower() +";");
                        declaration.AppendLine("public " + t.FullName + " " + cp.Key.name + "_" +
                                               s[s.Length - 1].ToLower() + ";");
                        break;
                    }
                }
                else if (cp.Value.Equals(""))
                {
                    var comps = cp.Key.gameObject.GetComponents<Component>();
                    for (int i = 0; i < comps.Length; i++)
                    {
                        var c = comps[i];
                        if(c == null)
                            continue;
                        var t = c.GetType();
                        if (!types.Contains(t.FullName))
                        {
                            continue;
                        }
                        var s = t.FullName.Split('.');
                        sb.AppendLine(t.FullName + " " + cp.Key.name + "_" + s[s.Length - 1].ToLower() + ";");
                        declaration.AppendLine(t.FullName + " " + cp.Key.name + "_" + s[s.Length - 1].ToLower() + ";");
                        break;
                    }
                }
            }
            sb.AppendLine("protected override void OnInit()");
            sb.AppendLine("{");
           
            foreach (var cp in c_path)
            {
                if (!string.IsNullOrEmpty(cp.Value))
                {
                    sb.AppendLine(cp.Key.name + @"_go = Go.transform.Find(""" + cp.Value + @""").gameObject;");
                    assignValue.AppendLine(cp.Key.name + @"_go = Go.transform.Find(""" + cp.Value +
                                           @""").gameObject;");
                    if (cp.Value.Contains("template_"))
                    {
                        continue;
                    }
                    var comps = cp.Key.gameObject.GetComponents<Component>();
                    for (int i = 0; i < comps.Length; i++)
                    {
                       
                        var c = comps[i];
                        var t = c.GetType();
                        if (!types.Contains(t.FullName))
                        {
                            continue;
                        }
                        var s = t.FullName.Split('.');
                        sb.AppendLine( cp.Key.name + "_" + s[s.Length - 1].ToLower() + " = " +
                                      cp.Key.name + "_go.GetComponent<" + t.FullName + ">();");
                        assignValue.AppendLine(cp.Key.name + "_" + s[s.Length - 1].ToLower() + " = " +
                                               cp.Key.name + "_go.GetComponent<" + t.FullName + ">();");
                        break;
                    }
                }
                else 
                {
                    var comps = cp.Key.gameObject.GetComponents<Component>();
                    for (int i = 0; i < comps.Length; i++)
                    {
                        var c = comps[i];
                        if (c == null)
                            continue;
                        var t = c.GetType();
                        if (!types.Contains(t.FullName))
                        {
                            continue;
                        }
                        var s = t.FullName.Split('.');
                        sb.AppendLine(cp.Key.name + "_" + s[s.Length - 1].ToLower() + " = Go.GetComponent<" + t.FullName + ">();");
                        assignValue.AppendLine(cp.Key.name + "_" + s[s.Length - 1].ToLower() + " = Go.GetComponent<" + t.FullName + ">();");
                        break;
                    }
                }
            }
            sb.AppendLine("}");
            sb.AppendLine("}");

        }
    }

    private static void GetChildren(Transform p, Dictionary<Transform, string> cp, string path)
    {
        Transform tp = null;
        if (path.Equals(""))
        {
            tp = p;
        }
        else
        {
            tp = p.Find(path);
        }
        if (tp.name.Contains("m_") || string.IsNullOrEmpty(path))
        {
            cp.Add(tp, path);
        }
        for (int i = 0; i < tp.childCount; i++)
        {
            var c = tp.GetChild(i);
            if (!c.name.Contains("template_"))
            {
                if (string.IsNullOrEmpty(path))
                {
                    GetChildren(p, cp, c.name);
                }
                else
                {
                    GetChildren(p, cp, path + "/" + c.name);
                }
            }
            else
            {
                if (string.IsNullOrEmpty(path))
                {
                    cp.Add(c, c.name);
                }
                else
                {
                    cp.Add(c, path + "/" + c.name);
                }
            }
        }
    }
}
