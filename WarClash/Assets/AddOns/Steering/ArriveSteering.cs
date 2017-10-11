using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lockstep;
using Logic;


class ArriveSteering : BaseSteering
{
    public Vector3d Target;

    public override Vector3d? GetDesiredSteering()
    {
        Vector3d dir = Target - Self.Position;
        Vector3d desiredVelocity = dir.Normalize()*Self.Speed;
        return (desiredVelocity - Self.Velocity).Div(LockFrameMgr.FixedFrameTime);
    }
}
