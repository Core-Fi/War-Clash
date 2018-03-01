using Lockstep;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicTest : MonoBehaviour {

    DynamicTree<FixtureProxy> tree = new DynamicTree<FixtureProxy>();
    System.Func<RayCastInput, int, long> func;
    // Use this for initialization
    void Start ()
    {
        func = callBack;
        //int count = 0;
        //for (int i = 0; i < 100; i++)
        //{
        //    var g = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //    g.transform.position = new Vector3(Random.Range(0, 100), 0, Random.Range(0, 100));
        //}
        //System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        //sw.Start();
        //for (int i = 0; i < 100; i++)
        //{
        //    Ray r = new Ray();
        //    r.origin = new Vector3(Random.Range(0, 50),0, Random.Range(0, 50));
        //    r.direction = new Vector3(Random.Range(0, 1f), 0, Random.Range(0, 1f));
        //    r.direction = r.direction.normalized;
        //    var hit = Physics.Raycast(r, 100, -1);
        //   // Debug.DrawLine(r.origin, r.direction*100+r.origin, Color.red, 10);
        //    if (hit) count++;
        //}
        //   Debug.Log(sw.ElapsedMilliseconds);
        //return;
        for (int i = 0; i < 100; i++)
        {
            var center = new Vector2d(Random.Range(0, 100), Random.Range(0, 100));
            var aabb = new AABB(center, FixedMath.One*2, FixedMath.One*2);
            FixtureProxy fp = new FixtureProxy();
            fp.AABB = aabb;
            long angle = FixedMath.One * (int)(Random.Range(0, 359));
            fp.Fixture = new Transform2d(ref center, ref angle);
            tree.AddProxy(ref aabb, fp);
            DrawFixtureProxy(fp);
        }

        //var bcs = GameObject.FindObjectsOfType<BoxCollider>();
        //for (int i = 0; i < bcs.Length; i++)
        //{
        //    var bc = bcs[i];
        //    var aabb = new AABB(new Vector2d(bc.transform.position), FixedMath.One, FixedMath.One);
        //    FixtureProxy fp = new FixtureProxy();
        //    fp.AABB = aabb;
        //    long angle = FixedMath.One * (int)(bc.transform.eulerAngles.y);
        //    Vector2d p = new Vector2d(bc.transform.position);
        //    fp.Fixture = new Transform2d(ref p, ref angle);
        //    tree.AddProxy(ref aabb, fp);
        //    DrawFixtureProxy(fp);
        //}
     //   sw.Reset();
     //   sw.Start();
        for (int i = 0; i < 1; i++)
        {
            RayCastInput input = new RayCastInput();
            input.Point1 = new Vector2d(Random.Range(0, 50) * FixedMath.One, Random.Range(0, 50) * FixedMath.One);
            input.Point2 = new Vector2d(Random.Range(50, 100) * FixedMath.One, Random.Range(50, 100) * FixedMath.One);
            input.MaxFraction = FixedMath.One;
            DrawLine(input);
            tree.RayCast(callBack, ref input);
        }
     //   Debug.Log(sw.ElapsedMilliseconds);
    }
    public void DrawAABB(AABB aabb)
    {
        Debug.DrawLine(new Vector3(aabb.LowerBound.x.ToFloat(), 0, aabb.LowerBound.y.ToFloat()), new Vector3(aabb.LowerBound.x.ToFloat(), 0, aabb.UpperBound.y.ToFloat()), Color.green, 10);
        Debug.DrawLine(new Vector3(aabb.LowerBound.x.ToFloat(), 0, aabb.UpperBound.y.ToFloat()), new Vector3(aabb.UpperBound.x.ToFloat(), 0, aabb.UpperBound.y.ToFloat()), Color.green, 10);
        Debug.DrawLine(new Vector3(aabb.UpperBound.x.ToFloat(), 0, aabb.UpperBound.y.ToFloat()), new Vector3(aabb.UpperBound.x.ToFloat(), 0, aabb.LowerBound.y.ToFloat()), Color.green, 10);
        Debug.DrawLine(new Vector3(aabb.UpperBound.x.ToFloat(), 0, aabb.LowerBound.y.ToFloat()), new Vector3(aabb.LowerBound.x.ToFloat(), 0, aabb.LowerBound.y.ToFloat()), Color.green, 10);
    }
    public void DrawFixtureProxy(FixtureProxy proxy)
    {
        var aabb = proxy.AABB;
        var a = new Vector2d(aabb.LowerBound.x, aabb.LowerBound.y);
        a = RotatePosi(a - aabb.Center, proxy.Fixture.angle) + aabb.Center;
        var b = new Vector2d(aabb.LowerBound.x, aabb.UpperBound.y);
        b = RotatePosi(b - aabb.Center, proxy.Fixture.angle) + aabb.Center;
        var c = new Vector2d(aabb.UpperBound.x, aabb.UpperBound.y);
        c = RotatePosi(c - aabb.Center, proxy.Fixture.angle) + aabb.Center;
        var d = new Vector2d(aabb.UpperBound.x, aabb.LowerBound.y);
        d = RotatePosi(d - aabb.Center, proxy.Fixture.angle) + aabb.Center;

        Debug.DrawLine(a.ToVector3(), b.ToVector3(), Color.green, 10);
        Debug.DrawLine(b.ToVector3(), c.ToVector3(), Color.green, 10);
        Debug.DrawLine(c.ToVector3(), d.ToVector3(), Color.green, 10);
        Debug.DrawLine(d.ToVector3(), a.ToVector3(), Color.green, 10);
    }
    private Vector2d RotatePosi(Vector2d a, long angle)
    {
        var relative = a;
        relative.Rotate(angle);
        var n = relative;
        return relative;
    }
    private Vector2d RotateReversePosi(Vector2d a, long angle)
    {
        var relative = a;
        relative.RotateInverse(angle);
        return relative;
    }
    public void DrawLine(RayCastInput i)
    {
        Debug.DrawLine(i.Point1.ToVector3(), i.Point2.ToVector3(), Color.red, 10);
    }
    private long callBack(RayCastInput i, int node)
    {
        var proxy = tree.GetUserData(node);
        var center = proxy.AABB.Center;
        var aabb = new AABB(-proxy.AABB.Extents, proxy.AABB.Extents);
        var a = i.Point1 - center;
        a = RotateReversePosi(a, proxy.Fixture.angle);
        var b = i.Point2 - center;
        b = RotateReversePosi(b, proxy.Fixture.angle);
        i.Point1 = a ;
        i.Point2 = b ;
        RayCastOutput o;
        if(aabb.RayCast(out o, ref i))
        {
            var hitPosi = i.Point1 + (i.Point2 - i.Point1) * o.Fraction;
            hitPosi = RotatePosi(hitPosi, proxy.Fixture.angle) + center;
            var g = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            g.transform.position = hitPosi.ToVector3();
            g.transform.localScale = Vector3.one / 5;
            Debug.Log("hit something at " + hitPosi);
            return 0;
        }
        else
        {
            return FixedMath.One;
        }
    }
    
    // Update is called once per frame
    void Update () {
    }
}
