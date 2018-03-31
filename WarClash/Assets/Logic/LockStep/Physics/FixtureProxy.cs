public class FixtureProxy
{
    public AABB AABB;
    public int ChildIndex;
    public Transform2d Fixture;
    public int ProxyId;
    public BodyType BodyType;
}
public enum BodyType
{
    Wall, Npc, Player
}