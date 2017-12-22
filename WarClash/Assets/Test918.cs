using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using AStar;
using DG.Tweening;
using Lockstep;
using Logic;
using Pathfinding;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Point = AStar.Point;
using Random = UnityEngine.Random;

public class Test918 : MonoBehaviour
{
    public float t;
    public AnimationClip clip;
    private FixedAnimationClip fclip;

    public GameObject g1;
    public GameObject g2;

    public SkinnedMeshRenderer smr;
    public MeshFilter mf;
    public Animator ani;
    byte[] bytes;
    private int index = 0;
    private int vertexCount = 0;
    private float[] vertexInfos;
    private Vector2[] uvs;
    void Start()
    {
        //fclip = FixedAnimationClip.CreateFixedAnimationClip(clip);
        //fclip.Transform = transform;
        //List<Vector3d> v = new List<Vector3d>();
        //JPSAStar.active.GetPath(new Vector3d(g1.transform.position), new Vector3d(g2.transform.position), v);
        
        //byte[,] g = new byte[JPSAStar.active.RowCount, JPSAStar.active.ColumnCount];
        //for (int i = 0; i < JPSAStar.active.Data.Length; i++)
        //{
        //    var x = i % JPSAStar.active.ColumnCount;
        //    var y = i / JPSAStar.active.ColumnCount;
        //    g[y,x] = JPSAStar.active.Data[i];
        //}
        //PathFinder pf = new PathFinder(g);
        //var list = new List<PathFinderNode>();
        //JPSAStar.active.AStarFindPath(new Vector3d(g1.transform.position), new Vector3d(g2.transform.position), list);
        //for (int i = 0; i < list.Count - 1; i++)
        //{
        //    Debug.DrawLine(p2v(list[i]), p2v(list[i + 1]), Color.blue, 10);
        //}
        for (int i = 0; i < 32; i++)
        {
           // var m = AssetDatabase.LoadAssetAtPath<Mesh>("Assets/BakeAnimaitons/" + i + ".asset");
        }
        bytes = File.ReadAllBytes(Application.dataPath + "/BakeAnimaitons/total_byte.byte");
        vertexCount = BitConverter.ToInt16(bytes, 0);
        v = new Vector3[vertexCount];
      
        uvs = new Vector2[vertexCount];
        int vertexIndex = 0;
        while (vertexIndex < vertexCount)
        {
            float x = (float)BitConverter.ToInt16(bytes, vertexIndex * 2 + 2) / 10000f;
            float y = (float)BitConverter.ToInt16(bytes, vertexIndex * 2 + 3) / 10000f;
            uvs[vertexIndex] = new Vector2(x, y);
            vertexIndex++;
        }
        int startIndex = 2 + vertexCount * 4;
        vertexIndex = 0;
        int frameIndex = 0;
        int frameCount = (bytes.Length - 2 - vertexCount*4)/(vertexCount*6);
        vertexInfos = new float[vertexCount * 3 * frameCount];
        while (frameCount-1 >= frameIndex)
        {
            vertexIndex = 0;
            while (vertexIndex < vertexCount)
            {
                float x = (float)BitConverter.ToInt16(bytes, vertexIndex * 6 + startIndex) / 10000f;
                float y = (float)BitConverter.ToInt16(bytes, vertexIndex * 6 + 2 + startIndex) / 10000f;
                float z = (float)BitConverter.ToInt16(bytes, vertexIndex * 6 + 4 + startIndex) / 10000f;
                vertexInfos[vertexIndex * 3 + frameIndex * vertexCount * 3] = x;
                vertexInfos[vertexIndex * 3 + 1 + frameIndex * vertexCount * 3] = y;
                vertexInfos[vertexIndex * 3 + 2 + frameIndex * vertexCount * 3] = z;
                vertexIndex++;
               
            }
            frameIndex++;
            startIndex += vertexCount * 6;
        //    Debug.LogError(frameIndex+" "+frameCount);
        }
      
    }

    Vector3 p2v(AStar.PathFinderNode p)
    {
        return JPSAStar.active.P2V(p).ToVector3();
    }

    private Mesh m;
    Vector3[] v ;
    void Update()
    {
       // ani.Play("attack", -1, 1);
        //if (m == null)
        //{
        //    m = new Mesh();
        //    m.vertices = new Vector3[vertexCount];
        //    m.triangles = smr.sharedMesh.triangles;
        //    mf.mesh = m;
        //}
        //smr.BakeMesh(m);
        //if (index > 30)
        //    index = 0;
        //for (int i = 0; i < vertexCount; i++)
        //{
        //    float x = vertexInfos[index * vertexCount * 3 + i];
        //    float y = vertexInfos[index * vertexCount * 3 + i + 1];
        //    float z = vertexInfos[index * vertexCount * 3 + i + 2];
        //    v[i] = new Vector3(x, y, z);
        //}
        //m.vertices = v;
        //m.RecalculateNormals();
        //index++;
    }
    void OnEnable()
    {
        
    }
    //RVOFixedAgent agent1 = new RVOFixedAgent();
    //RVOFixedAgent agent2 = new RVOFixedAgent();
    //RVOFixedQuadtree tree = new RVOFixedQuadtree();

    //void Start()
    //{
    //    agent1.velocity_ = new Vector2d(-FixedMath.One, 0);
    //    agent2.velocity_ = new Vector2d(FixedMath.One, 0);

    //    agent1.position_ = new Vector2d();
    //    agent1.position_ = new Vector2d(FixedMath.One*4,0);
    //}

    //void Update()
    //{
    //    tree.Clear();
    //    tree.SetBounds(new Utility.FixedRect(0, 0, FixedMath.Create(100), FixedMath.Create(100)));
    //    tree.Insert(agent1);
    //    tree.Insert(agent2);
    //    agent1.agentNeighbors_.Clear();
    //    agent2.agentNeighbors_.Clear();
    //    tree.Query(new Vector2d(agent1.position.x, agent1.position.z), agent1.radius, agent1);
    //    tree.Query(new Vector2d(agent2.position.x, agent2.position.z), agent2.radius, agent2);
    //    agent1.computeNewVelocity();
    //    agent2.computeNewVelocity();
    //    agent1.update();
    //    agent2.update();

    //    g1.transform.position = agent1.position.ToVector3();
    //    g2.transform.position = agent2.position.ToVector3();
    //}
}

