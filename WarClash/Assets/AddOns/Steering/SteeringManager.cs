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
    private void AddSteering(BaseSteering steering)
    {
        steering.Init(this);
        _steerings.Add(steering);
        _steerings.Sort((a, b) =>  a.Priority - b.Priority);
    }

    public T AddSteering<T>(int priority) where T : BaseSteering
    {
        if (HasSteering<T>())
            return null;
        var t = Pool.SP.Get<T>();// Activator.CreateInstance(typeof(T)) as T;
        t.Priority = priority;
        AddSteering(t);
        return t;
    }
    public bool HasSteering<T>() where T : BaseSteering
    {
        for (int i = 0; i < _steerings.Count; i++)
        {
            if (_steerings[i] is T)
                return true;
        }
        return false;
    }
    public BaseSteering GetSteering<T>() where T : BaseSteering
    {
        for (int i = 0; i < _steerings.Count; i++)
        {
            if (_steerings[i] is T)
                return _steerings[i];
        }
        return null;
    }
    private void RemoveSteering(BaseSteering steering) 
    {
        for (int i = 0; i < _steerings.Count; i++)
        {
            if (_steerings[i] ==  steering)
            {
                Pool.SP.Recycle(steering);
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
                Pool.SP.Recycle(_steerings[i]);
                _steerings[i].Exit();
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

