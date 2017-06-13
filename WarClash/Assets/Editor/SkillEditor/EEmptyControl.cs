using System;
using UnityEngine;
using System.Collections;
using Logic.Skill.Actions;
using UnityEditor;

public class EEmptyControl : EControl
{
  
    public override void Draw()
    {
        GUILayout.Space(width);
        base.Draw();
    }

}
