using System.Collections;
using System.Collections.Generic;
using System.IO;
using Lockstep;
using UnityEngine;
using UnityEngine.AI;

public class NavPath
{
    public NavRawData data;

    public void Save()
    {
        NavMeshTriangulation triangles = NavMesh.CalculateTriangulation();
        //Mesh mesh = new Mesh();
        //mesh.vertices = triangles.vertices;
        //mesh.triangles = triangles.indices;
        //AssetDatabase.CreateAsset(mesh, "Assets/navmesh.asset");
        // ListenEvents();
        NavRawData navRawData;
        navRawData.vertices = new Vector3d[triangles.vertices.Length];
        for (int i = 0; i < triangles.vertices.Length; i++)
        {
            navRawData.vertices[i] = new Vector3d(triangles.vertices[i]);
        }
        navRawData.indices = triangles.indices;
        navRawData.areas = triangles.areas;
        var str = JsonUtility.ToJson(navRawData);
        File.WriteAllText(Application.dataPath + "/nav.data", str);
        var rawData = JsonUtility.FromJson<NavRawData>(str);
    }

}
