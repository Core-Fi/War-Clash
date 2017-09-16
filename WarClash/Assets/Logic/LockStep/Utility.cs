using Lockstep;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LockStep
{
    public static class Utility
    {
        public static long IntHundredToLong(this int v)
        {
            return v * FixedMath.One / 100;
        }
        public static Vector3d Add(this Vector3d a, Vector3d b)
        {
            Vector3d v = a;
            v.Add(b);
            return v;
        }

        public static Vector3d Mul(this Vector3d a, long b)
        {
            Vector3d v = a;
            v.Mul(b);
            return v;
        }
    }
}
