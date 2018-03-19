using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CharacterWindowEditor : EditorWindow
{

    [MenuItem("Window/角色编辑")]
    static void Init()
    {
        CharacterWindowEditor window = (CharacterWindowEditor)EditorWindow.GetWindow(typeof(CharacterWindowEditor));
        window.Show();
    }
    void OnGUI()
    {

    }
}
