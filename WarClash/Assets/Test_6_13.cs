using Lockstep;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;

public class Test_6_13 : MonoBehaviour {
    public List<Action> actions = new List<Action>();
    // Use this for initialization
    void Start () {

        //string str1 = Md5(Application.dataPath+"/data1.bytes");
        //string str2 = Md5(Application.dataPath + "/data2.bytes");
        //Debug.LogError(str1.Equals(str2));
    }
    public string Md5(string filename)
    {
        using (var md5 = MD5.Create())
        {
            using (var stream = File.OpenRead(filename))
            {
                return BitConverter.ToString(md5.ComputeHash(stream));
            }
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
    private void Ope()
    {
        n += 1;
    }
    public void Exe(Action t)
    {
        t.Invoke();
    }
    System.Action a;
    int n = 0;
    // Update is called once per frame
    void Update () {
    
        
    }

    void OnDestroy()
    {
        Debug.LogError("Destroyed");
    }
}
