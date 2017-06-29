using Lockstep;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_6_13 : MonoBehaviour {
    public List<Action> actions = new List<Action>();
    // Use this for initialization
    void Start () {
        for (int i = 0; i < 10; i++)
        {
            actions.Add(() =>
            {
                int b = i + 0;
            });
        }
	}
    public static Quaternion CreateFromAxisAngle(Vector3 axis, float angle)
    {
        angle = angle * Mathf.Deg2Rad;
        Quaternion quaternion;
        float num2 = angle * 0.5f;
        float num = (float)Mathf.Sin((float)num2);
        float num3 = (float)Mathf.Cos((float)num2);
        quaternion.x = axis.x * num;
        quaternion.y = axis.y * num;
        quaternion.z = axis.z * num;
        quaternion.w = num3;
        return quaternion;

    }
    // Update is called once per frame
    void Update () {
    
        if(actions.Count>0)
        {
            foreach (var item in actions)
            {
                item.Invoke();
            }
            actions.Clear();
        }
	}
}
