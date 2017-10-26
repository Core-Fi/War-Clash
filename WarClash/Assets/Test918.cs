using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Lockstep;
using UnityEngine;
using Random = UnityEngine.Random;

public class Test918 : MonoBehaviour
{
    public Vector3 destination;
    public Vector3d velocity;
    public float radius = 1;
    private UnitAvoidSteering uas = new UnitAvoidSteering();
    private DestinaionSteering ds = new DestinaionSteering();


    private TestAvoidanceSteering tas = new TestAvoidanceSteering();
    void Start()
    {
        ds.Destination = new Vector3d(destination);
        tas._unitData = this;
        tas.Start();
    }
  

    private Action a;
    private int k = 0;
    // Update is called once per frame
    void Update ()
    {
        //var acce = tas.GetDesiredSteering();
        //acce += ds.GetDesiredSteering(this).ToVector3();
        //velocity = velocity + new Vector3d(acce * (1 / 30f));
        //Vector3d acc;
        //uas.GetDesiredSteering(out acc);
        //desiredAcceleration += ds.GetDesiredSteering(this);

        //velocity = velocity + desiredAcceleration * (FixedMath.One / 30);
        //transform.position += (velocity * (FixedMath.One / 30)).ToVector3();
    }
}

class DestinaionSteering
{
    public Vector3d Destination;

    public Vector3d GetDesiredSteering(Test918 t)
    {
        Vector3d disiredDir = Destination - new Vector3d(t.transform.position);
        disiredDir = disiredDir.Normalize();
        Vector3d disiredVelocity = disiredDir * 1;
        Vector3d disiredAcceleration = (disiredVelocity - t.velocity) / (FixedMath.One / 30);
      //  Debug.LogError("path acce "+disiredAcceleration.ToVector3());
        return disiredAcceleration;
    }
}
