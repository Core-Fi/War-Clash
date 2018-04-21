using Logic.LogicObject;
using System.Text;

namespace Logic.Components
{
    public abstract class BaseComponent : IEventDispatcher
    {
        public EventGroup EventGroup = new EventGroup();
        
        public void ListenEvent(int id, EventMsgHandler e)
        {
            EventGroup.ListenEvent(id, e);
        }
        public void DelEvent(int id, EventMsgHandler e)
        {
            EventGroup.DelEvent(id, e);
        }
        public void FireEvent(int id, object sender, EventMsg m)
        {
            EventGroup.FireEvent(id, sender, m);
        }
        public virtual void OnAdd() { }
        public virtual void OnRemove() { }

    }
    public abstract class SceneObjectBaseComponent : BaseComponent
    {
        public ExecuteOrder ExecuteOrder;
        public SceneObject SceneObject;
        public virtual void OnFixedUpdate() { }
 
    }
    public enum ExecuteOrder
    {
        Priority,
        Secondary,
        Late
    }
}
