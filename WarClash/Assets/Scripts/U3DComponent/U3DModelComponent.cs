using Logic.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class U3DModelComponent : U3DBaseComponent
{
    private ModelComponent modelComponent;
    private U3DTransformComponent u3dTransComp;
    public GameObject Go;
    public Transform Transform;
    protected LogicObject LogicObject;
    public override void OnAdd(SceneObjectBaseComponent c)
    {
        base.OnAdd(c);
        modelComponent = (ModelComponent)c;
        u3dTransComp = U3DSceneObject.GetComponent<U3DTransformComponent>();
        AssetResources.LoadAsset(modelComponent.RePath, OnLoadedRes);
    }

    public void OnLoadedRes(string name, UnityEngine.Object obj)
    {
        Go = UnityEngine.Object.Instantiate(obj) as GameObject;
        Go.name = name;
        Go.transform.position = U3DSceneObject.SceneObject.Position.ToVector3();
        Transform = Go.transform;
        Transform.SetParent(u3dTransComp.OuterTransform);
        Transform.localPosition = Vector3.zero;
        Transform.localRotation = Quaternion.identity;
        Transform.localScale = Vector3.one;
    }
    public override void OnUpdate()
    {
        base.OnUpdate();
      
    }
}
