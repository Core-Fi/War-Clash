using Lockstep;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


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
    public long Length
    {
        get
        {
            return FixedMath.Sqrt(x.Mul(x) + y.Mul(y) + z.Mul(z) + w.Mul(w));
        }
    }
    public long lengthSquared
    {
        get
        {
            return x.Mul(x) + y.Mul(y) + z.Mul(z) + w.Mul(w);
        }
    }
    public static FixedQuaternion Inverse(FixedQuaternion rotation)
    {
        long lengthSq = rotation.lengthSquared;
        if (lengthSq != 0)
        {
            long i = FixedMath.One.Div(lengthSq);
            return new FixedQuaternion(rotation.x.Mul(-i), rotation.y.Mul(-i), rotation.z.Mul(-i), rotation.w.Mul(i));
        }
        return rotation;
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
        //UnityEngine.Quaternion rotation = new UnityEngine.Quaternion() { x = rotation1.x.ToFloat(), y = rotation1.y.ToFloat(), z = rotation1.z.ToFloat(), w = rotation1.w.ToFloat() };
        //UnityEngine.Vector3 point = new UnityEngine.Vector3() { x = point1.x.ToFloat(), y = point1.y.ToFloat(), z = point1.z.ToFloat() };
        //float num = rotation.x * 2f;
        //float num2 = rotation.y * 2f;
        //float num3 = rotation.z * 2f;
        //float num4 = rotation.x * num;
        //float num5 = rotation.y * num2;
        //float num6 = rotation.z * num3;
        //float num7 = rotation.x * num2;
        //float num8 = rotation.x * num3;
        //float num9 = rotation.y * num3;
        //float num10 = rotation.w * num;
        //float num11 = rotation.w * num2;
        //float num12 = rotation.w * num3;
        //UnityEngine.Vector3 result;
        //result.x = (1f - (num5 + num6)) * point.x + (num7 - num12) * point.y + (num8 + num11) * point.z;
        //result.y = (num7 + num12) * point.x + (1f - (num4 + num6)) * point.y + (num9 - num10) * point.z;
        //result.z = (num8 - num11) * point.x + (num9 + num10) * point.y + (1f - (num4 + num5)) * point.z;
        long f_num = rotation1.x * 2;
        long f_num2 = rotation1.y * 2;
        long f_num3 = rotation1.z * 2;
        long f_num4 = rotation1.x.Mul(f_num);
        long f_num5 = rotation1.y .Mul( f_num2);
        long f_num6 = rotation1.z .Mul( f_num3);
        long f_num7 = rotation1.x .Mul( f_num2);
        long f_num8 = rotation1.x .Mul( f_num3);
        long f_num9 = rotation1.y .Mul( f_num3);
        long f_num10 = rotation1.w .Mul( f_num);
        long f_num11 = rotation1.w .Mul( f_num2);
        long f_num12 = rotation1.w .Mul( f_num3);
        Vector3d f_result;
        f_result.x = (FixedMath.One - (f_num5 + f_num6)).Mul( point1.x )+ (f_num7 - f_num12).Mul(point1.y) + (f_num8 + f_num11).Mul( point1.z);
        f_result.y = (f_num7 + f_num12).Mul(point1.x) + (FixedMath.One - (f_num4 + f_num6)).Mul(point1.y) + (f_num9 - f_num10).Mul(point1.z);
        f_result.z = (f_num8 - f_num11).Mul(point1.x) + (f_num9 + f_num10).Mul(point1.y) + (FixedMath.One - (f_num4 + f_num5)).Mul(point1.z);
        return f_result;
    }
    public static FixedQuaternion LookRotation(Vector3d forward, Vector3d up)
    {
        Vector3d vector =forward;
        Vector3d vector2 = Vector3d.Normalize(Vector3d.Cross(up, vector));
        Vector3d vector3 = Vector3d.Cross(vector, vector2);
        var m00 = vector2.x;
        var m01 = vector2.y;
        var m02 = vector2.z;
        var m10 = vector3.x;
        var m11 = vector3.y;
        var m12 = vector3.z;
        var m20 = vector.x;
        var m21 = vector.y;
        var m22 = vector.z;


        var num8 = (m00 + m11) + m22;
        var quaternion = FixedQuaternion.identity;
        if (num8 > 0)
        {
            var num = FixedMath.Sqrt(num8 + FixedMath.One);
            quaternion.w = num /2;
            num = FixedMath.Half.Div(num);
            quaternion.x = (m12 - m21).Mul(num);
            quaternion.y = (m20 - m02).Mul(num);
            quaternion.z = (m01 - m10).Mul(num);
            return quaternion;
        }
        if ((m00 >= m11) && (m00 >= m22))
        {
            var num7 = FixedMath.Sqrt(((FixedMath.One + m00) - m11) - m22);
            var num4 = FixedMath.Half.Div(num7);
            quaternion.x = num7/2;
            quaternion.y = (m01 + m10).Mul(num4);
            quaternion.z = (m02 + m20).Mul(num4);
            quaternion.w = (m12 - m21).Mul(num4);
            return quaternion;
        }
        if (m11 > m22)
        {
            var num6 = FixedMath.Sqrt(((FixedMath.One + m11) - m00) - m22);
            var num3 = FixedMath.Half.Div(num6);
            quaternion.x = (m10 + m01).Mul(num3);
            quaternion.y =  num6/2;
            quaternion.z = (m21 + m12).Mul(num3);
            quaternion.w = (m20 - m02).Mul(num3);
            return quaternion;
        }
        var num5 = FixedMath.Sqrt(((FixedMath.One + m22) - m00) - m11);
        var num2 = FixedMath.Half.Div(num5);
        quaternion.x = (m20 + m02) .Mul( num2);
        quaternion.y = (m21 + m12).Mul(num2);
        quaternion.z = num5/2;
        quaternion.w = (m01 - m10).Mul(num2);
        return quaternion;
    }
    public override string ToString()
    {
        return x.ToDouble().ToString("0.00") + "  " + y.ToDouble().ToString("0.00") + "  " + z.ToDouble().ToString("0.00") + "  " + w.ToDouble().ToString("0.00") + "  ";
    }
}

