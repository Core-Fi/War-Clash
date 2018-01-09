using Logic.LogicObject;

public interface IU3DScene
{
    void Init(IScene scene);
    void OnUpdate(float deltaTime);
    void Destroy();

}
