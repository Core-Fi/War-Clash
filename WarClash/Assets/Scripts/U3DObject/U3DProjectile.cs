using Logic.LogicObject;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class U3DProjectile : U3DSceneObject
{

    private Projectile projectile;
    public override void OnInit()
    {
        base.OnInit();
        projectile = so as Projectile;
    }
    public override void OnDestroy()
    {
        base.OnDestroy();

    }

}
