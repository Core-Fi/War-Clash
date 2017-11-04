using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lockstep;
using Logic;
using Logic.LogicObject;
using UnityEngine;


class BaseArriveSteering : BaseSteering
{
    public Vector3d Target;
    public bool Finish {get; protected set; }
    protected override void OnInit()
    {
        base.OnInit();
    }

    protected override void OnExit()
    {
        base.OnExit();
        GridService.Clear(Target, Self as SceneObject);
    }

    public override void GetDesiredSteering(SteeringResult rst)
    {
       base.GetDesiredSteering(rst);
    }

}
