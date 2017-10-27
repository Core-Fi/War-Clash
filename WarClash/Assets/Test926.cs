using System.Collections;
using System.Collections.Generic;
using Lockstep;
using Logic.LogicObject;
using UnityEngine;

public class Test926 : MonoBehaviour {

    FixedQuadTree<FixedAgentTest> tree = new FixedQuadTree<FixedAgentTest>();
    List<FixedAgentTest> agents = new List<FixedAgentTest>();
    private float previousTime;

    private float timeout = 2;
	// Use this for initialization
	void Start () {
        tree.Clear();
        tree.SetBounds(new Utility.FixedRect(0,0, FixedMath.One * 100, FixedMath.One * 100));
	    for (int i = 0; i < 100; i++)
	    {
	        var agent = new FixedAgentTest()
	        {
	            Position = new Vector3d(FixedMath.One * UnityEngine.Random.Range(0, 50), 0,
	                FixedMath.One * UnityEngine.Random.Range(0, 50))
	        };
            agents.Add(agent);
            tree.Insert<FixedAgentTest>(agent);
	    }
	    tree.Query(new Vector2d(agents[0].Position), FixedMath.One*10, agents[0]);
       
	}
	
	// Update is called once per frame
	void Update () {
		tree.DebugDraw();
	}

    public void Relocate()
    {
        for (int i = 0; i < agents.Count; i++)
        {
            agents[i].Position = new Vector3d(FixedMath.One * UnityEngine.Random.Range(0, 50), 0,
                FixedMath.One * UnityEngine.Random.Range(0, 50));
            tree.Relocate(agents[i]);
        }
    }

    public void OnDrawGizmos()
    {
        if (agents.Count > 0)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(agents[0].Position.ToVector3(), 0.5f);
            Gizmos.color = Color.green;
            for (int i = 0; i < agents[0].AgentNeighbors.Count; i++)
            {
                Gizmos.DrawSphere(agents[0].AgentNeighbors[i].Position.ToVector3(), 0.5f);
            }
        }
    }
    void OnEnable()
    {
        //Relocate();
        
    }
}

class FixedAgentTest : SceneObject, IFixedAgent
{
    public IList<IFixedAgent> AgentNeighbors { get; set; }
    public IList<long> AgentNeighborSqrDists { get; set; }
    public IFixedAgent Next { get; set; }
    public Vector3d Position { get; set; }
    public long Radius { get; set; }

    public FixedAgentTest()
    {
        AgentNeighbors = new List<IFixedAgent>();
        AgentNeighborSqrDists = new List<long>();
    }
    public long InsertAgentNeighbour(IFixedAgent fixedAgent, long rangeSq)
    {
        if (this == fixedAgent) return rangeSq;
        var dist = (fixedAgent.Position.x - Position.x).Mul(fixedAgent.Position.x - Position.x)
                   + (fixedAgent.Position.z - Position.z).Mul(fixedAgent.Position.z - Position.z);
        if (dist < rangeSq)
        {
            if (AgentNeighbors.Count < 10)
            {
                AgentNeighbors.Add(fixedAgent);
                AgentNeighborSqrDists.Add(dist);
            }
            var i = AgentNeighbors.Count - 1;
            if (dist < AgentNeighborSqrDists[i])
            {
                while (i != 0 && dist < AgentNeighborSqrDists[i - 1])
                {
                    AgentNeighbors[i] = AgentNeighbors[i - 1];
                    AgentNeighborSqrDists[i] = AgentNeighborSqrDists[i - 1];
                    i--;
                }
                AgentNeighbors[i] = fixedAgent;
                AgentNeighborSqrDists[i] = dist;
            }

            if (AgentNeighbors.Count == 10)
                rangeSq = AgentNeighborSqrDists[AgentNeighbors.Count - 1];
        }
        return rangeSq;
    }

}