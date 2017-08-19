using UnityEngine;
using System.Collections; using FastCollections;

namespace Lockstep
{
    //THIS IS HAPPENING!!!!
    [System.Serializable]
    public struct Vector3d : Lockstep.ICommandData
    {
        [FixedNumber]
		/// <summary>
		/// Horizontal
		/// </summary>
        public long x;
        [FixedNumber]
		/// <summary>
		/// Forward/Backward
		/// </summary>
        public long y;
        [FixedNumber]
		/// <summary>
		/// Up/Down
		/// </summary>
        public long z; //Height
        public Vector3d (Vector3 vec3) {
            this.x = FixedMath.Create(vec3.x);
            this.y = FixedMath.Create(vec3.y);
            this.z = FixedMath.Create(vec3.z);
        }
        public Vector3d (long X, long Y, long Z) {
            x = X;
            y = Y;
            z = Z;
        }
        public static Vector3d GetForward()
        {
            return new Vector3d(0, 0, FixedMath.One);
        }
        
        public Vector3d Normalize () {
            long magnitude = FixedMath.Sqrt(x.Mul(x) + y.Mul(y) + z.Mul(z));
            x = x.Div(magnitude);
            y = y.Div(magnitude);
            z = z.Div(magnitude);
            return this;
        }
        public Vector2d ToVector2d () {
            return new Vector2d(x,y);
        }
        public Vector3 ToVector3 () {
            return new Vector3(x.ToPreciseFloat(),y.ToPreciseFloat(),z.ToPreciseFloat());
        }
        public void SetVector2d(Vector2d vec2d) {
            x = vec2d.x;
            y = vec2d.y;
        }
        public void Add (ref Vector2d other) {
            x += other.x;
            y += other.y;
        }
        public void Add (ref Vector3d other) {
            x += other.x;
            y += other.y;
            z += other.z;
        }
        public static Vector3d operator +(Vector3d a, Vector3d b)
        {
            Vector3d v = a;
            v.Add(b);
            return v;
        }
        public Vector3d Add (Vector3d other) {
            x += other.x;
            y += other.y;
            z += other.z;
            return this;
        }
        public Vector3d Mul (long f1) {
            x *= f1;
            x >>= FixedMath.SHIFT_AMOUNT;
            y *= f1;
            y >>= FixedMath.SHIFT_AMOUNT;
            z *= f1;
            z >>= FixedMath.SHIFT_AMOUNT;
            return this;
        }
        public Vector3d Mul (int i) {
            x *= i;
            y *= i;
            z *= i;
            return this;
        }
        public static Vector3d operator *(Vector3d a, long b)
        {
            Vector3d v = a;
            v.Mul(b);
            return v;
        }
        public static Vector3d operator *(Vector3d a, int b)
        {
            Vector3d v = a;
            v.Mul(b);
            return v;
        }

        public static Vector3d operator -(Vector3d a, Vector3d b)
        {
            Vector3d v = a;
            int nagtive = -1;
            v.Add(b*nagtive);
            return v;
        }
        public static bool operator ==(Vector3d a, Vector3d b)
        {
            return a.x == b.x && a.y == b.y && a.z == b.z;
        }
        public static bool operator !=(Vector3d a, Vector3d b)
        {
            return a.x != b.x || a.y != b.y || a.z != b.z;
        }
        public static long Distance(Vector3d a, Vector3d b)
        {
            long tX = b.x - a.x;
            tX *= tX;
            tX >>= FixedMath.SHIFT_AMOUNT;
            long tY = b.y - a.y;
            tY *= tY;
            tY >>= FixedMath.SHIFT_AMOUNT;
            long tZ = b.z - a.z;
            tZ *= tZ;
            tZ >>= FixedMath.SHIFT_AMOUNT;
            return FixedMath.Sqrt(tX + tY + tZ);

        }
        public long Distance (Vector3d other) {
            long tX = other.x - x;
            tX *= tX;
            tX >>= FixedMath.SHIFT_AMOUNT;
            long tY = other.y - y;
            tY *= tY;
            tY >>= FixedMath.SHIFT_AMOUNT;
            long tZ = other.z - z;
            tZ *= tZ;
            tZ >>= FixedMath.SHIFT_AMOUNT;
            return FixedMath.Sqrt(tX + tY + tZ);
        }
        public long LongStateHash {get {return (x * 31 + y * 7 + z * 11);}}
        public int StateHash {get {return (int)(LongStateHash % int.MaxValue);}}

        public override string ToString()
        {
            return string.Format("({0}, {1}, {2})", x.ToFormattedDouble(),y.ToFormattedDouble(),z.ToFloat());
        }

        public void Write (Writer writer) {
            writer.Write(x);
            writer.Write(y);
            writer.Write(z);
        }

        public void Read (Reader reader) {
            x = reader.ReadLong();
            y = reader.ReadLong();
            z = reader.ReadLong();
        }
    }
}