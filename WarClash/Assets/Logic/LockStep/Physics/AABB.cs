using Lockstep;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public struct AABB
{
    /// <summary>
    /// The lower vertex
    /// </summary>
    public Vector2d LowerBound;
    /// <summary>
    /// The upper vertex
    /// </summary>
    public Vector2d UpperBound;

    public AABB(Vector2d min, Vector2d max)
        : this(ref min, ref max) { }

    public AABB(Vector2d center, long width, long height)
        : this(center - new Vector2d(width / 2, height / 2), center + new Vector2d(width / 2, height / 2))
    {
    }

    public AABB(ref Vector2d min, ref Vector2d max)
    {
        Center = Extents = Vector2d.zero;
        LowerBound = new Vector2d(Math.Min(min.x, max.x), Math.Min(min.y, max.y));
        UpperBound = new Vector2d(Math.Max(min.x, max.x), Math.Max(min.y, max.y));
        verts = null;
        UpdateCenterAndExtents();
    }
    public void UpdateCenterAndExtents()
    {
        Center = (LowerBound + UpperBound) / 2;
        Extents = (UpperBound - LowerBound) / 2;
    }
    private Vector2d[] verts;
    public Vector2d[] Local2World(long angle)
    {
        if (verts == null)
            verts = new Vector2d[4];
        verts[0] = new Vector2d(-this.Extents.x, -this.Extents.y);
        verts[1] = new Vector2d(this.Extents.x , -this.Extents.y );
        verts[2] = new Vector2d(this.Extents.x , this.Extents.y );
        verts[3] = new Vector2d(-this.Extents.x , this.Extents.y );
        for (var i = 0; i < verts.Length; ++i)
        {
            var v = new Vector2d(verts[i].x, verts[i].y);
            verts[i].x = v.x.Mul( FixedMath.Trig.CosDegree(angle)) + v.y.Mul(FixedMath.Trig.SinDegree(angle)) + this.Center.x;
            verts[i].y = -v.x.Mul(FixedMath.Trig.SinDegree(angle)) + v.y.Mul(FixedMath.Trig.CosDegree(angle)) + this.Center.y;
        }
        return verts;
    }

    public long Width {
        get {
            return UpperBound.x - LowerBound.x;
        }
    } 

    public long Height
    {
        get
        {
            return UpperBound.y - LowerBound.y;
        }
    }

    /// <summary>
    /// Get the center of the AABB.
    /// </summary>
    public Vector2d Center { get; set; }
    /// <summary>
    /// Get the extents of the AABB (half-widths).
    /// </summary>
    public Vector2d Extents { get; set; }
    /// <summary>
    /// Get the perimeter length
    /// </summary>
    public long Perimeter
    {
        get
        {
            long wx = UpperBound.x - LowerBound.x;
            long wy = UpperBound.y - LowerBound.y;
            return 2 * (wx + wy);
        }
    }

    ///// <summary>
    ///// Gets the vertices of the AABB.
    ///// </summary>
    ///// <value>The corners of the AABB</value>
    //public Vertices Vertices
    //{
    //    get
    //    {
    //        Vertices vertices = new Vertices(4);
    //        vertices.Add(UpperBound);
    //        vertices.Add(new Vector2(UpperBound.X, LowerBound.Y));
    //        vertices.Add(LowerBound);
    //        vertices.Add(new Vector2(LowerBound.X, UpperBound.Y));
    //        return vertices;
    //    }
    //}

    ///// <summary>
    ///// First quadrant
    ///// </summary>
    //public AABB Q1 => new AABB(Center, UpperBound);

    ///// <summary>
    ///// Second quadrant
    ///// </summary>
    //public AABB Q2 => new AABB(new Vector2(LowerBound.X, Center.Y), new Vector2(Center.X, UpperBound.Y));

    ///// <summary>
    ///// Third quadrant
    ///// </summary>
    //public AABB Q3 => new AABB(LowerBound, Center);

    ///// <summary>
    ///// Forth quadrant
    ///// </summary>
    //public AABB Q4 => new AABB(new Vector2(Center.X, LowerBound.Y), new Vector2(UpperBound.X, Center.Y));

    /// <summary>
    /// Verify that the bounds are sorted. And the bounds are valid numbers (not NaN).
    /// </summary>
    /// <returns>
    /// <c>true</c> if this instance is valid; otherwise, <c>false</c>.
    /// </returns>
    public bool IsValid()
    {
        Vector2d d = UpperBound - LowerBound;
        bool valid = d.x >= 0 && d.y >= 0;
        return valid;
    }

    /// <summary>
    /// Combine an AABB into this one.
    /// </summary>
    /// <param name="aabb">The AABB.</param>
    public void Combine(ref AABB aabb)
    {
        LowerBound = Vector2d.Min(LowerBound, aabb.LowerBound);
        UpperBound = Vector2d.Max(UpperBound, aabb.UpperBound);
        UpdateCenterAndExtents();
    }

    /// <summary>
    /// Combine two AABBs into this one.
    /// </summary>
    /// <param name="aabb1">The aabb1.</param>
    /// <param name="aabb2">The aabb2.</param>
    public void Combine(ref AABB aabb1, ref AABB aabb2)
    {
        LowerBound = Vector2d.Min(aabb1.LowerBound, aabb2.LowerBound);
        UpperBound = Vector2d.Max(aabb1.UpperBound, aabb2.UpperBound);
        UpdateCenterAndExtents();
    }

    /// <summary>
    /// Does this AABB contain the provided AABB.
    /// </summary>
    /// <param name="aabb">The AABB.</param>
    /// <returns>
    /// <c>true</c> if it contains the specified AABB; otherwise, <c>false</c>.
    /// </returns>
    public bool Contains(ref AABB aabb)
    {
        bool result = LowerBound.x <= aabb.LowerBound.x;
        result = result && LowerBound.y <= aabb.LowerBound.y;
        result = result && aabb.UpperBound.x <= UpperBound.x;
        result = result && aabb.UpperBound.y <= UpperBound.y;
        return result;
    }

    /// <summary>
    /// Determines whether the AABB contains the specified point.
    /// </summary>
    /// <param name="point">The point.</param>
    /// <returns>
    /// <c>true</c> if it contains the specified point; otherwise, <c>false</c>.
    /// </returns>
    public bool Contains(ref Vector2d point)
    {
        //using epsilon to try and guard against float rounding errors.
        return (point.x > (LowerBound.x + 1) && point.x < (UpperBound.x - 1) &&
                (point.y > (LowerBound.y + 1) && point.y < (UpperBound.y - 1)));
    }

    /// <summary>
    /// Test if the two AABBs overlap.
    /// </summary>
    /// <param name="a">The first AABB.</param>
    /// <param name="b">The second AABB.</param>
    /// <returns>True if they are overlapping.</returns>
    public static bool TestOverlap(ref AABB a, ref AABB b)
    {
        Vector2d d1 = b.LowerBound - a.UpperBound;
        Vector2d d2 = a.LowerBound - b.UpperBound;

        return (d1.x <= 0) && (d1.y <= 0) && (d2.x <= 0) && (d2.y <= 0);
    }

    /// <summary>
    /// Raycast against this AABB using the specified points and maxfraction (found in input)
    /// </summary>
    /// <param name="output">The results of the raycast.</param>
    /// <param name="input">The parameters for the raycast.</param>
    /// <returns>True if the ray intersects the AABB</returns>
    public bool RayCast(out RayCastOutput output, ref RayCastInput input, bool doInteriorCheck = true)
    {
        // From Real-time Collision Detection, p179.

        output = new RayCastOutput();

        var tmin = -long.MaxValue;
        var tmax = long.MaxValue;

        var p = input.Point1;
        var d = input.Point2 - input.Point1;
        var absD = Vector2d.Abs(d);

        var normal = new Vector2d(0,0);

        for (int i = 0; i < 2; ++i)
        {
            long absD_i = i == 0 ? absD.x : absD.y;
            long lowerBound_i = i == 0 ? LowerBound.x : LowerBound.y;
            long upperBound_i = i == 0 ? UpperBound.x : UpperBound.y;
            long p_i = i == 0 ? p.x : p.y;

            if (absD_i < 1)
            {
                // Parallel.
                if (p_i < lowerBound_i || upperBound_i < p_i)
                {
                    return false;
                }
            }
            else
            {
                long d_i = i == 0 ? d.x : d.y;

                long inv_d = FixedMath.One.Div(d_i);
                long t1 = (lowerBound_i - p_i).Mul(inv_d);
                long t2 = (upperBound_i - p_i).Mul(inv_d);

                // Sign of the normal vector.
                long s = -FixedMath.One;

                if (t1 > t2)
                {
                    Utility.Swap(ref t1, ref t2);
                    s = FixedMath.One;
                }

                // Push the min up
                if (t1 > tmin)
                {
                    if (i == 0)
                    {
                        normal.x = s;
                    }
                    else
                    {
                        normal.y = s;
                    }

                    tmin = t1;
                }

                // Pull the max down
                tmax = Math.Min(tmax, t2);

                if (tmin > tmax)
                {
                    return false;
                }
            }
        }

        // Does the ray start inside the box?
        // Does the ray intersect beyond the max fraction?
        if (doInteriorCheck && (tmin < 0 || input.MaxFraction < tmin))
        {
            return false;
        }

        // Intersection.
        output.Fraction = tmin;
        output.Normal = normal;
        return true;
    }
    public void DrawAABB(Color color, long angle = 0, float time = 0.1f )
    {
        Vector2d[] vers = Local2World(angle);
        Debug.DrawLine(new Vector3(vers[0].x.ToFloat(), vers[0].y.ToFloat(), 0), new Vector3(vers[1].x.ToFloat(), vers[1].y.ToFloat(), 0), color, time);
        Debug.DrawLine(new Vector3(vers[1].x.ToFloat(), vers[1].y.ToFloat(), 0), new Vector3(vers[2].x.ToFloat(), vers[2].y.ToFloat(), 0), color, time);
        Debug.DrawLine(new Vector3(vers[2].x.ToFloat(), vers[2].y.ToFloat(), 0), new Vector3(vers[3].x.ToFloat(), vers[3].y.ToFloat(), 0), color, time);
        Debug.DrawLine(new Vector3(vers[3].x.ToFloat(), vers[3].y.ToFloat(), 0), new Vector3(vers[0].x.ToFloat(), vers[0].y.ToFloat(), 0), color, time);
    }
    public void DrawAABB(long angle = 0, float time = 0.1f)
    {
        Vector2d[] vers = Local2World(angle);
        Debug.DrawLine(new Vector3(vers[0].x.ToFloat(),  vers[0].y.ToFloat(), 0), new Vector3(vers[1].x.ToFloat(),  vers[1].y.ToFloat(), 0), Color.blue, time);
        Debug.DrawLine(new Vector3(vers[1].x.ToFloat(),  vers[1].y.ToFloat(), 0), new Vector3(vers[2].x.ToFloat(),  vers[2].y.ToFloat(), 0), Color.blue, time);
        Debug.DrawLine(new Vector3(vers[2].x.ToFloat(),  vers[2].y.ToFloat(), 0), new Vector3(vers[3].x.ToFloat(),  vers[3].y.ToFloat(), 0), Color.blue, time);
        Debug.DrawLine(new Vector3(vers[3].x.ToFloat(),  vers[3].y.ToFloat(), 0), new Vector3(vers[0].x.ToFloat(),  vers[0].y.ToFloat(), 0), Color.blue, time);
    }
    private static Vector2d[] axes = new Vector2d[4];

    public static bool TestObb(AABB a, AABB b, long aangle, long bangle)
    {
        if(aangle ==0 && bangle == 0)
        {
            return TestOverlap(ref a, ref b);
        }
        // find axes
        axes[0] = new Vector2d(FixedMath.Trig.CosDegree(aangle), -FixedMath.Trig.SinDegree(aangle));
        axes[1] = new Vector2d(FixedMath.Trig.SinDegree(aangle), FixedMath.Trig.CosDegree(aangle));
        axes[2] = new Vector2d(FixedMath.Trig.CosDegree(bangle), -FixedMath.Trig.SinDegree(bangle));
        axes[3] = new Vector2d(FixedMath.Trig.SinDegree(bangle), FixedMath.Trig.CosDegree(bangle));
        var verts1 = a.Local2World(aangle);
       // DrawAABB(verts1);
        var verts2 = b.Local2World(bangle);
       // DrawAABB(verts2);
        // project vertices to each axis
        for (var i = 0; i < axes.Length; ++i)
        {
            // find max and min from o1
            long min1 = long.MaxValue, max1 = long.MinValue, ret1;
            for (var j = 0; j < verts1.Length; ++j)
            {
                ret1 = verts1[j].Dot(axes[i]);
                min1 = min1 > ret1 ? ret1 : min1;
                max1 = max1 < ret1 ? ret1 : max1;
            }
            // find max and min from o2
            long min2 = long.MaxValue, max2 = long.MinValue, ret2;
            for (var j = 0; j < verts2.Length; ++j)
            {
                ret2 = verts2[j].Dot(axes[i]);
                min2 = min2 > ret2 ? ret2 : min2;
                max2 = max2 < ret2 ? ret2 : max2;
            }
            // overlap check
            var r1 = max1 - min1;
            var r2 = max2 - min2;
            var r = (max1 > max2 ? max1 : max2) - (min1 < min2 ? min1 : min2);
            if (r1 + r2 <= r)
            {
                return false;
            }
        }
        return true;
    }
    public static bool TestTriangle(Vector2d v0, Vector2d v1, Vector2d v2, AABB b, long angle)
    {
        long p0, p1, p2, r;
        // Compute box center and extents (if not already given in that format)
        Vector2d c = (b.LowerBound + b.UpperBound) /2;
        long e0 = (b.UpperBound.x - b.LowerBound.x) / 2;
        long e1 = (b.UpperBound.y - b.LowerBound.y) / 2;
        // Translate triangle as conceptually moving AABB to origin
        v0 = v0 - c;
        v1 = v1 - c;
        v2 = v2 - c;
        Vector2d f0 = v1 - v0, f1 = v2 - v1, f2 = v0 - v2;
        // Test axes a00..a22 (category 3)
        // Test axis a00
        p0 = v0.x * v1.y - v0.y * v1.x;
        p2 = v2.x * (v1.y - v0.y) - v2.x * (v1.x - v0.x);
        r = e1 .Mul(Math.Abs(f0.x)) + e0.Mul(Math.Abs(f0.y));
        if (Math.Max(-Math.Max(p0, p2), Math.Min(p0, p2)) > r) return false; // Axis is a separating axis

        if (Mathf.Max(v0.x, v1.x, v2.x) < -e0 || Mathf.Min(v0.x, v1.x, v2.x) > e0) return false;
        // ... [-e1, e1] and [min(v0.y,v1.y,v2.y), max(v0.y,v1.y,v2.y)] do not overlap
        if (Mathf.Max(v0.y, v1.y, v2.y) < -e1 || Mathf.Min(v0.y, v1.y, v2.y) > e1) return false;
        // ... [-e2, e2] and [min(v0.z,v1.z,v2.z), max(v0.z,v1.z,v2.z)] do not overlap
        // Test separating axis corresponding to triangle face normal (category 2)
        return true;
    }
}