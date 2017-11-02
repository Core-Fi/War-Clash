using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Lockstep;
using Logic;
using UnityEngine.VR;
public struct FixedLine
{
    public Vector2d direction;
    public Vector2d point;
}
public class FixedAgent {
    bool inited = false;
    private Vector2d _fixedPrefVelocity;
    float maxVelocity;
    List<ISteering> neighborsList = new List<ISteering>();
    List<FixedLine> lines = new List<FixedLine>();
    private List<FixedLine> _fixedLines = new List<FixedLine>();
  
    void getNeighbors()
    {
        LogicCore.SP.SceneManager.CurrentScene.FixedQuadTree.Query(_self, FixedMath.One * 4, neighborsList);
    }

    private Vector2d _fixedVecocity;

    Vector2d linearPrograming(List<FixedLine> lines)
    {
        _fixedVecocity = _self.Velocity.ToVector2d();
        _fixedPrefVelocity = _fixedVecocity.Normalize() * _self.Speed;
        Vector2d newV = _fixedPrefVelocity;

        // return curVelocity todo: 3D Linear Programming
        for (int i = 0; i < lines.Count; ++i)
        {
            var line1 = lines[i];
            var d = line1.direction;
            var p = line1.point;
            if (Vector2d.det(d, p - _fixedVecocity) > 0)
            {
                // ||d||^2 * t^2 + 2(dot(d, p)) * t + (||p||^2 - r^2) >= 0  available area in max speed
                // delta = b^2 - 4ac, ||d|| = 1
                var dp = Vector2d.Dot(p, d);
                var delta = 4 * dp.Mul(dp) - 4 * (p.SqrMagnitude() - _self.Speed.Mul(_self.Speed));
                if (delta < 0)
                {
                    newV = _fixedVecocity;
                    break;
                }
                delta /= 4;

                // m1 = (-b - sqrt(delta)) / ||d||^2
                // m2 = (-b + sqrt(delta)) / ||d||^2
                // m1 <= m2
                var m1 = -dp - FixedMath.Sqrt(delta);
                var m2 = -dp + FixedMath.Sqrt(delta);
                // Cramer's rule
                // p = p1 + t1d1 = p2 + t2d2
                for (var j = 0; j < i; ++j)
                {
                    var line2 = lines[j];

                    // t1 = det(d2, p1 - p2) / det(d1, d2)
                    var t1y = Vector2d.det(line1.direction, line2.direction);
                    var t1x = Vector2d.det(line2.direction, p - line2.point);
                    // parallel
                    if (Math.Abs(t1y) < 100)
                    {
                        // on the other side of available area
                        if (t1x < 0)
                        {
                            newV = _fixedVecocity;
                            return newV;
                        }
                        continue;
                    }

                    var t = t1y.Div(t1x);
                    // right side of line1, mod m1
                    if (t1y < 0)
                    {
                        m1 = Math.Max(m1, t);
                    }
                    else
                    {
                        m2 = Math.Min(m2, t);
                    }
                    //      Debug.LogError("fixed " + m1.ToFloat()+" "+m2.ToFloat());
                    if (m1 > m2)
                    {
                        newV = _fixedVecocity;
                        return newV;
                    }
                }

                var tPref = Vector2d.Dot(d, _fixedPrefVelocity - p);
                if (tPref > m2)
                {
                    newV = p + m2 * d;
                }
                else if (tPref < m1)
                {
                    newV = p + m1 * d;
                }
                else
                {
                    newV = p + tPref * d;
                }
                _fixedVecocity = newV;
            }
        }

        return newV;
    }

    void computeNewFixedVelocity()
    {
        _fixedLines.Clear();

        for (var i = 0; i < neighborsList.Count; ++i)
        {
            var neighbor = neighborsList[i];

            var relativePos = neighbor.Position.ToVector2d() - _self.Position.ToVector2d();
            var mRelativePos = relativePos.Magnitude();
            //flag
            var relativeVel = _self.Velocity.ToVector2d() - neighbor.Velocity.ToVector2d();
            var comRadius = neighbor.Radius + _self.Radius;

            FixedLine line = new FixedLine();
            Vector2d u = Vector2d.zero;

            if (comRadius < mRelativePos)
            {
                Vector2d vec = relativeVel - relativePos / FixedMath.Create(6);
                var mVec = vec.Magnitude();
                var dotProduct = Vector2d.Dot(vec, relativePos);
                if (dotProduct < 0 && dotProduct.Mul(dotProduct) > comRadius.Mul(comRadius) * mVec.Mul(mVec))
                {
                    Vector2d normVec = vec.Normalize();
                    line.direction = new Vector2d(normVec.y, -normVec.x);
                    u = normVec * (comRadius.Div(FixedMath.Create(6)) - mVec);
                }
                else
                {
                    long mEdge = FixedMath.Sqrt(mRelativePos.Mul(mRelativePos) - comRadius.Mul(comRadius));
                    Vector2d edge = Vector2d.zero;
                    if (Vector2d.det(relativePos, vec) > 0)
                    {
                        edge = Vector2d.rotate(relativePos, mEdge.Div(mRelativePos), comRadius.Div(mRelativePos));
                    }
                    else
                    {
                        edge = Vector2d.rotate(relativePos, mEdge.Div(mRelativePos), -comRadius.Div(mRelativePos));
                        edge *= -1;
                    }
                    line.direction = edge.Normalize();
                    // project   u' = v * dot(u, v) / v^2
                    // v = ||line.direction|| = 1
                    u = Vector2d.Dot(relativeVel, line.direction) * line.direction - relativeVel;
                }
            }
            // tricks
            else
            {
                Vector2d vec = relativeVel - relativePos / LockFrameMgr.FixedFrameTime;
                Vector2d normVec = vec.Normalize();
                line.direction = new Vector2d(normVec.y, -normVec.x);
                u = (comRadius.Div(LockFrameMgr.FixedFrameTime) - vec.Magnitude()) * normVec;
            }

            line.point = _self.Velocity.ToVector2d() + u / 2;
            _fixedLines.Add(line);
        }

        _fixedVecocity = linearPrograming(_fixedLines);
    }
   

    public Vector2d UpdateAgent()
    {
        getNeighbors();
        computeNewFixedVelocity();
        return _fixedVecocity;
    }
    public FixedAgent(ISteering self)
    {
        _self = self;
    }
    private ISteering _self;
}
