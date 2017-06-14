using UnityEngine;
using System.Reflection;
using System;

namespace Lockstep
{

    public class LockstepAttribute : Attribute
    {
        public LockstepAttribute()
        {
           
        }
    }
    public class FixedNumberAttribute : Attribute
    {
        public bool Timescaled = false;
        public bool Ranged;
        public long Min;
        public long Max;

        public FixedNumberAttribute(bool timescaled = false, bool ranged = false, long min = 0, long max = FixedMath.One)
        {
            Timescaled = timescaled;
            Ranged = ranged;
            Max = max;
            Min = min;
        }
    }

    public class FixedNumberAngleAttribute : Attribute
    {
        public bool Timescaled;
        public double Max;

        public FixedNumberAngleAttribute(bool timescaled = false, double max = -1d)
        {
            Timescaled = timescaled;
            Max = max;
        }

    }

    public class FrameCountAttribute : Attribute
    {
        public bool IsRate;
        /// <summary>
        /// Initializes a new instance of the <see cref="Lockstep.FrameCountAttribute"/> class.
        /// </summary>
        /// <param name="isRate">Is this FrameCount a rate (true) or a count?</param>
        public FrameCountAttribute (bool isRate = false) {
            this.IsRate = isRate;
        }
    }

    public class VisualizeAttribute : Attribute
    {
    }

    public class VectorRotationAttribute : Attribute
    {

        public bool Timescaled {get; private set;}
        public VectorRotationAttribute (bool timescaled) {
            Timescaled = timescaled;
        }
    }

    public class LocalVisualizeAttribute : Attribute
    {

    }

    /// <summary>
    /// Allows customization of a derived ActiveAbility in the database.
    /// </summary>
    public class CustomActiveAbilityAttribute :System.Attribute {

    }

    public class DataCodeAttribute : Attribute
    {
        public string TargetDataName { get; private set; }

        public DataCodeAttribute(string targetDataName)
        {
            this.TargetDataName = targetDataName;
        }

    }

    public class EnumMaskAttribute : Attribute
    {
    
    }
}