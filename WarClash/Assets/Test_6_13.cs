using Lockstep;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_6_13 : MonoBehaviour {
    // Use this for initialization
    void Start () {
        int angle = 234;
        Vector3 axis = new Vector3(0.3f, 0.4f, 0.5f);
        Quaternion a = Quaternion.AngleAxis(angle, axis);
        var b = FixedQuaternion.AngleAxis(FixedMath.One.Mul(angle), new Vector3d(axis));
        var c = CreateFromAxisAngle(axis, angle);
        Debug.LogError(a+"  "+b+"   "+c);
        Debug.LogError(a*Vector3.left+"   "+b*new Vector3d(Vector3.left));
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
    
	}
}
