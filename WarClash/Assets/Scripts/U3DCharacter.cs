using Logic.LogicObject;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class U3DCharacter : U3DSceneObject{

    public Character character;
    public override void OnInit()
    {
        base.OnInit();
        character = so as Character;
    }
    public override void ListenEvents()
    {
        base.ListenEvents();
    }
    public override void OnDestroy()
    {
        base.OnDestroy();
    }
}
