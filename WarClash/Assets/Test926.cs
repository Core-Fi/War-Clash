using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Lockstep;
using Logic.LogicObject;
using UnityEngine;

public class Test926 : MonoBehaviour {

    //cosx -sinx
    //sinx cosx
	// Use this for initialization
    public float Angle = 60;

    public int relativex;
    public int relativey;
    public Transform target;
	void Start ()
	{

	    for (int i = 0; i < 1000; i++)
	    {
	        var ra = UnityEngine.Random.Range(0, 1000f);
	        var v1 = Mathf.Sin(ra * Mathf.Deg2Rad);
	        var v2 = FixedMath.Trig.SinDegree(FixedMath.Create(ra));
           // Debug.LogError(ra+" "+v1+" "+v2.ToFloat());
	    }
        var f = transform.forward;
        Vector3 nf = Vector3.zero;
        nf.x = f.x * Mathf.Cos(Angle * Mathf.Deg2Rad) + f.z * Mathf.Sin(Angle * Mathf.Deg2Rad);
        nf.z = f.z * Mathf.Cos(Angle * Mathf.Deg2Rad) - f.x * Mathf.Sin(Angle * Mathf.Deg2Rad);
        transform.forward = nf;
        //   StringBuilder sb = new StringBuilder(1024);
        //for (int i = 0; i <= 360; i++)
        //{
        //    var cos = Mathf.Cos(i * Mathf.Deg2Rad);
        //    var fcos = FixedMath.Create(cos);
        //    sb.Append("{"+FixedMath.Create(i)+","+fcos+ "},");
        //}
        //   File.WriteAllText("D://cos.txt", sb.ToString());
    }
     double rand0_1(double r)
        {
            double _base=256.0;
             double a = 17.0;
             double b = 139.0;
             double temp1 = a * (r) + b;
             //printf("%lf",temp1);
             double temp2 = (int)(temp1 / _base); //得到余数
             double temp3 = temp1 - temp2 * _base;
             //printf("%lf\n",temp2);
            //printf("%lf\n",temp3);
             r=temp3;
             double p = r / _base;
             return p;
    }

    float rand(ref int r)
    {
        int _base = 256;
        int a = 17;
        int b = 139;
        long temp1 = (a * (r) + b).ToLong();
        long temp2 = (temp1.Div( _base)).ToInt().ToLong(); //得到余数
        long temp3 = temp1 - temp2 * _base;
        r = temp3.ToInt();
        long p = temp3.Div(_base);
        return p.ToFloat();
    }
    public int RandomRange(int s, int e)
    {
        var value = Utility.Random(ref seed);
        value *= (e - s);
        value += s.ToLong();
        return value.ToInt();
    }
    private int seed = 10;
    void Update()
    {
        Debug.LogError(RandomRange(-10,10));
        var f = transform.forward;
        Vector3 newf = Vector3.zero;
        newf.x = relativex * f.z + relativey * f.x;
        newf.z = relativey * f.z - relativex * f.x;
      //  target.position = newf;
    }
	// Update is called once per frame


    void OnEnable()
    {
        //Relocate();
        
    }
}

class SteeringTest : ISteering
{
    public IFixedAgent Next { get; set; }
    public Vector3d Position { get; set; }
    public long Radius { get;  set; }
    public Vector3d Acceleration { get;  set; }
    public Vector3d Velocity { get;  set; }
    public long Speed { get;  set; }
    public long MaxAcceleration { get;  set; }
    public long MaxDeceleration { get;  set; }
}
class FixedAgentTest : SceneObject, IFixedAgent
{
    public IList<IFixedAgent> AgentNeighbors { get; set; }
    public IList<long> AgentNeighborSqrDists { get; set; }
    public IFixedAgent Next { get; set; }
    public Vector3d Position { get; set; }
    public long Radius { get; set; }

    public FixedAgentTest()
    {
        AgentNeighbors = new List<IFixedAgent>();
        AgentNeighborSqrDists = new List<long>();
    }
    public long InsertAgentNeighbour(IFixedAgent fixedAgent, long rangeSq)
    {
        if (this == fixedAgent) return rangeSq;
        var dist = (fixedAgent.Position.x - Position.x).Mul(fixedAgent.Position.x - Position.x)
                   + (fixedAgent.Position.z - Position.z).Mul(fixedAgent.Position.z - Position.z);
        if (dist < rangeSq)
        {
            if (AgentNeighbors.Count < 10)
            {
                AgentNeighbors.Add(fixedAgent);
                AgentNeighborSqrDists.Add(dist);
            }
            var i = AgentNeighbors.Count - 1;
            if (dist < AgentNeighborSqrDists[i])
            {
                while (i != 0 && dist < AgentNeighborSqrDists[i - 1])
                {
                    AgentNeighbors[i] = AgentNeighbors[i - 1];
                    AgentNeighborSqrDists[i] = AgentNeighborSqrDists[i - 1];
                    i--;
                }
                AgentNeighbors[i] = fixedAgent;
                AgentNeighborSqrDists[i] = dist;
            }

            if (AgentNeighbors.Count == 10)
                rangeSq = AgentNeighborSqrDists[AgentNeighbors.Count - 1];
        }
        return rangeSq;
    }

}