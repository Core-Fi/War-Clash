using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test918 : MonoBehaviour
{
    private static int randomSeed;
	// Use this for initialization
	void Start ()
	{
	    randomSeed = 1000;
    }

    private void OnBtLoad(string arg1, UnityEngine.Object arg2)
    {
    }

    // Update is called once per frame
    void Update () {
	    DLog.Log(RandomRange(0,1000).ToString());	
	}

    int RandomRange(int s, int e)
    {
        randomSeed++;
        UnityEngine.Random.InitState(randomSeed);
        return Random.Range(s, e);
    }
}
