using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using DG.Tweening;
using Lockstep;
using Logic;
using Pathfinding;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class Test918 : MonoBehaviour
{
    public float t;
    public AnimationClip clip;
    private FixedAnimationClip fclip;

    public GameObject g1;
    public GameObject g2;

    void Start()
    {
        //fclip = FixedAnimationClip.CreateFixedAnimationClip(clip);
        //fclip.Transform = transform;
        List<Vector3d> v = new List<Vector3d>();
        JPSAStar.active.GetPath(new Vector3d(g1.transform.position), new Vector3d(g2.transform.position), v);
        for (int i = 0; i < v.Count-1; i++)
        {
            Debug.DrawLine(v[i].ToVector3(), v[i+1].ToVector3(), Color.blue, 10);
        }
    }
  

    void Update()
    {
        //fclip.Update();
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

