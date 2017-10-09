using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Lockstep;
using UnityEngine;
using Random = UnityEngine.Random;

public class Test918 : MonoBehaviour
{
    private static int randomSeed;
    public Vector3 v3_velocity;

    public Vector3d velocity
    {
        get
        {
            return new Vector3d(v3_velocity);
        }
    }

    public Vector3d normalVelocity;
    private UnitAvoidSteering uas = new UnitAvoidSteering();
	// Use this for initialization
	void Start ()
	{
	    randomSeed = 1000;
    }

    private void OnBtLoad(string arg1, UnityEngine.Object arg2)
    {
    }

    private Action a;
    private int k = 0;
    // Update is called once per frame
    void Update ()
    {
        var desiredVelocity = uas.GetDesiredSteering(this);
        Debug.LogError(desiredVelocity);
        if(desiredVelocity!=Vector3d.zero)
            transform.position += desiredVelocity.Mul(FixedMath.One / 30).ToVector3();
        else
            transform.position += velocity.Mul(FixedMath.One/30).ToVector3();
    }

    int RandomRange(int s, int e)
    {
        randomSeed++;
        UnityEngine.Random.InitState(randomSeed);
        return Random.Range(s, e);
    }
}
