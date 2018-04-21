using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FastCollections;
using Logic.Skill;
using UnityEngine;
using Lockstep;

namespace Logic.Map
{
    public class Map
    {
        public int Width
        {
            get
            {
                return Data.Width;
            }
        }
        public int Height
        {
            get
            {
                return Data.Height;
            }
        }
        public StageType[,] MapData { get; private set; }
        public MapData Data { get; private set; }
        public Map(MapData data)
        {
            Data = data;
            MapData = new StageType[Data.Height, Data.Width];
            for (int i = 0; i < data.Data.Count; i++)
            {
                var stage = data.Data[i];
                MapData[stage.Y, stage.X] = stage.Type;
            }
        }
        public Vector3d GetProperPosi(Vector3d v1, Vector3d v2, out bool hitx, out bool hity)
        {
            var offset = v2 - v1;
            long x = v1.x, y =v1.y;
            hitx = hity = false;
            if(offset.x>0)
            {
                var testPosi = v1 + new Vector3d(offset.x + FixedMath.Half, 0, 0);
                if (IsInObstacle(testPosi))
                {
                    hitx = true;
                    x = FixedMath.Floor(testPosi.x) - FixedMath.One/2 - 1;
                    DLog.Log(x.ToFloat()+" "+v1.ToString()+" "+v2);
                }
            }
            else if (offset.x < 0)
            {
                var testPosi = v1 + new Vector3d(offset.x - FixedMath.Half, 0, 0);
                if (IsInObstacle(testPosi))
                {
                    hitx = true;
                    x = FixedMath.Floor(testPosi.x) + FixedMath.One/2 + 1;
                }
            }
            if(offset.y>0)
            {
                var testPosi = v1 + new Vector3d(0, offset.y + FixedMath.One * 2, 0);
                if (IsInObstacle(testPosi))
                {
                    hity = true;
                    y = FixedMath.Floor(testPosi.y) - FixedMath.One * 2 - 1;
                }
            }
            else if (offset.y < 0)
            {
                var testPosi = v1 + new Vector3d(0, offset.y, 0);
                if (IsInObstacle(testPosi))
                {
                    hity = true;
                    y = FixedMath.Floor(testPosi.y) + FixedMath.One + 1;
                }
            }
            Vector3d v = new Vector3d(hitx? x : v2.x, hity?y:v2.y, 0);
            return v;
        }

        public bool IsInObstacle(Vector3d v)
        {
            var x = v.x.FloorToInt();
            var y = v.y.FloorToInt();
            if (x < 0 || y < 0 || x >= Width || y >= Height)
            {
                return false;
            }
            if(MapData[y,x] == StageType.Clear)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        private static long Sign(Vector2d p1, Vector2d p2, Vector2d p3)
        {
            return (p1.x - p3.x) * (p2.y - p3.y) - (p2.x - p3.x) * (p1.y - p3.y);
        }

        public static bool PointInTriangle(Vector2d pt, Vector2d v1, Vector2d v2, Vector2d v3)
        {
            bool b1, b2, b3;

            b1 = Sign(pt, v1, v2) < 0;
            b2 = Sign(pt, v2, v3) < 0;
            b3 = Sign(pt, v3, v1) < 0;

            return ((b1 == b2) && (b2 == b3));
        }
        public static Vector2d ClosestPointOnTriangle(Vector2d a, Vector2d b, Vector2d c, Vector2d p)
        {
            // Check if p is in vertex region outside A
            var ab = b - a;
            var ac = c - a;
            var ap = p - a;

            var d1 = Vector2d.Dot(ab, ap);
            var d2 = Vector2d.Dot(ac, ap);

            // Barycentric coordinates (1,0,0)
            if (d1 <= 0 && d2 <= 0)
            {
                return a;
            }

            // Check if p is in vertex region outside B
            var bp = p - b;
            var d3 = Vector2d.Dot(ab, bp);
            var d4 = Vector2d.Dot(ac, bp);

            // Barycentric coordinates (0,1,0)
            if (d3 >= 0 && d4 <= d3)
            {
                return b;
            }

            // Check if p is in edge region outside AB, if so return a projection of p onto AB
            if (d1 >= 0 && d3 <= 0)
            {
                var vc = d1.Mul(d4) - d3.Mul(d2);
                if (vc <= 0)
                {
                    // Barycentric coordinates (1-v, v, 0)
                    var v = d1.Div(d1 - d3);
                    return a + ab.Mul(v);
                }
            }

            // Check if p is in vertex region outside C
            var cp = p - c;
            var d5 = Vector2d.Dot(ab, cp);
            var d6 = Vector2d.Dot(ac, cp);

            // Barycentric coordinates (0,0,1)
            if (d6 >= 0 && d5 <= d6)
            {
                return c;
            }

            // Check if p is in edge region of AC, if so return a projection of p onto AC
            if (d2 >= 0 && d6 <= 0)
            {
                var vb = d5.Mul(d2) - d1.Mul(d6);
                if (vb <= 0)
                {
                    // Barycentric coordinates (1-v, 0, v)
                    var v = d2.Div(d2 - d6);
                    return a + ac.Mul(v);
                }
            }

            // Check if p is in edge region of BC, if so return projection of p onto BC
            if ((d4 - d3) >= 0 && (d5 - d6) >= 0)
            {
                var va = d3.Mul(d6) - d5.Mul(d4);
                if (va <= 0)
                {
                    var v = (d4 - d3).Div((d4 - d3) + (d5 - d6));
                    return b + (c - b).Mul(v);
                }
            }

            return p;
        }
        public static Map Deserialize(string name)
        {
            var str = System.IO.File.ReadAllText("D://stageConf.txt");// Utility.ReadStringFromStreamingAsset("Map/" + name + ".map");
            if (!string.IsNullOrEmpty(str))
            {
                var data = Newtonsoft.Json.JsonConvert.DeserializeObject<MapData>(str, SkillUtility.settings);
                Map map = new Map(data);
                return map;
            }
            return null;
        }

    }
}
