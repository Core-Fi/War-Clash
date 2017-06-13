using System;
using System.Collections.Generic;
using System.Text;

namespace Logic
{
    public class EventGroup
    {
        private static Dictionary<Type, Queue<EventMsg>> dic = new Dictionary<Type, Queue<EventMsg>>();

        public static T NewArg<T, K>(K param) where T : EventSingleArgs<K>
        {
            T t = null;
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
        public static T NewArg<T, K, V>(K param1, V param2) where T : EventTwoArgs<K,V>
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
        public void AddEvent(int e_id, EventMsgHandler action)
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
}
