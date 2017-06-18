using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logic.LogicObject;

public class LogicObject : MonoBehaviour {

    public int ID;
    public SceneObject so{
        get{
            if(_so == null)
            {
                _so = Logic.LogicCore.SP.sceneManager.currentScene.GetObject(ID);
            }
            return _so;
        }
    }
    private SceneObject _so;

}
