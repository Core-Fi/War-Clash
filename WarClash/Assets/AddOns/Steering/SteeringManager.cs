using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lockstep;
using Logic.LogicObject;
public class SteeringManager
{
    public ISteering Self;
    private List<BaseSteering> _steerings = new List<BaseSteering>();
    
    public SteeringManager(ISteering self)
    {
        Self = self;
        _steeringResult = new SteeringResult();
    }
    public void AddSteering(BaseSteering steering)
    {
        steering.Self = Self;
        _steerings.Add(steering);
        _steerings.Sort((a, b) =>  a.Priority - b.Priority);
    }

    public T AddSteering<T>() where T : BaseSteering
    {
        var t = Activator.CreateInstance<T>();
        _steerings.Add(t);
        return t;
    }
    public void RemoveSteering(BaseSteering steering) 
    {
        for (int i = 0; i < _steerings.Count; i++)
        {
            if (_steerings[i] ==  steering)
            {
                _steerings.RemoveAt(i);
                break;
            }
        }
    }
    public void RemoveSteering<T>() where T:BaseSteering
    {
        for (int i = 0; i < _steerings.Count; i++)
        {
            if (_steerings[i] is T)
            {
                _steerings.RemoveAt(i);
                break;
            }
        }
    }
    public bool GetDesiredAcceleration(out Vector3d acceleration)
    {
        acceleration = Vector3d.zero;
        _steeringResult.Reset();
        for (int i = 0; i < _steerings.Count; i++)
        {
            if (_steerings[i].Enable)
            {
                _steerings[i].GetDesiredSteering(_steeringResult);
                if (_steeringResult.HasValue)
                {
                    acceleration = _steeringResult.DesiredSteering;
                    return true;
                }
            }
        }
        return false;
    }

    private SteeringResult _steeringResult;
}

