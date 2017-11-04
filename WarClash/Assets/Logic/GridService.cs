﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lockstep;
using Logic;
using Logic.LogicObject;
using UnityEngine;

struct GridData
{
    public SceneObject So;
    public int Value;

    public void Clear()
    {
        So = null;
        Value = 0;
    }
}

public enum NodeType
{
    Empty = 0,
    FlagAsTarget = 1,
    BeTaken = 2,
}
class GridService
{
    private static GridData[,] _gridData;
    private static Vector3d Offset = new Vector3d(FixedMath.One/2,0,FixedMath.One/2);
    private static long CellSize;
    public static void Init(int width, int height, long cellSize)
    {
        CellSize = cellSize;
        _gridData = new GridData[height, width];
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                _gridData[i,j] = new GridData();
            }
        }
    }
    public static bool IsEmpty(int x, int y)
    {
        return _gridData[y, x].Value == 0;
    }
    public static SceneObject IsNotEmptyBy(int x, int y)
    {
        if (_gridData[y, x].Value  != 0)
        {
            return _gridData[y, x].So;
        }
        return null;
    }
    public static SceneObject IsNotEmptyBy(Vector3d posi)
    {
        int x, y;
        GetCoordinate(posi, out x, out y);
        return IsNotEmptyBy(x,y);
    }
    public static void TagAs(int x, int y, SceneObject so, NodeType type)
    {
        if (IsEmpty(x,y))
        {
            _gridData[y, x].Value += (int)type;
            _gridData[y, x].So = so;
        }
        else
        {
            if (_gridData[y, x].So == so)
            {
                if ((_gridData[y, x].Value & (int) type) == 0)
                {
                    _gridData[y, x].Value += (int)type;
                }
            }
        }
    }
    public static void Clear(Vector3d posi, SceneObject so)
    {
        int x, y;
        GetCoordinate(posi, out x, out y);
        if (IsNotEmptyBy(x,y)==so)
            _gridData[y, x].Clear();
    }
    public static void TagAs(Vector3d posi, SceneObject so, NodeType type)
    {
        int x, y;
        GetCoordinate(posi, out x, out y);
        TagAs(x, y, so, type);
    }

    public static void UnTagAs(int x, int y, SceneObject so, NodeType type)
    {
        if (!IsEmpty(x, y))
        {
            if (_gridData[y, x].So == so)
            {
                if ((_gridData[y, x].Value & (int)type) != 0)
                {
                    _gridData[y, x].Value -= (int)type;
                }
            }
        }
    }
    public static void UnTagAs(Vector3d posi, SceneObject so, NodeType type)
    {
        int x, y;
        GetCoordinate(posi, out x, out y);
        UnTagAs(x, y, so, type);
    }
    public static bool SearchNearCircleEmptyPoint(Vector3d selfPosi, Vector3d posi, int radius, out Vector3d target)
    {
        int x, y;
        GetCoordinate(posi, out x, out y);
        int selfx, selfy;
        GetCoordinate(selfPosi, out selfx, out selfy);
        if (IsEmpty(x, y))
        {
            target = new Vector3d(FixedMath.Create(x), 0, FixedMath.Create(y)) + Offset;
            return true;
        }
        int searchRadius = 1+radius;
        int nearestx = 0, nearesty = 0;
        for (int i = -searchRadius ; i <= searchRadius ; i++)
        {
            for (int j = -searchRadius; j <= searchRadius; j++)
            {
                if (i != searchRadius && i != -searchRadius)
                {
                    if (j == searchRadius || j == -searchRadius)
                    {
                        int tempx = j + x;
                        int tempy = i + y;
                        if (IsEmpty(tempx, tempy))
                        {
                            if (nearestx == 0)
                            {
                                nearestx = j;
                                nearesty = i;
                            }
                            else
                            {
                                if ((Math.Abs(selfx - nearestx-x) + Math.Abs(selfy - nearesty-y)) >
                                    (Math.Abs(selfx - tempx) + Math.Abs(selfy - tempy)))
                                {
                                    nearestx = j;
                                    nearesty = i;
                                }
                            }
                           
                        }
                    }
                }
                else
                {
                    int tempx = j + x;
                    int tempy = i + y;
                    if (IsEmpty(tempx, tempy))
                    {
                        if (nearestx == 0)
                        {
                            nearestx = j;
                            nearesty = i;
                        }
                        else
                        {
                            if ((Math.Abs(selfx - nearestx - x) + Math.Abs(selfy - nearesty - y)) >
                                (Math.Abs(selfx - tempx) + Math.Abs(selfy - tempy)))
                            {
                                nearestx = j;
                                nearesty = i;
                            }
                        }
                    }
                }
            }
        }

        if (nearestx != 0 || nearesty!=0)
        {
            var diff = FixedMath.Create((searchRadius * searchRadius * 2 - nearestx * nearestx - nearesty * nearesty));
            diff = diff.Div((searchRadius * searchRadius));
            diff = (diff - FixedMath.Half) * 9 / 10;
            target = (new Vector3d(FixedMath.Create(nearestx + x) + Convert(nearestx, diff), 0,
                          FixedMath.Create(nearesty + y) + Convert(nearesty, diff)) + Offset) * CellSize;
            return true;
        }
        var randomAngle = LogicCore.SP.LockFrameMgr.RandomRange(0, 360);
        long cos = FixedMath.Trig.Cos(FixedMath.One.Div(180).Mul(FixedMath.Pi).Mul(randomAngle));
        long sin = FixedMath.Trig.Sin(FixedMath.One.Div(180).Mul(FixedMath.Pi).Mul(randomAngle));
        target = new Vector3d(cos.Mul(searchRadius), 0, sin.Mul(searchRadius)) +posi + Offset;
        return false;
    }

    private static long Convert(int v, long mag)
    {
        if (v >= 0)
        {
            return mag;
        }
        else
        {
            return -mag;
        }
    }

    public static void GetCoordinate(Vector3d posi, out int x, out int y)
    {
         x = posi.x.Div(CellSize).ToInt();
         y = posi.z.Div(CellSize).ToInt();
    }
    public static bool SearchNearEmptyPoint(Vector3d posi, out Vector3d target)
    {
        int x, y;
        GetCoordinate(posi, out x, out y);
        if (IsEmpty(x, y))
        {
            target = new Vector3d(FixedMath.Create(x), 0, FixedMath.Create(y)) + Offset;
            return true;
        }
        int radius = 1;
        while (true)
        {
            for (int i = radius; i >= -radius ; i--)
            {
                for (int j = radius ; j >= -radius ; j--)
                {
                    if (i != radius  && i != -radius )
                    {
                        if (j == radius  || j == -radius )
                        {
                            if (IsEmpty(j + x, i + y))
                            {
                                 target =(new Vector3d(FixedMath.Create(j + x), 0, FixedMath.Create(i + y))+ Offset)*CellSize;
                                return true;
                            }
                        }
                    }
                    else
                    {
                        if (IsEmpty(j + x, i + y))
                        {
                            target = (new Vector3d(FixedMath.Create(j + x), 0, FixedMath.Create(i + y)) + Offset) * CellSize;
                            return true;
                        }
                    }
                }
            }
            radius++;
            if (radius > 10)
            {
                target = posi;
                return false;
            }
        }
    }
    public static void DrawGizmos()
    {
        for (int i = 0; i < 20; i++)
        {
            for (int j = 0; j < 20; j++)
            {
                if (!IsEmpty(j, i))
                {
                    Gizmos.DrawCube(new Vector3(j,0,i)+Offset.ToVector3(), Vector3.one-new Vector3(0,0.9f,0));
                }
            }
        }
    }
}



