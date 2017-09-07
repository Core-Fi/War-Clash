using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using Lockstep;

public class NavMeshEditor : Editor {

    [MenuItem("Tool/SaveNavMesh")]
    public static void SaveNavMesh()
    {
        UnityEngine.AI.NavMeshTriangulation triangles = UnityEngine.AI.NavMesh.CalculateTriangulation();
        NavRawData navRawData;
        navRawData.vertices = new Vector3d[triangles.vertices.Length];
        for (int i = 0; i < triangles.vertices.Length; i++)
        {
            navRawData.vertices[i] = new Vector3d(triangles.vertices[i]);
        }
        navRawData.indices = triangles.indices;
        navRawData.areas = triangles.areas;
        var g = AstarPath.active.astarData.graphs[0];
        var str = JsonUtility.ToJson(navRawData);
        File.WriteAllText(Application.dataPath + "/nav.data", str);
        var rawData = JsonUtility.FromJson<NavRawData>(str);
    }
	
}
