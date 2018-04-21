using Lockstep;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicTest : MonoBehaviour {
    public GameObject[] points;
    DynamicTree<FixtureProxy> tree = new DynamicTree<FixtureProxy>();
    System.Func<RayCastInput, int, long> func;
    private void OnEnable()
    {
      
    }
    // Use this for initialization
    void Start ()
    {


        return;

        func = callBack;
        //int count = 0;
        //for (int i = 0; i < 100; i++)
        //{
        //    var g = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //    g.transform.position = new Vector3(Random.Range(0, 100), 0, Random.Range(0, 100));
        //}
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
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
        Vector2d[] vs = new Vector2d[4];
        for (int i = 0; i < 100; i++)
        {
            var center = new Vector2d(Random.Range(0, 100), Random.Range(0, 100));
            var aabb = new AABB(center, FixedMath.One*2, FixedMath.One*2);
            FixtureProxy fp = new FixtureProxy();
            long angle = FixedMath.One * (int)(Random.Range(0, 359));
            Vector2d a = -aabb.Extents;
            Vector2d b = a + new Vector2d(0, aabb.Height);
            Vector2d c = aabb.Extents;
            Vector2d d = a + new Vector2d(aabb.Width, 0);
            var halfw = aabb.Width / 2;
            var halfh = aabb.Height / 2;
            var radius = FixedMath.Sqrt((halfw).Mul (halfw) + halfh.Mul(halfh));
            a = RotatePosi(a, angle);
            b = RotatePosi(b, angle);
            c = RotatePosi(c, angle);
            d = RotatePosi(d, angle);
            vs[0] = a;
            vs[1] = b;
            vs[2] = c;
            vs[3] = d;
            Vector2d min = Vector2d.Min(vs) + center;
            Vector2d max = Vector2d.Max(vs) + center;
            DLog.Log(radius.ToFloat().ToString());
            var outteraabb = new AABB(center, radius*2, radius*2);
            fp.AABB = aabb;
            fp.Fixture = new Transform2d(ref center, ref angle);
            int id = tree.AddProxy(ref outteraabb, fp);
            tree.MoveProxy(id, ref outteraabb, Vector2d.zero);
            DrawFixtureProxy(fp);
            DrawAABB(outteraabb);
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
        sw.Start();
        for (int i = 0; i < 1; i++)
        {
            RayCastInput input = new RayCastInput();
            input.Point1 = new Vector2d(Random.Range(0, 50) * FixedMath.One, Random.Range(0, 50) * FixedMath.One);
            input.Point2 = new Vector2d(Random.Range(50, 100) * FixedMath.One, Random.Range(50, 100) * FixedMath.One);
            NewSphere(input.Point1.ToVector3(), "start");
            NewSphere(input.Point2.ToVector3(), "end");
            input.MaxFraction = FixedMath.One;
            DrawLine(input);
            tree.RayCast(callBack, ref input);
        }
        Debug.Log(sw.ElapsedMilliseconds);
    }
    private void NewSphere(Vector3 p, string name)
    {
        var g = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        g.transform.position = p;
        g.transform.localScale = Vector3.one / 2;
        g.name = name;
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
            //Debug.Log(o.Fraction.ToFloat());
            var hitPosi = i.Point1 + (i.Point2 - i.Point1) * o.Fraction;
            hitPosi = RotatePosi(hitPosi, proxy.Fixture.angle) + center;
            NewSphere(hitPosi.ToVector3(), "hit");
            //Debug.Log("hit something at " + hitPosi);
            return FixedMath.One;
        }
        else
        {
            return i.MaxFraction;
        }
    }
    
    // Update is called once per frame
    void Update () {
        var aabb = new AABB(new Vector2d(FixedMath.Create(points[3].transform.position.x), FixedMath.Create(points[3].transform.position.y)), FixedMath.One, FixedMath.One);
        aabb.DrawAABB(0, 0.01f);
        bool rst = AABB.TestTriangle(new Vector2d(FixedMath.Create(points[0].transform.position.x), FixedMath.Create(points[0].transform.position.y)),
            new Vector2d(FixedMath.Create(points[1].transform.position.x), FixedMath.Create(points[1].transform.position.y)),
            new Vector2d(FixedMath.Create(points[2].transform.position.x), FixedMath.Create(points[2].transform.position.y)),
            aabb,
            0
            );
        DLog.Log(rst.ToString());
    }
}
public class PhysicsOutput
{
    public HitInfo[] HitInfos = new HitInfo[10];
    public int EndIndex;
}
public struct HitInfo
{
    public BodyType BodyType;
    public FixtureProxy Proxy;
}