using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Logic
{
    public delegate void EventMsgHandler(object sender, EventMsg e);
    public class EventMsg
    {
        public virtual void Clear()
        {

        }
    }
    public class EventSingleArgs<T> : EventMsg
    {
        public T value { get; internal set; }
        public EventSingleArgs() { } 
        public EventSingleArgs(T _t)
        {
            value = _t;
        }

        public override void Clear()
        {
            value = default(T);
            base.Clear();
        }
    }
    public class EventTwoArgs<K, V> : EventMsg
    {
        public K value1 { get; internal set; }
        public V value2 { get; internal set; }
        public EventTwoArgs() { }
        public EventTwoArgs(K k, V v)
        {
            value1 = k;
            value2 = v;
        }
        public override void Clear()
        {
            value1 = default(K);
            value2 = default(V);
            base.Clear();
        }
    }
}
