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
    public static StringBuilder Stringbuilder = new StringBuilder();
    public static readonly List<IFixedAgent> List = new List<IFixedAgent>();
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

    public static byte[] ReadBytesFromAbsolutePath(string path)
    {
#if UNITY_EDITOR
        byte[] bytes = File.ReadAllBytes(path);
        return bytes;
#elif UNITY_ANDROID
        WWW www = new WWW(path);
        while (!www.isDone)
        {
        }
        var bytes = www.bytes;
        www.Dispose();
        return bytes;
#endif
        return null;
    }
    public static byte[] ReadByteFromStreamingAsset(string path)
    {
        var filePath = Path.Combine(Application.streamingAssetsPath, path);
        return ReadBytesFromAbsolutePath(filePath);
    }

    public static string ReadStringFromAbsolutePath(string path)
    {
#if UNITY_EDITOR
        var str = File.ReadAllText(path);
        return str;

#elif UNITY_ANDROID
        WWW www = new WWW(path);
        while (!www.isDone)
        {
        }
        var str = www.text;
        www.Dispose();
        return str;
#endif
        return null;
    }
    public static string ReadStringFromStreamingAsset(string path)
    {
        var filePath = Path.Combine(Application.streamingAssetsPath, path);
        return ReadStringFromAbsolutePath(filePath);
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
#if UNITY_EDITOR
        Vector3d leftDown = baseQuaternion * (new Vector3d(rect.center)+new Vector3d(-rect.width/2, 0, -rect.height / 2)) + basePosition;
        Vector3d rightDown = baseQuaternion * (new Vector3d(rect.center) + new Vector3d(rect.width / 2, 0, -rect.height / 2)) + basePosition;
        Vector3d leftUp = baseQuaternion * (new Vector3d(rect.center) + new Vector3d(-rect.width / 2, 0, rect.height / 2)) + basePosition;
        Vector3d rightUp = baseQuaternion * (new Vector3d(rect.center) + new Vector3d(rect.width / 2, 0, rect.height / 2)) + basePosition;

        Debug.DrawLine(leftDown.ToVector3(), leftUp.ToVector3(), Color.green, 1);
        Debug.DrawLine(leftUp.ToVector3(), rightUp.ToVector3(), Color.green, 1);
        Debug.DrawLine(rightUp.ToVector3(), rightDown.ToVector3(), Color.green, 1);
        Debug.DrawLine(rightDown.ToVector3(), leftDown.ToVector3(), Color.green, 1);

#endif
        var relativeP = posi - basePosition;
        var rotateRelativeP = FixedQuaternion.Inverse(baseQuaternion) * relativeP;
        return rect.ContainsPoint(rotateRelativeP);
    }

    public static bool PositionIsInFan(Vector3d basePosi, long radius, int angle, FixedQuaternion baseQuaternion, Vector3d posi)
    {
#if UNITY_EDITOR
        var p1_sin = FixedMath.Trig.Sin(FixedMath.One.Div(180).Mul(FixedMath.Pi).Mul(angle/2));
        var p1_cos = FixedMath.Trig.Cos(FixedMath.One.Div(180).Mul(FixedMath.Pi).Mul(angle / 2));
        var p1 = new Vector3d(p1_sin.Mul(radius), 0, p1_cos.Mul(radius));
        var worldP1 = baseQuaternion*p1 + basePosi;

        var p2_sin = FixedMath.Trig.Sin(FixedMath.One.Div(180).Mul(FixedMath.Pi).Mul(-angle / 2));
        var p2_cos = FixedMath.Trig.Cos(FixedMath.One.Div(180).Mul(FixedMath.Pi).Mul(-angle / 2));
        var p2 = new Vector3d(p2_sin.Mul(radius), 0, p2_cos.Mul(radius));
        var worldP2 = baseQuaternion * p2 + basePosi;
        Debug.DrawLine(basePosi.ToVector3(), worldP1.ToVector3(), Color.red, 3);
        Debug.DrawLine(basePosi.ToVector3(), worldP2.ToVector3(), Color.red,3);

#endif
        var radiusSqr = radius.Mul(radius);
        var relativeP = posi-basePosi;
        if (radiusSqr > relativeP.sqrMagnitude)
        {
            var rotateRelativeP = FixedQuaternion.Inverse(baseQuaternion) * relativeP;
            var cos = Vector3d.Dot(rotateRelativeP.Normalize(), new Vector3d(0, 0, FixedMath.One));
            var realCos = FixedMath.Trig.Cos(FixedMath.One.Div(180).Mul(FixedMath.Pi).Mul(angle/2));
            if (relativeP.z > 0)
            {
                if (angle > 180)
                    return true;
                return realCos < cos;
            }
            else
            {
                if (angle < 180)
                    return false;
                else
                {
                    return realCos > cos;
                }
            }
               
        }
        else
        {
            return false;
        }
    }
    public static FixedRect MinMaxRect(long xmin, long ymin, long xmax, long ymax)
    {
        return new FixedRect(xmin, ymin, xmax - xmin, ymax - ymin);
    }

    public static long Min(long a, long b)
    {
        return a > b ? b : a;
    }

    public static long Max(long a, long b)
    {
        return a < b ? b : a;
    }
    public struct FixedRect
    {
        public Vector2d center;
        public long xMin;
        public long yMin;
        public long xMax;
        public long yMax;
        public long width;
        public long height;

        public FixedRect(Vector2d center, long width, long height)
        {
            this.center = center;
            this.width = width;
            this.height = height;
            xMin = center.x - width / 2;
            xMax = center.x + width / 2;
            yMin = center.y - height / 2;
            yMax = center.y + height / 2;
        }
        public FixedRect(long xmin, long ymin, long width, long height)
        {
            xMin = xmin;
            yMin = ymin;
            this.width = width;
            this.height = height;
            xMax = xmin + width;
            yMax = ymin + height;
            center.x = xmin + width / 2;
            center.y = ymin + height / 2;
        }
        public bool ContainsPoint(Vector3d p)
        {
            if (p.x < xMax && p.x > xMin && p.z > yMin && p.z < yMax)
            {
                return true;
            }
            else return false;
        }
        public bool ContainsPoint(Vector2d p)
        {
            if (p.x < xMax && p.x > xMin && p.y > yMin && p.y < yMax)
            {
                return true;
            }
            else return false;
        }
    }
}
