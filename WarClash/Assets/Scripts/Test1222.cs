using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test1222 : MonoBehaviour
{
    public Mesh  mf;
	// Use this for initialization
	void Start ()
	{
	    Mesh om = mf;
	    int triCount = om.triangles.Length / 3;
        List<Vector3> vs = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> ts = new List<int>();
	    for (int i = 0; i < triCount; i++)
	    {
	        int index1 = om.triangles[i*3];
	        int index2 = om.triangles[i * 3+1];
	        int index3 = om.triangles[i * 3+2];
            var v1 = om.vertices[index1];
            var v2 = om.vertices[index2];
	        var v3 = om.vertices[index3];

	        var center = (v1 + v2 + v3)/3;

	        var uv1 = om.uv[index1];
	        var uv2 = om.uv[index2];
	        var uv3 = om.uv[index3];
            vs.Add((v1 - center).normalized * 0.1f + center);
	        vs.Add((v2 - center).normalized * 0.1f + center);
	        vs.Add((v3 - center).normalized * 0.1f + center);
            uvs.Add(uv1);
	        uvs.Add(uv2);
	        uvs.Add(uv3);
            ts.Add(i*3);
	        ts.Add(i * 3+1);
	        ts.Add(i * 3+2);
        }
        Mesh m = new Mesh();
	    m.vertices = vs.ToArray();
	    m.triangles = ts.ToArray();
	    m.uv = uvs.ToArray();
        m.RecalculateNormals();
        var g = new GameObject("T");
	    var nmf = g.AddComponent<MeshFilter>();
	    nmf.mesh = m;
	    var mr = g.AddComponent<MeshRenderer>();
        mr.material = new Material(Shader.Find("Standard"));
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
