using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lockstep;
using Logic.LogicObject;

public interface ISteering
{
    Vector3d Acceleration { get;}
    Vector3d Velocity { get; }
    Vector3d Position { get; }
    long Speed { get; }
    long Radius { get;}
}
public abstract class BaseSteering
{
    public bool Enable;
    public ISteering Self;
    public int Priority;
    public virtual Vector3d? GetDesiredSteering()
    {
        return Vector3d.zero;
    }
}
