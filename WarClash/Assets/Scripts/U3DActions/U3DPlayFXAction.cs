using System.Collections.Generic;
using System.Linq;
using System.Text;
using Logic.Skill.Actions;
using UnityEngine;

public class U3DPlayFXAction : U3DDisplayAction
{
    private PlayFXAction _playFxAction;
    private U3DSceneObject _sender;
    private GameObject go;
    public override void Execute(U3DCharacter sender, U3DCharacter receiver, object data)
    {
        _playFxAction = this.Action as PlayFXAction;
        _sender = sender;
        AssetResources.LoadAsset(_playFxAction.FXName, OnLoadRes);
    }

    private void OnLoadRes(string name, UnityEngine.Object obj)
    {
        go = UnityEngine.Object.Instantiate(obj) as GameObject;
        go.transform.position = _sender.So.Position.ToVector3();

    }
    public override void Stop()
    {
        if (go != null)
        {
            Object.Destroy(go);
        }
        go = null;
    }
}
