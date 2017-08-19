using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapEdit))]
public class InspectorOfMapEdit : Editor {

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        MapEdit t = target as MapEdit;
        if (GUILayout.Button("Generate"))
        {
            t.Generate();
        }
        if (GUILayout.Button("Save"))
        {
            t.Save();   
        }
        if(GUILayout.Button("ShowElse"))
        {
            var list = Resources.FindObjectsOfTypeAll(typeof(GameObject));
            for (int i = 0; i < list.Length; i++)
            {
                if(list[i] != t.gameObject)
                {
                    list[i].hideFlags = HideFlags.None;
                }
            }
        }
    }
    bool edit = false;
    public void OnSceneGUI()
    {
        MapEdit t = target as MapEdit;
        if (Event.current.type == EventType.mouseDown)
        {
            edit = !edit;
        }
        if (edit && Event.current.type == EventType.MouseMove)
        {
            t.Raycast(HandleUtility.GUIPointToWorldRay(Event.current.mousePosition));
        }
    }
}
