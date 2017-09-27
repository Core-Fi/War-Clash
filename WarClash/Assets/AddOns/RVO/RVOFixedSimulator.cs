using System.Collections;
using System.Collections.Generic;
using FixedRVO;
using Lockstep;
using RVO;
using UnityEngine;

public class RVOFixedSimulator
{
    private List<RVOFixedAgent> agents = new List<RVOFixedAgent>();
    public RVOFixedQuadtree tree;

    public RVOFixedSimulator()
    {
        tree = new RVOFixedQuadtree();
    }

    public void AddAgent(RVOFixedAgent agent)
    {
        agents.Add(agent);
    }
    public void Update()
    {
        BuildQuadTree();
        for (int i = 0; i < agents.Count; i++)
        {
            agents[i].agentNeighbors_.Clear();
            tree.Query(new Vector2d(agents[i].position.x, agents[i].position.z), agents[i].radius, agents[i]);
            agents[i].computeNewVelocity();
        }
    }

    void BuildQuadTree()
    {
        tree.Clear();
        Utility.FixedRect bounds = Utility.MinMaxRect(agents[0].position.x, agents[0].position.z, agents[0].position.x, agents[0].position.z);
        for (int i = 0; i < agents.Count; i++)
        {
            Vector3d p = agents[i].position;
            bounds = Utility.MinMaxRect(Utility.Min(bounds.xMin, p.x), Utility.Min(bounds.yMin, p.z), Utility.Max(bounds.xMax, p.x), Utility.Max(bounds.yMax, p.z));
        }
        tree.SetBounds(bounds);
        for (int i = 0; i < agents.Count; i++)
        {
            tree.Insert(agents[i]);
        }
      //  tree.DebugDraw();
    }
}
public class RVOTestSimulator
{
    private List<Agent> agents = new List<Agent>();
    public RVOTestQuadtree tree;

    public RVOTestSimulator()
    {
        tree = new RVOTestQuadtree();
    }

    public void AddAgent(RVO.Agent agent)
    {
        agents.Add(agent);
    }
    public void Update()
    {
        BuildQuadTree();
        for (int i = 0; i < agents.Count; i++)
        {
            agents[i].agentNeighbors_.Clear();
            tree.Query(agents[i].position_, agents[i].radius_, agents[i]);
            agents[i].computeNewVelocity();
        }
    }

    void BuildQuadTree()
    {
        tree.Clear();
        Rect bounds = Rect.MinMaxRect(agents[0].position_.x(), agents[0].position_.y(), agents[0].position_.x(), agents[0].position_.y());
        for (int i = 0; i < agents.Count; i++)
        {
            var p = agents[i].position_;
            bounds = Rect.MinMaxRect(Mathf.Min(bounds.xMin, p.x()), Mathf.Min(bounds.yMin, p.y()), Mathf.Max(bounds.xMax, p.x()), Mathf.Max(bounds.yMax, p.y()));
        }
        tree.SetBounds(bounds);
        for (int i = 0; i < agents.Count; i++)
        {
            tree.Insert(agents[i]);
        }
        tree.DebugDraw();
    }
}