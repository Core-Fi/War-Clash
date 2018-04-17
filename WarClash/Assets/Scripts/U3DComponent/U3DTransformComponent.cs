using Logic.Components;
using UnityEngine;

public class U3DTransformComponent : U3DBaseComponent
{
    private TransformComponent transformComponent;
    public GameObject Outer;
    public Transform OuterTransform;
    protected LogicObject LogicObject;
    public override void OnAdd(SceneObjectBaseComponent c)
    {
        base.OnAdd(c);
        transformComponent = (TransformComponent)c;
        Outer = new GameObject(U3DSceneObject.SceneObject.ToString());
        OuterTransform = Outer.transform;
        LogicObject = Outer.GetComponent<LogicObject>(true);
        LogicObject.ID = U3DSceneObject.SceneObject.Id;
    }
 
    public override void OnUpdate()
    {
        base.OnUpdate();
        if (OuterTransform != null)
        {
            var logicPosi = transformComponent.Position.ToVector3();
            var logicForward = transformComponent.Forward.ToVector3();
            OuterTransform.position = logicPosi;// Vector3.Lerp(OuterTransform.position, logicPosi, Time.deltaTime * 6);
            OuterTransform.forward = logicForward;// Vector3.Lerp(OuterTransform.forward, logicForward, Time.deltaTime * 6);
        }
    }
}
