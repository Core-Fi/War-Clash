using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(Test0711))]
public class Test0711Inspector : Editor {

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if(GUILayout.Button("AddNew"))
        {
            var t = target as Test0711;
            t.AddNew();
        }
    }


}
