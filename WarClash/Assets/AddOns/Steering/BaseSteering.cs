using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lockstep;
using Logic.LogicObject;

public class SteeringResult
{
    public bool HasValue
    {
        get { return _hasValue; }
    }

    private bool _hasValue;
    public Vector3d DesiredSteering {
        set
        {
            _hasValue = true;
            _desiredSteering = value;
        }
        get { return _desiredSteering; }
    }
    private Vector3d _desiredSteering;

    public void Reset()
    {
        _hasValue = false;
        _desiredSteering = Vector3d.zero;
    }

}
public interface ISteering : IFixedAgent
{
    Vector3d Acceleration { get;}
    Vector3d Velocity { get; }
    long Speed { get; }
    long MaxAcceleration { get; }
    long MaxDeceleration { get; }
}
public abstract class BaseSteering
{
    public bool Enable = true;
    public ISteering Self;
    public int Priority;

    public BaseSteering(ISteering self)
    {
        this.Self = self;
    }
    public virtual void GetDesiredSteering(SteeringResult rst)
    {
        return;
    }
}
