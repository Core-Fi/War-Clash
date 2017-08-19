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
    }
    public override void OnDestroy()
    {
        base.OnDestroy();
    }


}
