using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FingerGestureImpl : FingerGestures
{
    private int _maxFingers = 2;


    public override int MaxFingers
    {
        get { return _maxFingers; }
    }

    protected override FingerPhase GetPhase(Finger finger)
    {
        throw new System.NotImplementedException();
    }

    protected override Vector2 GetPosition(Finger finger)
    {
        throw new System.NotImplementedException();
    }
}
