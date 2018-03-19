using Logic.LogicObject;

public interface IU3DScene
{
    void Init(IScene scene);
    void Update(float deltaTime);
    void Destroy();

}
