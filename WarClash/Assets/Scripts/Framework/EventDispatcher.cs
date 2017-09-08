using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
class EventDispatcher
{
    private static readonly EventGroup EventGroup = new EventGroup();
    public static void ListenEvent(int id, EventMsgHandler e)
    {
        EventGroup.ListenEvent(id, e);
    }

    public static void DelEvent(int id, EventMsgHandler e)
    {
        EventGroup.DelEvent(id, e);
    }

    public static void FireEvent(int id, object sender, EventMsg m)
    {
        EventGroup.FireEvent(id, sender, m);
    }
}

public interface IEventDispatcher
{
    void ListenEvent(int id, EventMsgHandler e);
    void DelEvent(int id, EventMsgHandler e);
    void FireEvent(int id, object sender, EventMsg m);
}
