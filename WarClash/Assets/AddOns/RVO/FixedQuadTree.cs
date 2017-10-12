using System.Collections.Generic;
using Lockstep;
using UnityEngine;

public interface IFixedAgent
{
    IList<IFixedAgent> AgentNeighbors { get; set; }
    IList<long> AgentNeighborSqrDists { get; set; }
    //  IList<IFixedAgent> AgentNeighbors { get; set; }
    IFixedAgent Next { get; set; }
    Vector3d Position { get; set; }
    long InsertAgentNeighbour(IFixedAgent fixedAgent, long rangeSq);
}

/** Quadtree for quick nearest neighbour search of agents.
 */
public class FixedQuadTree
{
    private const int LeafSize = 15;

    private Utility.FixedRect bounds;
    private int filledNodes = 1;

    private Node[] nodes = new Node[42];

    public void Clear()
    {
        nodes[0] = new Node();
        filledNodes = 1;
    }

    public void SetBounds(Utility.FixedRect r)
    {
        bounds = r;
    }

    public int GetNodeIndex()
    {
        if (filledNodes == nodes.Length)
        {
            var nds = new Node[nodes.Length * 2];
            for (var i = 0; i < nodes.Length; i++) nds[i] = nodes[i];
            nodes = nds;
        }
        nodes[filledNodes] = new Node();
        nodes[filledNodes].child00 = filledNodes;
        filledNodes++;
        return filledNodes - 1;
    }

    public void Insert(IFixedAgent fixedAgent)
    {
        var i = 0;
        var r = bounds;
        var p = new Vector2d(fixedAgent.Position.x, fixedAgent.Position.z);

        fixedAgent.Next = null;

        var depth = 0;

        while (true)
        {
            depth++;

            if (nodes[i].child00 == i)
            {
                if (nodes[i].count < LeafSize || depth > 10)
                {
                    nodes[i].Add(fixedAgent);
                    nodes[i].count++;
                    break;
                }
                else
                {
                    // Split
                    var node = nodes[i];
                    node.child00 = GetNodeIndex();
                    node.child01 = GetNodeIndex();
                    node.child10 = GetNodeIndex();
                    node.child11 = GetNodeIndex();
                    nodes[i] = node;

                    nodes[i].Distribute(nodes, r);
                }
            }
            // Note, no else
            if (nodes[i].child00 != i)
            {
                // Not a leaf node
                var c = r.center;
                if (p.x > c.x)
                {
                    if (p.y > c.y)
                    {
                        i = nodes[i].child11;
                        r = Utility.MinMaxRect(c.x, c.y, r.xMax, r.yMax);
                    }
                    else
                    {
                        i = nodes[i].child10;
                        r = Utility.MinMaxRect(c.x, r.yMin, r.xMax, c.y);
                    }
                }
                else
                {
                    if (p.y > c.y)
                    {
                        i = nodes[i].child01;
                        r = Utility.MinMaxRect(r.xMin, c.y, c.x, r.yMax);
                    }
                    else
                    {
                        i = nodes[i].child00;
                        r = Utility.MinMaxRect(r.xMin, r.yMin, c.x, c.y);
                    }
                }
            }
        }
    }

    public void Query(Vector2d p, long radius, IFixedAgent fixedAgent)
    {
        QueryRec(0, p, radius, fixedAgent, bounds);
    }

    private long QueryRec(int i, Vector2d p, long radius, IFixedAgent fixedAgent, Utility.FixedRect r)
    {
        if (nodes[i].child00 == i)
        {
            var a = nodes[i].linkedList;
            while (a != null)
            {
                var v = fixedAgent.InsertAgentNeighbour(a, radius.Mul(radius));
                if (v < radius.Mul(radius))
                    radius = FixedMath.Sqrt(v);
                a = a.Next;
            }
        }
        else
        {
            // Not a leaf node
            var c = r.center;
            if (p.x - radius < c.x)
            {
                if (p.y - radius < c.y)
                    radius = QueryRec(nodes[i].child00, p, radius, fixedAgent,
                        Utility.MinMaxRect(r.xMin, r.yMin, c.x, c.y));
                if (p.y + radius > c.y)
                    radius = QueryRec(nodes[i].child01, p, radius, fixedAgent,
                        Utility.MinMaxRect(r.xMin, c.y, c.x, r.yMax));
            }

            if (p.x + radius > c.x)
            {
                if (p.y - radius < c.y)
                    radius = QueryRec(nodes[i].child10, p, radius, fixedAgent,
                        Utility.MinMaxRect(c.x, r.yMin, r.xMax, c.y));
                if (p.y + radius > c.y)
                    radius = QueryRec(nodes[i].child11, p, radius, fixedAgent,
                        Utility.MinMaxRect(c.x, c.y, r.xMax, r.yMax));
            }
        }

        return radius;
    }

    public void DebugDraw()
    {
        DebugDrawRec(0, bounds);
    }

    private void DebugDrawRec(int i, Utility.FixedRect r)
    {
        Debug.DrawLine(new Vector3(r.xMin, 0, r.yMin), new Vector3(r.xMax, 0, r.yMin), Color.white);
        Debug.DrawLine(new Vector3(r.xMax, 0, r.yMin), new Vector3(r.xMax, 0, r.yMax), Color.white);
        Debug.DrawLine(new Vector3(r.xMax, 0, r.yMax), new Vector3(r.xMin, 0, r.yMax), Color.white);
        Debug.DrawLine(new Vector3(r.xMin, 0, r.yMax), new Vector3(r.xMin, 0, r.yMin), Color.white);

        if (nodes[i].child00 != i)
        {
            // Not a leaf node
            var c = r.center;
            DebugDrawRec(nodes[i].child11, Utility.MinMaxRect(c.x, c.y, r.xMax, r.yMax));
            DebugDrawRec(nodes[i].child10, Utility.MinMaxRect(c.x, r.yMin, r.xMax, c.y));
            DebugDrawRec(nodes[i].child01, Utility.MinMaxRect(r.xMin, c.y, c.x, r.yMax));
            DebugDrawRec(nodes[i].child00, Utility.MinMaxRect(r.xMin, r.yMin, c.x, c.y));
        }

        var a = nodes[i].linkedList;
        while (a != null)
        {
            Debug.DrawLine(nodes[i].linkedList.Position.ToVector3() + Vector3.up, a.Position.ToVector3() + Vector3.up,
                new Color(1, 1, 0, 0.5f));
            a = a.Next;
        }
    }

    private struct Node
    {
        public int child00;
        public int child01;
        public int child10;
        public int child11;
        public byte count;
        public IFixedAgent linkedList;

        public void Add(IFixedAgent fixedAgent)
        {
            fixedAgent.Next = linkedList;
            linkedList = fixedAgent;
        }

        public void Distribute(Node[] nodes, Utility.FixedRect r)
        {
            var c = r.center;

            while (linkedList != null)
            {
                var nx = linkedList.Next;
                if (linkedList.Position.x > c.x)
                {
                    if (linkedList.Position.z > c.y) nodes[child11].Add(linkedList);
                    else nodes[child10].Add(linkedList);
                }
                else
                {
                    if (linkedList.Position.z > c.y) nodes[child01].Add(linkedList);
                    else nodes[child00].Add(linkedList);
                }
                linkedList = nx;
            }
            count = 0;
        }
    }
}