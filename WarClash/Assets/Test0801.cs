using System;
using System.Collections;
using System.Collections.Generic;
using Lockstep;
using Pathfinding;
using UnityEngine;

public enum VectorRelation
{
    Left,
    Inside,
    Right,
    Equal,
}
public class ReflectionCaculator 
{
    public static void CaculateReflectionPoints(FixedABPath path, List<Vector3d> points)
    {
        if (path.path.Count >= 2)
            SearchReflectionPointFromIndex(path.StartPoint, 0, path, points);
    }

    private static void AddPoint(Vector3d point, List<Vector3d> points )
    {
        if (points.Count > 0)
        {
            if(points[points.Count - 1] != point)
                points.Add(point);
        }
        else
            points.Add(point);
    }
    private static void SearchReflectionPointFromIndex(Vector3d startPosi, int index, FixedABPath path, List<Vector3d> points)
    {
        var graph = AstarPath.active.graphs[0] as RecastGraph;
        bool findStartPoint = true;
        Vector3d left = new Vector3d();
        Vector3d right = new Vector3d();
        int startPosiIndex = -1;
        for (int i = index; i < path.path.Count; i++)
        {
            if (i != 0)
            {
                TriangleMeshNode prenode = path.path[i - 1] as TriangleMeshNode;
                TriangleMeshNode curnode = path.path[i] as TriangleMeshNode;
                int a, b;
                a = prenode.SharedEdge(curnode);
                b = a ==2?0:a+1;
                Int3 l = new Int3(); Int3 r = new Int3();
                if (a != -1)
                {
                    l = prenode.GetVertex(a);
                }
                if (b != -1)
                {
                    r = prenode.GetVertex(b);
                }
                if (findStartPoint)
                {
                    left = Int3.ToVector3D(l);
                    right = Int3.ToVector3D(r);
                    findStartPoint = false;
                }
                else
                {
                    var tempL = Int3.ToVector3D(l);
                    var tempR = Int3.ToVector3D(r);
                    var relationL = GetRelation(left-startPosi, right - startPosi, tempL - startPosi);
                    var relationR = GetRelation(left - startPosi, right - startPosi, tempR - startPosi);
                    //寻找拐点
                    if (relationR == VectorRelation.Left && relationL == VectorRelation.Left)
                    {
                        AddPoint(left, points);
                        startPosi = left;
                        startPosiIndex = i;
                        //SpawnGO(left);
                        break; 
                    }
                    else if (relationR == VectorRelation.Right && relationL == VectorRelation.Right)
                    {
                        AddPoint(right, points);
                        startPosi = right;
                        startPosiIndex = i;
                        //SpawnGO(right); 
                        break;
                    }
                    else if(relationR== VectorRelation.Equal && relationL == VectorRelation.Right)
                    {
                        AddPoint(right, points);
                        startPosi = right;
                        startPosiIndex = i;
                        //SpawnGO(right); 
                        break;
                    }
                    else if (relationR == VectorRelation.Equal && relationL == VectorRelation.Left)
                    {
                        AddPoint(left, points);
                        startPosi = left;
                        startPosiIndex = i;
                        //SpawnGO(left); 
                        break;
                    }
                    else if (relationL == VectorRelation.Equal && relationR == VectorRelation.Left)
                    {
                        AddPoint(left, points);
                        startPosi = left;
                        startPosiIndex = i;
                        //SpawnGO(left);
                        break;
                    }
                    else if (relationL == VectorRelation.Equal && relationR == VectorRelation.Right)
                    {
                        AddPoint(right, points);
                        startPosi = right;
                        startPosiIndex = i;
                        //SpawnGO(right);
                        break;
                    }
                    //改变左右
                    if (relationR == VectorRelation.Inside && relationL == VectorRelation.Inside)
                    {
                        left = tempL;
                        right = tempR;
                    }
                    if (relationL == VectorRelation.Inside || relationL== VectorRelation.Equal)
                    {
                        left = tempL;
                    }
                    if (relationR == VectorRelation.Inside || relationR == VectorRelation.Equal)
                    {
                        right = tempR;
                    }
                    var relationEndPoint = GetRelation(left - startPosi, right - startPosi, path.EndPoint - startPosi);
                    if (relationEndPoint == VectorRelation.Left)
                    {
                        AddPoint(left, points);
                    }
                    else if (relationEndPoint == VectorRelation.Right)
                    {
                        AddPoint(right, points);
                    }
                }
            }
        }
        if(startPosiIndex!=-1)
            SearchReflectionPointFromIndex(startPosi, startPosiIndex, path, points);
    }

    private void SpawnGO(Vector3d posi)
    {
        var g = GameObject.CreatePrimitive(PrimitiveType.Cube);
        g.transform.position = posi.ToVector3();
    }

    private GameObject SpawnSphere(Vector3d posi)
    {
        var g = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        g.transform.position = posi.ToVector3();
        return g;
    }
    private static VectorRelation GetRelation(Vector3d left, Vector3d right, Vector3d v)
    {
        if (left == v)
        {
            return VectorRelation.Equal;
        }
        else if (right == v)
        {
            return VectorRelation.Equal;
        }
        if (IsLeftSide(left, right, v))
        {
            return VectorRelation.Left;
        }
        else if(IsInside(left, right, v))
        {
            return VectorRelation.Inside;
        }
        else
        {
            return VectorRelation.Right;
        }
    }

    static bool IsLeftSide(Vector3d left, Vector3d right, Vector3d v)
    {
        var lva = Vector3d.Cross(v, left);
        var lvb = Vector3d.Cross(left, right);
        var ldot = Vector3d.Dot(lva, lvb);
        return ldot > 0;
    }
    static bool IsRightSide(Vector3d left, Vector3d right, Vector3d v)
    {
        var lva = Vector3d.Cross(left, right);
        var lvb = Vector3d.Cross(right, v);
        var ldot = Vector3d.Dot(lva, lvb);
        return ldot > 0;
    }

    static bool IsInside(Vector3d left, Vector3d right, Vector3d v)
    {
        var lva = Vector3d.Cross(left, v);
        var lvb = Vector3d.Cross(v, right);
        var ldot = Vector3d.Dot(lva, lvb);
        return ldot > 0;
    }
}

