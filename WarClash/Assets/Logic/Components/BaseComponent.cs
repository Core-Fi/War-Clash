using Logic.LogicObject;

namespace Logic.Components
{
    public abstract class BaseComponent : IEventDispatcher
    {
        public SceneObject SceneObject;
        public EventGroup EventGroup { get; set; }
        public virtual void OnAdd() { }
        public virtual void OnRemove() { }
        public virtual void OnUpdate() { }
        public virtual void OnFixedUpdate() { }
        public void ListenEvent(int id, EventMsgHandler e)
        {
            EventGroup.ListenEvent(id, e);
        }

        public  void DelEvent(int id, EventMsgHandler e)
        {
            EventGroup.DelEvent(id, e);
        }

        public  void FireEvent(int id, object sender, EventMsg m)
        {
            EventGroup.FireEvent(id, sender, m);
        }
    }
}
