using System.Collections;
using System.Collections.Generic;
using Logic.Map;
using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using Logic.Skill;
using Newtonsoft.Json;
using Newtonsoft.Json.Utilities;
using UnityEngine.SceneManagement;

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
    private MapEdit me;
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
        float.TryParse(temp, out cellwidth);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("格子高度");
        temp = GUILayout.TextField(cellheight.ToString(), GUILayout.MinWidth(100));
        float.TryParse(temp, out cellheight);
        GUILayout.EndHorizontal();
        if(width*height*cellheight * cellwidth >0 && GUILayout.Button("创建"))
        {
            GameObject g = new GameObject();
            g.transform.position = Vector3.zero;
            me = g.AddComponent<MapEdit>();
            me.width = width;
            me.height = height;
            me.cell_height = cellheight;
            me.cell_width = cellwidth;
            me.Generate();
        }
        if (GUILayout.Button("Create Buiding"))
        {
            GameObject g = new GameObject("Building");
            var mapItemBe = g.AddComponent<MapItemBehaviour>();
            mapItemBe.MapItem = new MapBuildingItem();
        }
        if (GUILayout.Button("生成地图信息"))
        {
            var items = GameObject.FindObjectsOfType<MapItemBehaviour>();
            Dictionary<int, MapItem> mapDic = new Dictionary<int, MapItem>();
            foreach (var mapItemBehaviour in items)
            {
                if(mapItemBehaviour.MapItem == null) continue;
                var count =mapDic.Count+1;
                mapItemBehaviour.MapItem.Id = count;
                mapDic.Add(count, mapItemBehaviour.MapItem);
            }
            var str = Newtonsoft.Json.JsonConvert.SerializeObject(mapDic, Formatting.Indented, SkillUtility.settings);
            var scene = SceneManager.GetActiveScene();
            File.WriteAllText(Application.streamingAssetsPath+"/Map/"+scene.name+".map", str);
        }

        GUILayout.EndVertical();



    }



	
}
