using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lockstep;
using Logic;
using Logic.LogicObject;
using UnityEngine;


class CircularArriveSteering : BaseArriveSteering
{
    private bool _validTarget = true;
    protected override void OnInit()
    {
        base.OnInit();
        _validTarget = true;
    }
    public override void GetDesiredSteering(SteeringResult rst)
    {
        if (Finish)
            return;
        if (Vector3d.SqrDistance(Target, Self.Position)<FixedMath.One/100)
        {
            Finish = true;
            if (_validTarget)
            {
                GridService.Clear(Target, Self as SceneObject);
                GridService.TagAs(Self.Position, Self as SceneObject, GridService.NodeType.BeTaken);
            }
            return;
        }
        if (_validTarget && GridService.IsNotEmptyBy(Target)!= Self)
        {
            _validTarget = GridService.SearchNearCircleEmptyPoint(Self.Position, Target, 4, out Target);
            if(_validTarget)
                GridService.TagAs(Target, Self as SceneObject, GridService.NodeType.FlagAsTarget);
        }
       
        Vector3d dir = Target - Self.Position;
        Vector3d desiredVelocity = dir.Normalize()*Self.Speed;
        var acc = (desiredVelocity - Self.Velocity)/(LockFrameMgr.FixedFrameTime);
        rst.DesiredSteering = acc;
    }

    
   
}
