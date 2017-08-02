using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
class EventDispatcher
{
    private static EventGroup eventGroup = new EventGroup();
    public static void ListenEvent(int id, EventMsgHandler e)
    {
        eventGroup.ListenEvent(id, e);
    }

    public static void DelEvent(int id, EventMsgHandler e)
    {
        eventGroup.DelEvent(id, e);
    }

    public static void FireEvent(int id, object sender, EventMsg m)
    {
        eventGroup.FireEvent(id, sender, m);
    }
}

public interface IEventDispatcher
{
    void ListenEvent(int id, EventMsgHandler e);
    void DelEvent(int id, EventMsgHandler e);
    void FireEvent(int id, object sender, EventMsg m);
}
