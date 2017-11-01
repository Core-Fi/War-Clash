using System.Collections.Generic;
using Lockstep;
using Logic.LogicObject;
using UnityEngine;

public interface IFixedAgent
{
    IFixedAgent Next { get; set; }
    Vector3d Position { get; set; }
    long Radius { get; }

}

public class FixedQuadTree<T> where T: SceneObject
{
    private const int LeafSize = 10;

    private Utility.FixedRect _bounds;

    private Node _root;
    private readonly Dictionary<IFixedAgent, Node> _agentRootDic = new Dictionary<IFixedAgent, Node>();
    public void Clear()
    {
       _root = new Node();
    }

    public void SetBounds(Utility.FixedRect r)
    {
        _bounds = r;
        if (_root == null)
        {
            Clear();
        }
        _root.rect = _bounds;
    }

    private bool Remove(Node node, IFixedAgent fixedAgent)
    {
        var n = node.linkedList;
        if (n == null) return false;
        if (n == fixedAgent)
        {
            node.linkedList = n.Next;
            node.count--;
            return true;
        }
        int count = 0;
        while (n.Next!=null && count<LeafSize+1)
        {
            //count++;
            if (n.Next == fixedAgent)
            {
                n.Next = fixedAgent.Next;
                node.count--;
                return true;
            }
            n = n.Next;
        }
        return false;
    }
    public bool Remove(IFixedAgent fixedAgent)
    {
        Node node = null;
        if (_agentRootDic.TryGetValue(fixedAgent, out node))
        {
            return Remove(node, fixedAgent);
        }
        return false;
    }

    public void Relocate(IFixedAgent fixedAgent)
    {
        Node node;
        if (_agentRootDic.TryGetValue(fixedAgent, out node))
        {
            if (!node.rect.ContainsPoint(fixedAgent.Position))
            {
                if (Remove(node, fixedAgent))
                {
                    Insert<T>(fixedAgent);
                }
            }
        }
    }
    private void Add(Node node, IFixedAgent fixedAgent)
    {
        fixedAgent.Next = node.linkedList;
        node.linkedList = fixedAgent;
        node.count++;
        _agentRootDic[fixedAgent] = node;
    }
    private void Distribute(Node node, Utility.FixedRect r)
    {
        var c = r.center;
        node.child11 = new Node { rect = Utility.MinMaxRect(c.x, c.y, r.xMax, r.yMax) };
        node.child10 = new Node { rect = Utility.MinMaxRect(c.x, r.yMin, r.xMax, c.y) };
        node.child01 = new Node { rect = Utility.MinMaxRect(r.xMin, c.y, c.x, r.yMax) }; 
        node.child00 = new Node { rect = Utility.MinMaxRect(r.xMin, r.yMin, c.x, c.y) }; 
        while (node.linkedList != null)
        {
            var nx = node.linkedList.Next;
            if (node.linkedList.Position.x > c.x)
            {
                if (node.linkedList.Position.z > c.y) Add(node.child11,node.linkedList);
                else Add(node.child10,node.linkedList);
            }
            else
            {
                if (node.linkedList.Position.z > c.y) Add(node.child01, node.linkedList);
                else Add(node.child00, node.linkedList);
            }
            node.linkedList = nx;
        }
        node.linkedList = null;
        node.count = 0;
    }
    public void Insert<T>(IFixedAgent fixedAgent)
    {
        var r = _bounds;
        var p = new Vector2d(fixedAgent.Position.x, fixedAgent.Position.z);
        fixedAgent.Next = null;
        var node = _root;
        while (true)
        {
            if (!node.HasChild)
            {
                if (node.count < LeafSize)
                {
                    Add(node, fixedAgent);
                    break;
                }
                else
                {
                    Distribute(node, r);
                }

            }
            else
            {
                var c = r.center;
                if (p.x > c.x)
                {
                    if (p.y > c.y)
                    {
                        node = node.child11;
                        r = Utility.MinMaxRect(c.x, c.y, r.xMax, r.yMax);
                    }
                    else
                    {
                        node = node.child10;
                        r = Utility.MinMaxRect(c.x, r.yMin, r.xMax, c.y);
                    }
                }
                else
                {
                    if (p.y > c.y)
                    {
                        node = node.child01;
                        r = Utility.MinMaxRect(r.xMin, c.y, c.x, r.yMax);
                    }
                    else
                    {
                        node = node.child00;
                        r = Utility.MinMaxRect(r.xMin, r.yMin, c.x, c.y);
                    }
                }
            }
        }
    }

    public void Query(IFixedAgent fixedAgent, long radius, List<IFixedAgent> fixedAgents)
    {
        QueryRec(_root, fixedAgent, radius, fixedAgents, _bounds);
    }

    private long QueryRec(Node node, IFixedAgent fixedAgent, long radius, List<IFixedAgent> fixedAgents, Utility.FixedRect r)
    {
        Vector2d p = new Vector2d(fixedAgent.Position.x, fixedAgent.Position.z);
        //找到一个子节点
        if (!node.HasChild)
        {
            var a = node.linkedList;
            while (a != null)
            {
                if (a!=fixedAgent && Vector2d.SqrDistance(p, new Vector2d(a.Position.x, a.Position.z)) < radius.Mul(radius))
                {
                    fixedAgents.Add(a);
                }
                a = a.Next;
            }
        }
        else
        {
            //搜索子节点
            var c = r.center;
            if (p.x - radius < c.x)
            {
                if (p.y - radius < c.y)
                    radius = QueryRec(node.child00, fixedAgent, radius, fixedAgents,
                        Utility.MinMaxRect(r.xMin, r.yMin, c.x, c.y));
                if (p.y + radius > c.y)
                    radius = QueryRec(node.child01, fixedAgent, radius, fixedAgents,
                        Utility.MinMaxRect(r.xMin, c.y, c.x, r.yMax));
            }

            if (p.x + radius > c.x)
            {
                if (p.y - radius < c.y)
                    radius = QueryRec(node.child10, fixedAgent, radius, fixedAgents,
                        Utility.MinMaxRect(c.x, r.yMin, r.xMax, c.y));
                if (p.y + radius > c.y)
                    radius = QueryRec(node.child11, fixedAgent, radius, fixedAgents,
                        Utility.MinMaxRect(c.x, c.y, r.xMax, r.yMax));
            }
        }

        return radius;
    }
    public void Query(Vector2d p, long radius, List<IFixedAgent> fixedAgents)
    {
        fixedAgents.Clear();
        QueryRec(_root, p, radius, fixedAgents, _bounds);
    }

    private long QueryRec(Node node, Vector2d p, long radius, List<IFixedAgent> fixedAgents, Utility.FixedRect r)
    {
        //找到一个子节点
        if (!node.HasChild)
        {
            var a = node.linkedList;
            while (a != null)
            {
                if( Vector2d.SqrDistance(p, new Vector2d(a.Position.x, a.Position.z))<radius.Mul(radius))
                {
                    fixedAgents.Add(a);
                }
                a = a.Next;
            }
        }
        else
        {
            //搜索子节点
            var c = r.center;
            if (p.x - radius < c.x)
            {
                if (p.y - radius < c.y)
                    radius = QueryRec(node.child00, p, radius, fixedAgents,
                        Utility.MinMaxRect(r.xMin, r.yMin, c.x, c.y));
                if (p.y + radius > c.y)
                    radius = QueryRec(node.child01, p, radius, fixedAgents,
                        Utility.MinMaxRect(r.xMin, c.y, c.x, r.yMax));
            }

            if (p.x + radius > c.x)
            {
                if (p.y - radius < c.y)
                    radius = QueryRec(node.child10, p, radius, fixedAgents,
                        Utility.MinMaxRect(c.x, r.yMin, r.xMax, c.y));
                if (p.y + radius > c.y)
                    radius = QueryRec(node.child11, p, radius, fixedAgents,
                        Utility.MinMaxRect(c.x, c.y, r.xMax, r.yMax));
            }
        }

        return radius;
    }
    public void DebugDraw()
    {
        DebugDrawRec(_root, _bounds);
    }

    private void DebugDrawRec(Node node, Utility.FixedRect r)
    {
        Debug.DrawLine(new Vector3(r.xMin.ToInt(), 0, r.yMin.ToInt()), new Vector3(r.xMax.ToInt(), 0, r.yMin.ToInt()), Color.white);
        Debug.DrawLine(new Vector3(r.xMax.ToInt(), 0, r.yMin.ToInt()), new Vector3(r.xMax.ToInt(), 0, r.yMax.ToInt()), Color.white);
        Debug.DrawLine(new Vector3(r.xMax.ToInt(), 0, r.yMax.ToInt()), new Vector3(r.xMin.ToInt(), 0, r.yMax.ToInt()), Color.white);
        Debug.DrawLine(new Vector3(r.xMin.ToInt(), 0, r.yMax.ToInt()), new Vector3(r.xMin.ToInt(), 0, r.yMin.ToInt()), Color.white);

        if (node.child00 != null)
        {
            // Not a leaf node
            var c = r.center;
            DebugDrawRec(node.child11, Utility.MinMaxRect(c.x, c.y, r.xMax, r.yMax));
            DebugDrawRec(node.child10, Utility.MinMaxRect(c.x, r.yMin, r.xMax, c.y));
            DebugDrawRec(node.child01, Utility.MinMaxRect(r.xMin, c.y, c.x, r.yMax));
            DebugDrawRec(node.child00, Utility.MinMaxRect(r.xMin, r.yMin, c.x, c.y));
        }

        var a = node.linkedList;
        int count = 0;
        while (a != null && count<LeafSize+1)
        {
            count++;
            Debug.DrawLine(node.linkedList.Position.ToVector3() + Vector3.up, a.Position.ToVector3() + Vector3.up,
                new Color(1, 1, 0, 0.5f));
            a = a.Next;

        }
    }

    public class Node
    {
        public Node child00;
        public Node child01;
        public Node child10;
        public Node child11;
        public byte count;
        public Utility.FixedRect rect;
        public IFixedAgent linkedList;

        public bool HasChild
        {
            get { return child00 != null; }
        }
    }
}