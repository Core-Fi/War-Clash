using System.Collections;
using System.Collections.Generic;
using RVO;
using UnityEngine;

public class Test925_2 : MonoBehaviour {

    public RVOTestSimulator simulator;

    public Transform[] transforms;
    private List<Agent> agents = new List<Agent>();
    // Use this for initialization
    void Start()
    {
        simulator = new RVOTestSimulator();
        for (int i = 0; i < transforms.Length; i++)
        {
            var agent = new Agent()
            {
                position_ = new RVO.Vector2(transforms[i].position.x, transforms[i].position.z),
                radius_ = 1,
                velocity_ = new RVO.Vector2(transforms[i].forward.x, transforms[i].forward.z),
                id_ =  i
            };
            agents.Add(agent);
            simulator.AddAgent(agent);
        }
    }

    // Update is called once per frame
    void Update()
    {
        simulator.Update();
        for (int i = 0; i < transforms.Length; i++)
        {
            agents[i].position_ = new RVO.Vector2(transforms[i].position.x, transforms[i].position.z);
            agents[i].velocity_ = new RVO.Vector2(transforms[i].forward.x, transforms[i].forward.z);
        }
    }
}
