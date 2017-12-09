using Lockstep;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using Logic;
using Logic.LogicObject;
using Pathfinding;
using UnityEngine;

public class Test_6_13 : MonoBehaviour
{
    public Transform A;
    public Transform B;
    public List<Action> actions = new List<Action>();
    private bool caculated = false;
    // Use this for initialization
    void Start ()
    {
        MainPlayer.SP.StateMachine.Start<GuiseState>();
        //string str1 = Md5(Application.dataPath + "/data1.bytes");
        //string str2 = Md5(Application.dataPath + "/data2.bytes");
        //Debug.LogError(str1.Equals(str2));
    }

 
    
    List<Vector3d> list = new List<Vector3d>(); 
    void OnCaculate(Pathfinding.Path path)
    {
        Debug.Log(Time.frameCount);
        caculated = true;
        list.Clear();
        ReflectionCaculator.CaculateReflectionPoints(path as FixedABPath, list);
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
        //var fixedQuaternion = new FixedQuaternion(FixedMath.Create(A.rotation.x),
        // FixedMath.Create(A.rotation.y),
        // FixedMath.Create(A.rotation.z),
        // FixedMath.Create(A.rotation.w)
        // );
        //var posi = new Vector3d(A.transform.position);
        //Utility.FixedRect rect = new Utility.FixedRect
        //{
        //    center = new Vector2d( posi.x, posi.z),
        //    width = FixedMath.One,
        //    height = FixedMath.One*2
        //};
        //var contains = Utility.PositionIsInRect(rect, posi, fixedQuaternion, new Vector3d(B.position));
        //if (contains)
        //{
        //    Debug.Log("Contains Congras");
        //}
        //if (Time.frameCount == 3)
        //{
        //    var sw = new System.Diagnostics.Stopwatch();
        //    sw.Start();
        //    FixedABPath path = FixedABPath.Construct(new Vector3d(A.position), new Vector3d(B.position), null);
        //    path.CacualteNow();
        //    OnCaculate(path);
        //    //  AstarPath.StartPath(path);
        //    Debug.Log("start " + sw.ElapsedMilliseconds);
        //}
    }
}
