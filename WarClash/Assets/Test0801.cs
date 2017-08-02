using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test0801 : MonoBehaviour {

	// Use this for initialization
	void Start () {
   
	}

    private void Do(object sender, EventMsg e)
    {
        
    }

    // Update is called once per frame
    void Update () {
       
	}

    void OnDestroy()
    {
        Debug.LogError("1212");
    }
}

class TestController : Manager
{


}
