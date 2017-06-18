using Lockstep;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class FixedQuaternion
{
    public static long degToRad
    {
        get
        {
            return FixedMath.Pi.Div(FixedMath.One * 180);
        }
    }
    public static FixedQuaternion AngleAxis(long angle, Vector3d axis)
    {
        FixedQuaternion result = identity;
        var radians = angle.Mul(degToRad);
        float v = radians.ToFloat();
        radians = radians/2;
        float v2 = radians.ToFloat();
        axis.Normalize();
        axis.Mul(FixedMath.Trig.Sin(radians));
        result.x = axis.x;
        result.y = axis.y;
        result.z = axis.z;
        result.w = FixedMath.Trig.Cos(radians);
        return Normalize(result);
    }
    public static FixedQuaternion identity
    {
        get
        {
            return new FixedQuaternion(0, 0, 0, FixedMath.One);
        }
    }
    public long x, y, z, w;
    public Vector3d xyz;
    public FixedQuaternion(long x, long y, long z, long w)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.w = w;
    }
    private static FixedQuaternion INTERNAL_CALL_AngleAxis(long degress, ref Vector3d axis)
    {
        FixedQuaternion result = identity;
        var radians = degress.Mul(degToRad);
        radians = radians.Mul(FixedMath.Half);
        axis.Normalize();
        axis.Mul(FixedMath.Trig.Sin(radians));
        result.x = axis.x;
        result.y = axis.y;
        result.z = axis.z;
        float v = radians.ToFloat();
        result.w = FixedMath.Trig.Cos(radians);
        return Normalize(result);
    }
    public long Length
    {
        get
        {
            return FixedMath.Sqrt(x.Mul(x) + y.Mul(y) + z.Mul(z) + w.Mul(w));
        }
    }
    public static FixedQuaternion Normalize(FixedQuaternion q)
    {
        FixedQuaternion result;
        Normalize(ref q, out result);
        return result;
    }
    public static void Normalize(ref FixedQuaternion q, out FixedQuaternion result)
    {
        long scale = FixedMath.One.Div(q.Length);
        result = new FixedQuaternion(q.x.Mul(scale), q.y.Mul(scale), q.z.Mul(scale), q.w.Mul(scale));
    }
    public void Normalize()
    {
        long scale = FixedMath.One / this.Length;
        x = x.Mul(scale);
        y = y.Mul(scale);
        z = z.Mul(scale);
        w = w.Mul(scale);
    }
    public static Vector3d operator *(FixedQuaternion rotation1, Vector3d point1)
    {
        //long num = rotation.x*2;
        //long num2 = rotation.y*2;
        //long num3 = rotation.z*2;
        //long num4 = rotation.x .Mul( num);
        //long num5 = rotation.y .Mul( num2);
        //long num6 = rotation.z .Mul( num3);
        //long num7 = rotation.x .Mul( num2);
        //long num8 = rotation.x .Mul( num3);
        //long num9 = rotation.y .Mul( num3);
        //long num10 = rotation.w .Mul( num);
        //long num11 = rotation.w .Mul( num2);
        //long num12 = rotation.w .Mul( num3);
        //Vector3d result;
        //result.x = (FixedMath.One - (num5 + num6)).Mul(point.x) + (num7 - num12) .Mul(point.y) + (num8 + num11).Mul( point.z);
        //result.y = (num7 + num12).Mul(point.x) + (FixedMath.One - (num4 + num6)).Mul( point.y) + (num9 - num10).Mul( point.z);
        //result.z = (num8 - num11).Mul(point.x) + (num9 + num10).Mul(point.y) + (FixedMath.One - (num4 + num5)).Mul(point.z);
        //return result;
        UnityEngine.Quaternion rotation = new UnityEngine.Quaternion() { x = rotation1.x.ToFloat(), y = rotation1.y.ToFloat(), z = rotation1.z.ToFloat(), w = rotation1.w.ToFloat() };
        UnityEngine.Vector3 point = new UnityEngine.Vector3() { x = point1.x.ToFloat(), y = point1.y.ToFloat(), z = point1.z.ToFloat() };
        float num = rotation.x * 2f;
        float num2 = rotation.y * 2f;
        float num3 = rotation.z * 2f;
        float num4 = rotation.x * num;
        float num5 = rotation.y * num2;
        float num6 = rotation.z * num3;
        float num7 = rotation.x * num2;
        float num8 = rotation.x * num3;
        float num9 = rotation.y * num3;
        float num10 = rotation.w * num;
        float num11 = rotation.w * num2;
        float num12 = rotation.w * num3;
        UnityEngine.Vector3 result;
        result.x = (1f - (num5 + num6)) * point.x + (num7 - num12) * point.y + (num8 + num11) * point.z;
        result.y = (num7 + num12) * point.x + (1f - (num4 + num6)) * point.y + (num9 - num10) * point.z;
        result.z = (num8 - num11) * point.x + (num9 + num10) * point.y + (1f - (num4 + num5)) * point.z;
        return new Vector3d(result);
    }
    public override string ToString()
    {
        return x.ToDouble().ToString("0.00") + "  " + y.ToDouble().ToString("0.00") + "  " + z.ToDouble().ToString("0.00") + "  " + w.ToDouble().ToString("0.00") + "  ";
    }
}

