using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Box : MonoBehaviour
{
    public enum Status
    {
        Unreachable = 0,
        Reachable = 1,
    }
    public Vector3 size;
    public Status status;
    public Action OnClick;
    public int x, y;
    public void DrawGizmos()
    {
        if(status == Status.Reachable)
        {
            Gizmos.color = Color.green;
        }
        else
        {
            Gizmos.color = Color.red;
        }
        Gizmos.DrawCube(transform.position,
                  new Vector3(size.x, 0.001f, size.z));
    }
}
