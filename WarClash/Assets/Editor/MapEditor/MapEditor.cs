using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MapEditor : EditorWindow {

    [MenuItem("Window/地图编辑")]
    static void Init()
    {
        MapEditor window = (MapEditor)EditorWindow.GetWindow(typeof(MapEditor));
        window.Show();
    }

    public int width;
    public int height;
    public float cellwidth;
    public float cellheight;
    
    void OnGUI()
    {
        string temp ;
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        GUILayout.Label("宽度");
        temp = GUILayout.TextField(width.ToString(), GUILayout.MinWidth(100));
        width = int.Parse(temp);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("高度");
        temp = GUILayout.TextField(height.ToString(), GUILayout.MinWidth(100));
        height = int.Parse(temp);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("格子宽度");
        temp = GUILayout.TextField(cellwidth.ToString(), GUILayout.MinWidth(100));
        cellwidth = int.Parse(temp);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("格子高度");
        temp = GUILayout.TextField(cellheight.ToString(), GUILayout.MinWidth(100));
        cellheight = int.Parse(temp);
        GUILayout.EndHorizontal();
        if(width*height*cellheight * cellwidth >0 && GUILayout.Button("创建"))
        {
            GameObject g = GameObject.CreatePrimitive(PrimitiveType.Plane);
            g.transform.position = Vector3.zero;
            var me = g.AddComponent<MapEdit>();
            me.width = width;
            me.height = height;
            me.cell_height = cellheight;
            me.cell_width = cellwidth;
            me.Generate();
        }
        GUILayout.EndVertical();
    }



	
}
