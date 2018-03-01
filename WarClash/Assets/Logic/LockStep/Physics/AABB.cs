using Lockstep;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        UpdateCenterAndExtents();
    }
    public void UpdateCenterAndExtents()
    {
        Center = (LowerBound + UpperBound) / 2;
        Extents = (UpperBound - LowerBound) / 2;
    }
    public long Width {
        get {
            return UpperBound.x - LowerBound.x;
        }
    } 

    public float Height
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
}