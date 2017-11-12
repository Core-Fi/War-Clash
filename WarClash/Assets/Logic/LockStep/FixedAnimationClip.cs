using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lockstep;
using Logic;
using UnityEngine;

class FixedAnimationClip
{
    public long Length;
    public FixedAnimationCurve LocalPositionX;
    public FixedAnimationCurve LocalPositionY;
    public FixedAnimationCurve LocalPositionZ;
    //public FixedAnimationCurve LocalRotationX;
    //public FixedAnimationCurve LocalRotationY;
    //public FixedAnimationCurve LocalRotationZ;
    public Transform Transform;

    private long _curTime;
    public void Update()
    {
        if (_curTime > Length)
        {
            return;
        }
        long lx = LocalPositionX.Evaluate(_curTime);
        long ly = LocalPositionY.Evaluate(_curTime);
        long lz = LocalPositionZ.Evaluate(_curTime);
        Transform.localPosition = new Vector3(lx.ToFloat(), ly.ToFloat(), lz.ToFloat());
        _curTime += FixedMath.Create(Time.deltaTime);
    }
#if UNITY_EDITOR
    public static FixedAnimationClip CreateFixedAnimationClip(AnimationClip clip)
    {
        FixedAnimationClip fixedAnimationClip = new FixedAnimationClip();
        //fixedAnimationClip.Transform = transform;
        fixedAnimationClip.Length = FixedMath.Create(clip.length);
        var clips = UnityEditor.AnimationUtility.GetCurveBindings(clip);
        for (int i = 0; i < clips.Length; i++)
        {
            var c = clips[i];
            var ocurve = UnityEditor.AnimationUtility.GetEditorCurve(clip, c);
            var fcurve = FixedAnimationCurve.CreateFixedAnimationCurve(ocurve);
            if (c.propertyName.Equals("m_LocalPosition.x"))
            {
                fixedAnimationClip.LocalPositionX = fcurve;
            }
            else if (c.propertyName.Equals("m_LocalPosition.y"))
            {
                fixedAnimationClip.LocalPositionY = fcurve;
            }
            else if (c.propertyName.Equals("m_LocalPosition.z"))
            {
                fixedAnimationClip.LocalPositionZ = fcurve;
            }
        }
        return fixedAnimationClip;
    }
#endif

}
