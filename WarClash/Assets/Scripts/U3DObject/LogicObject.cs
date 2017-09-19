using System.Collections;
using System.Collections.Generic;
using Lockstep;
using Logic;
using UnityEngine;
using Logic.LogicObject;

public class LogicObject : MonoBehaviour {

    public int ID;
#if UNITY_EDITOR

    public bool ShowAttrs;
    void OnGUI()
    {
        if (ShowAttrs)
        {
            GUILayout.BeginVertical();

            foreach (var att in so.AttributeManager.Attributes)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(((AttributeType)att.Key).ToString()+" :"+att.Value.FinalValue.ToInt());
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }
    }
#endif
    public SceneObject so{
        get{
            if(_so == null)
            {
                _so = Logic.LogicCore.SP.SceneManager.currentScene.GetObject(ID);
            }
            return _so;
        }
    }
    private SceneObject _so;

}
