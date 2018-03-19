using Lockstep;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Lockstep
{
    public static class Utility
    {
        public static void Vector3ToHundredString(this Vector3d a, StringBuilder str)
        {
            str.Append(a.x.LongToIntHundred());
            str.Append(',');
            str.Append(a.y.LongToIntHundred());
            str.Append(',');
            str.Append(a.z.LongToIntHundred());
        }
        public static void HundredStringToVector3(this string str)
        {
            Vector3d v;
            var strs = str.Split(',');
            var x = int.Parse(strs[0]);
            v.x = x.IntHundredToLong();
            var y = int.Parse(strs[1]);
            v.y = y.IntHundredToLong();
            var z = int.Parse(strs[2]);
            v.z = z.IntHundredToLong();
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
