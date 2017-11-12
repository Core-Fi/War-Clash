using Lockstep;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public struct FixedKeyFrame
{
    public FixedKeyFrame(long time, long value, long inTangent, long outTangent, int tangentMode)
    {
        this.time = time;
        this.value = value;
        this.inTangent = inTangent;
        this.outTangent = outTangent;
        this.tangentMode = tangentMode;
    }

    public long inTangent { get; set; }
    public long outTangent { get; set; }
    public long time { get; set; }
    public long value { get; set; }
    public int tangentMode { get; set; }

}
public class FixedAnimationCurve
{
#if UNITY_EDITOR
    public AnimationCurve OriginalAnimationCurve;
#endif
    public List<FixedKeyFrame> Keyframes = new List<FixedKeyFrame>();

    public void AddKeyFrame(FixedKeyFrame keyFrame)
    {
        //keyFrame._time = Math.Min(keyFrame._time, FixedMath.One);
        //keyFrame._time = Math.Max(keyFrame._time, 0);
        Keyframes.Add(keyFrame);
        Keyframes.Sort((a, b) => a.time - b.time > 0 ? 1 : -1);
    }
    public long Evaluate(long t)
    {
        //t = Math.Min(t, FixedMath.One);
        //t = Math.Max(t, 0);
        for (var i = 0; i < Keyframes.Count - 1; i++)
        {
            var curAnimationCurve = Keyframes[i];
            var nextAnimationCurve = Keyframes[i + 1];
            if (curAnimationCurve.time <= t && nextAnimationCurve.time >= t)
                return Evaluate((t - curAnimationCurve.time).Div(nextAnimationCurve.time - curAnimationCurve.time), curAnimationCurve, nextAnimationCurve);
        }
        return 0;
    }
    private long Evaluate(long t, FixedKeyFrame keyframe0, FixedKeyFrame keyframe1)
    {
        var dt = keyframe1.time - keyframe0.time;
        var m0 = keyframe0.outTangent.Mul(dt);
        var m1 = keyframe1.inTangent.Mul(dt);
        var t2 = t.Mul(t);
        var t3 = t2.Mul(t);
        var a = 2 * t3 - 3 * t2 + FixedMath.One;
        var b = t3 - 2 * t2 + t;
        var c = t3 - t2;
        var d = -2 * t3 + 3 * t2;
        return a.Mul(keyframe0.value) + b.Mul(m0) + c.Mul(m1) + d.Mul(keyframe1.value);
    }

    public static FixedAnimationCurve CreateFixedAnimationCurve(AnimationCurve oac)
    {
        var newfac = new FixedAnimationCurve();
        foreach (var acKey in oac.keys)
        {
            var fkf = new FixedKeyFrame(acKey.time.ToLong(), acKey.value.ToLong(), acKey.inTangent.ToLong(), acKey.outTangent.ToLong(), acKey.tangentMode);
            newfac.AddKeyFrame(fkf);
        }
        return newfac;
    }
}