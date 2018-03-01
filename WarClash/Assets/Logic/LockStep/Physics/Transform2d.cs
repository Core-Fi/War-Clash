using Lockstep;

public struct Transform2d
{
    public Vector2d p;
    public long angle;

    /// <summary>
    /// Initialize using a position vector and a rotation matrix.
    /// </summary>
    /// <param name="position">The position.</param>
    /// <param name="rotation">The r.</param>
    public Transform2d(ref Vector2d position, ref long _rotation)
    {
        p = position;
        angle = _rotation;
    }

    /// <summary>
    /// Set this to the identity transform.
    /// </summary>
    public void SetIdentity()
    {
        p = Vector2d.Zero;
    }

    /// <summary>
    /// Set this based on the position and angle.
    /// </summary>
    /// <param name="position">The position.</param>
    /// <param name="angle">The angle.</param>
    public void Set(Vector2d position, long _angle)
    {
        p = position;
        angle = _angle;
    }
}