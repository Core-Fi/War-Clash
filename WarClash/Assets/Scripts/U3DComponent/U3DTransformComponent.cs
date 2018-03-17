using Logic.Components;
using UnityEngine;

public class U3DTransformComponent : U3DBaseComponent
{
    private TransformComponent transformComponent;
    public GameObject Outer;
    public Transform OuterTransform;
    public GameObject Go;
    public Transform Transform;
    protected LogicObject LogicObject;
    public override void OnAdd(BaseComponent c)
    {
        base.OnAdd(c);
        transformComponent = (TransformComponent)c;
        AssetResources.LoadAsset(transformComponent.ResPath, OnLoadedRes);
    }
    public void OnLoadedRes(string name, UnityEngine.Object obj)
    {
        Outer = new GameObject(U3DSceneObject.SceneObject.ToString());
        OuterTransform = Outer.transform;
        LogicObject = Outer.GetComponent<LogicObject>(true);
        LogicObject.ID = U3DSceneObject.SceneObject.Id;
        Go = UnityEngine.Object.Instantiate(obj) as GameObject;
        Go.name = name;
        Go.transform.position = U3DSceneObject.SceneObject.Position.ToVector3();
        Transform = Go.transform;
        Transform.SetParent(OuterTransform);
    }
    public override void OnUpdate()
    {
        base.OnUpdate();
        if (Go != null)
        {
            var logicPosi = transformComponent.Position.ToVector3();
            var logicForward = transformComponent.Forward.ToVector3();
            OuterTransform.position = Vector3.Lerp(OuterTransform.position, logicPosi, Time.deltaTime * 6);
            OuterTransform.forward = Vector3.Lerp(OuterTransform.forward, logicForward, Time.deltaTime * 6);
        }
    }
}
