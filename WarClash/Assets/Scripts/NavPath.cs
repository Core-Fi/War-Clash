using System.Collections;
using System.Collections.Generic;
using System.IO;
using Lockstep;
using UnityEngine;
using UnityEngine.AI;
public struct NavRawData
{
    public Vector3d[] vertices;
    public int[] indices;
    public int[] areas;
}

public class NavNode
{
    private NavNode parent;
    public int v0;
    public int v1;
    public int v2;
    public Vector3d position;

}

public class NavPath
{
    public List<NavNode> nodes = new List<NavNode>();
     

}
