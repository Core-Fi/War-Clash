using System;
using System.Collections;
using System.Collections.Generic;
using Lockstep;
using Pathfinding;
using UnityEngine;

public class Test0801 : MonoBehaviour
{

    public GameObject a;
    public GameObject b;
    public GameObject c;
    public GameObject target;
    public GameObject closetPoint;
    // Use this for initialization
    void Start ()
    {
       
    }

    private void Do(object sender, EventMsg e)
    {
        
    }

    // Update is called once per frame
    void Update ()
    {

        var p = Polygon.ClosestPointOnTriangle(new Vector2d(a.transform.position), new Vector2d(b.transform.position), new Vector2d(c.transform.position),
            new Vector2d(target.transform.position));
        closetPoint.transform.position = p.ToVector3();
    }

    void OnDestroy()
    {

    }
}

