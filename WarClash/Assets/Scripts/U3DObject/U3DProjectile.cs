using Logic.LogicObject;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class U3DProjectile : U3DSceneObject
{
    private Projectile _projectile;
    public override void OnInit()
    {
        base.OnInit();
        _projectile = So as Projectile;
        AssetResources.LoadAsset("Barrel.prefab", OnLoadedRes);
    }
    public override void OnUpdate()
    {
        base.OnUpdate();
        if (Go != null)
        {
            var logicPosi = So.Position.ToVector3();
            float distance = Vector3.Distance(logicPosi, Transform.position);
            Transform.position = Vector3.Lerp(Transform.position, logicPosi, Time.deltaTime * 6);
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

    public override void OnDestroy()
    {
        base.OnDestroy();
    }


}
