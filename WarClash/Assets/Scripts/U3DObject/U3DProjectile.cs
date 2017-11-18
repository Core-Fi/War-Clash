using Logic.LogicObject;
using System.Collections;
using System.Collections.Generic;
using Lockstep;
using UnityEngine;
using System;

public class U3DProjectile : U3DSceneObject
{
    private Projectile _projectile;
    private float _curHeight;
    public override void OnInit()
    {
        base.OnInit();
        _projectile = So as Projectile;
        AssetResources.LoadAsset("arrow.prefab", OnLoadedRes);
        _curHeight = _projectile.InitHeight.ToFloat();
    }
    public override void OnUpdate()
    {
        base.OnUpdate();
        if (Go != null)
        {
            _curHeight -= Time.deltaTime/2;
            _curHeight = Math.Max(_curHeight, 0);
            var logicPosi = So.Position.ToVector3();
            float distance = Vector3.Distance(logicPosi, Transform.position);
            var posi = Vector3.Lerp(Transform.position, logicPosi, Time.deltaTime * 6);
            posi.y = _curHeight;
            Transform.position = posi;
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
