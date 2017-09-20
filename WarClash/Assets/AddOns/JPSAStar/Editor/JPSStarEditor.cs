using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[CustomEditor(typeof(JPSAStar))]
public class JPSStarEditor : Editor {
    public override void OnInspectorGUI()
    {
        var jpsStar = target as JPSAStar;
        base.OnInspectorGUI();
        if (GUILayout.Button("Scan"))
        {
            jpsStar.Data = new byte[jpsStar.RowCount * jpsStar.ColumnCount];
            var offset = new Vector3(jpsStar.Offset.x/100f, 0, jpsStar.Offset.y/100f);
            for (int i = 0; i < jpsStar.RowCount; i++)
            {
                for (int j = 0; j < jpsStar.ColumnCount; j++)
                {
                    var layerMask = LayerMask.GetMask("Obstacle");
                    var ob = Physics.OverlapSphere(new Vector3(j * jpsStar.Size / 100f, 0, i * jpsStar.Size / 100f) + offset,
                        jpsStar.Size / 202f, layerMask);
                    if (ob.Length==0)
                    {
                        jpsStar.Data[i*jpsStar.ColumnCount + j] += (byte)JPSAStar.NodeType.Walkable;
                    }
                    else
                    {
                        jpsStar.Data[i * jpsStar.ColumnCount + j] += (byte)JPSAStar.NodeType.UnWalkable;//有障碍物
                    }
                }
            }
            var scene = SceneManager.GetActiveScene();
            File.WriteAllBytes(Application.streamingAssetsPath + "/map/" + scene.name + "_jps.map", jpsStar.Data);
        }
        if (GUILayout.Button("GetPath"))
        {
            jpsStar.GenerateGrid();
        }

    }
}
