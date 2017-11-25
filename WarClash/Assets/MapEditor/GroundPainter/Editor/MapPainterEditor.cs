using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Test0810))]
public class MapPainterEditor : Editor {

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        Test0810 t = target as Test0810;
        if (GUILayout.Button("Generate"))
        {
            t.Generate();
        }
        if (GUILayout.Button("材质1"))
        {
            Test0810.Type = 1;
        }
        if (GUILayout.Button("材质2"))
        {
            Test0810.Type = 2;
        }
        if (GUILayout.Button("Save"))
        {
            for (int i = 0; i < t.data.Length; i++)
            {
                if ((int) (t.data[i].r * 256) >= 15)
                {
                 //   t.data[i].r = UnityEngine.Random.Range(15, 23)/256f;
                }
            }
            t.tex.SetPixels(t.data);
            t.tex.Apply();
            var png = t.tex.EncodeToPNG();
            var jpg = t.tex.EncodeToJPG(100);
            File.WriteAllBytes(Application.dataPath + "/mapdata_jpg.jpg", jpg);
            File.WriteAllBytes(Application.dataPath+"/mapdata.png", png);
            var bytes = File.ReadAllBytes(Application.dataPath + "/mapdata.png");
            t.tex.LoadImage(bytes);
            t.tex.Apply(false);
        }
    }
    bool edit = false;
    public void OnSceneGUI()
    {
        Test0810 t = target as Test0810;
        if (Event.current.type == EventType.mouseDown)
        {
            edit = !edit;
        }
        if (edit && Event.current.type == EventType.MouseMove)
        {
            t.Raycast(true);
        }
    }
}
