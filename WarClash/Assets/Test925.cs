using System.Collections;
using System.Collections.Generic;
using FixedRVO;
using Lockstep;
using RVO;
using UnityEngine;

public class Test925 : MonoBehaviour
{
    public RVOFixedSimulator simulator;

    public Transform[] transforms;
    private List<RVOFixedAgent> agents = new List<RVOFixedAgent>();
	// Use this for initialization
	void Start () {
        simulator = new RVOFixedSimulator();
	    for (int i = 0; i < transforms.Length; i++)
	    {
	        var agent = new RVOFixedAgent()
	        {
	            position = new Vector3d(transforms[i].position), radius = FixedMath.One,
	            velocity_ = new Vector2d(transforms[i].forward.x, transforms[i].forward.z),
	            id_ = i
            };
            agents.Add(agent);
            simulator.AddAgent(agent);
	    }
	}
	
	// Update is called once per frame
	void Update () {
		simulator.Update();
	    for (int i = 0; i < transforms.Length; i++)
	    {
	        agents[i].position = new Vector3d(transforms[i].position);
	        agents[i].velocity_ = new Vector2d(transforms[i].forward.x, transforms[i].forward.z);
	    }
	}
}
