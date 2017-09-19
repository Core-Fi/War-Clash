using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

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

    private Action a;
    private int k = 0;
    // Update is called once per frame
    void Update ()
    {
        DOTween.To(() => { return transform.position; }, value => transform.position = value, Vector3.forward * 100, 2);
    }

    int RandomRange(int s, int e)
    {
        randomSeed++;
        UnityEngine.Random.InitState(randomSeed);
        return Random.Range(s, e);
    }
}
