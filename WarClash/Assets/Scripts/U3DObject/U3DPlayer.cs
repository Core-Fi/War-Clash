using Lockstep;
using Logic;
using UnityEngine;
using Object = UnityEngine.Object;

class U3DPlayer : U3DCharacter
{
    private GameObject logicGo;
    public override void OnInit()
    {
        base.OnInit();
        Resource.LoadAsset("Footman_prefab.prefab", OnLoadedRes);
       // logicGo = GameObject.CreatePrimitive(PrimitiveType.Cube);
    }

    public override void OnLoadedRes(string name, Object obj)
    {
        base.OnLoadedRes(name, obj);
    }
    public override void OnUpdate()
    {
        base.OnUpdate();
        if (Go != null)
        {
            var logicPosi = Character.Position.ToVector3();
           // logicGo.transform.position = logicPosi;
            float speed = this.Character.GetAttributeValue(AttributeType.MaxSpeed).ToFloat();
            float distance = Vector3.Distance(logicPosi, Transform.position);
            Transform.position = Vector3.Lerp(Transform.position, logicPosi, Time.deltaTime*6);

            //if (distance < 0.1f)
            //{
            //   // Transform.position = logicPosi;
            //}
            //else
            //{
            //    var additive = (logicPosi - Transform.position).normalized*Time.deltaTime*speed;

            //    if (additive.magnitude > distance)
            //    {
            //        Transform.position = logicPosi;
            //        Debug.LogError("on dest point");
            //    }
            //    else
            //    {
            //        Transform.position += additive;
            //    }
            //}
            if (Vector3.Distance(logicPosi, Transform.position) > 0.1f)
            {
                var tempForward = (logicPosi - Transform.position).normalized;
                if (Transform.forward != tempForward)
                {
                       Transform.forward = tempForward;
                }
            }
            
          
        }
    }
}
