using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class EventGroup
{
    private static Dictionary<Type, Queue<EventMsg>> dic = new Dictionary<Type, Queue<EventMsg>>();

    public static T NewArg<T>() where T : EventMsg
    {
        T t = null;
        if (!dic.ContainsKey(typeof(T)))
        {
            dic.Add(typeof(T), new Queue<EventMsg>());
        }
        if (dic[typeof(T)].Count == 0)
        {
            t = Activator.CreateInstance<T>();
        }
        else
        {
            t = dic[typeof(T)].Dequeue() as T;
        }
        return t;
    }
    public static T NewArg<T, TK>(TK param) where T : EventSingleArgs<TK>
    {
        T t = null;
        Type type = typeof (T);
        if (!dic.ContainsKey(typeof(T)))
        {
            dic.Add(typeof(T), new Queue<EventMsg>());
        }
        if (dic[typeof(T)].Count==0)
        {
            t = Activator.CreateInstance(typeof(T), param) as T;
        }
        else
        {
            t = dic[typeof(T)].Dequeue() as T;
            t.value = param;
        }
        return t;
    }
    public static T NewArg<T, T1, T2>(T1 param1, T2 param2) where T : EventTwoArgs<T1, T2>
    {
        T t = null;
        if (!dic.ContainsKey(typeof(T)))
        {
            dic.Add(typeof(T), new Queue<EventMsg>());
        }
        if (dic[typeof(T)].Count == 0)
        {
            t = Activator.CreateInstance(typeof(T), param1, param2) as T;
        }
        else
        {
            t = dic[typeof(T)].Dequeue() as T;
            t.value1 = param1;
            t.value2 = param2;
        }
        return t;
    }
    public static T NewArg<T, T1, T2, T3>(T1 param1, T2 param2, T3 param3) where T : EventThreeArgs<T1, T2, T3>
    {
        T t = null;
        if (!dic.ContainsKey(typeof(T)))
        {
            dic.Add(typeof(T), new Queue<EventMsg>());
        }
        if (dic[typeof(T)].Count == 0)
        {
            t = Activator.CreateInstance(typeof(T), param1, param2, param3) as T;
        }
        else
        {
            t = dic[typeof(T)].Dequeue() as T;
            t.value1 = param1;
            t.value2 = param2;
            t.value3 = param3;
        }
        return t;
    }

    public static void Return(EventMsg e)
    {
        Type t = e.GetType();
        if (!dic.ContainsKey(t))
        {
            dic.Add(t, new Queue<EventMsg>());
        }
        e.Clear();
        dic[t].Enqueue(e);
    }
    private Dictionary<int, EventMsgHandler> events = new Dictionary<int, EventMsgHandler>();
    public void ListenEvent(int e_id, EventMsgHandler action)
    {
        if (events.ContainsKey(e_id))
        {
            events[e_id] += action;
        }
        else
        {
            events[e_id] = action;      
        }
    }
    public void DelEvent(int e_id, EventMsgHandler action)
    {
        if(events.ContainsKey(e_id) && events[e_id]!=null)
            events[e_id] -= action;
    }

    public void FireEvent(int e_id, object sender, EventMsg e)
    {
        if (events.ContainsKey(e_id) && events[e_id] != null)
        {
            events[e_id].Invoke(sender, e);
        }
        if (e != null)
            Return(e);
    }
}
