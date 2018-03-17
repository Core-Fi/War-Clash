using System.Collections;
using System.Collections.Generic;

using Lockstep;
using UnityEngine;

public class Test925 : MonoBehaviour
{
    public int renderOrder;
    public GameObject g;
    public RectTransform rt;
    public Camera UICamera;
    
    void Update()
    {
        DLog.Log(rt.transform.position.ToString());

    }

}
