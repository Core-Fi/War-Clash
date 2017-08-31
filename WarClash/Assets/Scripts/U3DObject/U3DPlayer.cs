using UnityEngine;
using Object = UnityEngine.Object;

class U3DPlayer : U3DCharacter
{
    private GameObject logicGo;
    public override void OnInit()
    {
        base.OnInit();
        Resource.LoadAsset("Footman_prefab.prefab", OnLoadedRes);
        logicGo = GameObject.CreatePrimitive(PrimitiveType.Cube);
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
            logicGo.transform.position = logicPosi;
            var additive = (logicPosi - Go.transform.position) * Time.deltaTime * 6;;
            if (additive != Vector3.zero)
            {
                if (additive.sqrMagnitude < 0.1f)
                {
                    Transform.position = logicPosi;
                }
                else
                {
                    Transform.position += additive;
                }
            }
            if (Vector3.Distance(logicPosi, Go.transform.position) < 0.1f)
            {
                var tempForward = Character.Forward.ToVector3();
                if (Transform.forward != tempForward)
                {
                    Transform.forward = tempForward;

                }
            }
            else
            {
                var tempForward = (logicPosi - Go.transform.position).normalized;
                if (Transform.forward != tempForward)
                {
                    Transform.forward = tempForward;
                }
            }
          
        }
    }
}
