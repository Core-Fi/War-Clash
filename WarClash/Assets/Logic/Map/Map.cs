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
                if (IsInObstacle(v1 + new Vector3d(offset.x + FixedMath.One/2, 0, 0)))
                {
                    hitx = true;
                    if (offset.x < 0)
                    {
                        x = FixedMath.Floor(v2.x) + FixedMath.One/2 + 1;
                    }
                    else
                    {
                        x = FixedMath.Floor(v2.x) - FixedMath.One/2 - 1;
                    }
                }
            }
            else if (offset.x < 0)
            {
                if (IsInObstacle(v1 + new Vector3d(offset.x-FixedMath.One/2, 0, 0)))
                {
                    hitx = true;
                    if (offset.x < 0)
                    {
                        x = FixedMath.Floor(v2.x) + FixedMath.One/2 + 1;
                    }
                    else
                    {
                        x = FixedMath.Floor(v2.x) - FixedMath.One/2 - 1;
                    }
                }
            }
            if(offset.y>0)
            {
                if (IsInObstacle(v1 + new Vector3d(0, offset.y+FixedMath.One*2, 0)))
                {
                    hity = true;
                    if (offset.y < 0)
                    {
                        y = FixedMath.Floor(v2.y) + FixedMath.One + 1;
                    }
                    else
                    {
                        y = FixedMath.Floor(v2.y) - FixedMath.One * 2 - 1;
                    }
                }
            }
            else if (offset.y < 0)
            {
                if (IsInObstacle(v1 + new Vector3d(0, offset.y, 0)))
                {
                    hity = true;
                    if (offset.y < 0)
                    {
                        y = FixedMath.Floor(v2.y) + FixedMath.One + 1;
                    }
                    else
                    {
                        y = FixedMath.Floor(v2.y) - FixedMath.One * 2 - 1;
                    }
                }
            }
            Vector3d v = new Vector3d(hitx? x : v2.x, hity?y:v2.y, 0);
            return v;
        }

        public bool IsInObstacle(Vector3d v)
        {
            var x = v.x.FloorToInt();
            var y = v.y.FloorToInt();
            if(MapData[y,x] == StageType.Clear)
            {
                return false;
            }
            else
            {
                return true;
            }
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
