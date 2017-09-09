using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Lockstep;
using UnityEngine;

public static class Utility
{
    public static StringBuilder stringbuilder = new StringBuilder();

    public static void Clear(this StringBuilder builder)
    {
        if (builder != null)
        {
            builder.Length = 0;
            builder.Capacity = 0;
        }
    }

    public static bool RoundEquals(this float f, float v)
    {
        if (Mathf.Abs(f - v) < 0.0001f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static T GetComponent<T>(this GameObject go, bool add) where T : Component
    {
        var comp = go.GetComponent<T>();
        if (comp == null && add)
        {
            comp = go.AddComponent<T>();
        }
        return comp;
    }

    public static byte[] ReadByteFromStreamingAsset(string path)
    {
        stringbuilder.Clear();
        stringbuilder.Append(Application.streamingAssetsPath);
        stringbuilder.Append("/");
        stringbuilder.Append(path);
        byte[] bytes = File.ReadAllBytes(stringbuilder.ToString());
        return bytes;
    }

    public static string ReadStringFromStreamingAsset(string path)
    {
        stringbuilder.Clear();
        stringbuilder.Append(Application.streamingAssetsPath);
        stringbuilder.Append("/");
        stringbuilder.Append(path);
        string str = File.ReadAllText(stringbuilder.ToString());
        return str;
    }

    public static byte[] Decompress(byte[] gzip)
    {
        // Create a GZIP stream with decompression mode.
        // ... Then create a buffer and write into while reading from the GZIP stream.
        using (GZipStream stream = new GZipStream(new MemoryStream(gzip),
            CompressionMode.Decompress))
        {
            const int size = 4096;
            byte[] buffer = new byte[size];
            using (MemoryStream memory = new MemoryStream())
            {
                int count = 0;
                do
                {
                    count = stream.Read(buffer, 0, size);
                    if (count > 0)
                    {
                        memory.Write(buffer, 0, count);
                    }
                } while (count > 0);
                return memory.ToArray();
            }
        }
    }

    public static bool PositionIsInRect(FixedRect rect, Vector3d basePosition, FixedQuaternion baseQuaternion, Vector3d posi)
    {
        var relativeP = posi - basePosition;
        var rotateRelativeP = FixedQuaternion.Inverse(baseQuaternion) * relativeP;
        return rect.ContainsPoint(rotateRelativeP);
    }

    public static bool PositionIsInFan(Vector3d basePosi, long radius, int angle, FixedQuaternion baseQuaternion, Vector3d posi)
    {
        var radiusSqr = radius.Mul(radius);
        var relativeP = posi-basePosi;
        if (radiusSqr > relativeP.sqrMagnitude)
        {
            var rotateRelativeP = FixedQuaternion.Inverse(baseQuaternion) * posi;
            var cosSqr = Vector3d.Dot(rotateRelativeP.Normalize(), new Vector3d(0, 0, FixedMath.One));
            var realCos = FixedMath.Trig.Cos(FixedMath.One.Div(180).Mul(FixedMath.Pi).Mul(angle/2));
            var realCosSqr = realCos.Mul(realCos);
            if (relativeP.z > 0)
            {
                if (angle > 180)
                    return true;
                return realCosSqr < cosSqr;
            }
            else
            {
                if (angle < 180)
                    return false;
                else
                {
                    return realCosSqr > cosSqr;
                }
            }
               
        }
        else
        {
            return false;
        }
    }

    public struct FixedRect
    {
        public Vector3d center;
        public long width;
        public long height;
        public bool ContainsPoint(Vector3d p)
        {
            var relativeP = p - center;
            if (relativeP.x < width && relativeP.x > -width && relativeP.z > -height && relativeP.z < height)
            {
                return true;
            }
            else return false;
        }
    }
}
