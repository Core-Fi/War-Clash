using System;
using System.Collections.Generic;
using Lockstep;
using Logic;
using UnityEngine;

internal class SeperationSteering : BaseSteering
{
    private readonly List<IFixedAgent> _neighbors = new List<IFixedAgent>();

    private List<Vector2d> testOffset = new List<Vector2d>
    {
        new Vector2d(new Vector2(-1, -1)),
        new Vector2d(new Vector2(-1, -1)),
        new Vector2d(new Vector2(-1, 0)),
        new Vector2d(new Vector2(-1, 1)),
        new Vector2d(new Vector2(0, -1)),
        new Vector2d(new Vector2(0, 1)),
        new Vector2d(new Vector2(1, -1)),
        new Vector2d(new Vector2(1, 0)),
        new Vector2d(new Vector2(1, 1))
    };

    public override void GetDesiredSteering(SteeringResult rst)
    {
        _neighbors.Clear();
        LogicCore.SP.SceneManager.CurrentScene.FixedQuadTree.Query(Self, FixedMath.One * 2, _neighbors);
        if(_neighbors.Count==0)return;
        var seperationVector = Vector3d.zero;
        for (var i = 0; i < _neighbors.Count; i++)
        {
            var combinedRadius = Self.Radius + _neighbors[i].Radius;

            if (combinedRadius.Mul(combinedRadius) > Vector3d.SqrDistance(Self.Position, _neighbors[i].Position))
            {
                var avoidDir = Self.Position - _neighbors[i].Position;
                if (avoidDir.sqrMagnitude < FixedMath.One / 1000)
                {
                    seperationVector += new Vector3d(FixedMath.One/ UnityEngine.Random.Range(1, 3), 0, FixedMath.One/ UnityEngine.Random.Range(1, 3)) * combinedRadius;
                }
                else
                {
                    var mag = avoidDir.magnitude;
                    seperationVector += avoidDir.Normalize() * Math.Max(FixedMath.Half , combinedRadius - mag);
                }
            }
        }
        if (seperationVector != Vector3d.zero)
        {
            rst.DesiredSteering = seperationVector / _neighbors.Count;
        }
    }
}