using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        public EventSingleArgs(T t)
        {
            value = t;
        }

        public override void Clear()
        {
            value = default(T);
            base.Clear();
        }
    }
    public class EventTwoArgs<TK, TV> : EventMsg
    {
        public TK value1 { get; internal set; }
        public TV value2 { get; internal set; }
        public EventTwoArgs() { }
        public EventTwoArgs(TK k, TV v)
        {
            value1 = k;
            value2 = v;
        }
        public override void Clear()
        {
            value1 = default(TK);
            value2 = default(TV);
            base.Clear();
        }
    }
public class EventThreeArgs<TK, TV, T> : EventMsg
{
    public TK value1 { get; internal set; }
    public TV value2 { get; internal set; }
    public T value3 { get; internal set; }
    public EventThreeArgs() { }
    public EventThreeArgs(TK k, TV v, T t)
    {
        value1 = k;
        value2 = v;
        value3 = t;
    }
    public override void Clear()
    {
        value1 = default(TK);
        value2 = default(TV);
        value3 = default(T);
        base.Clear();
    }
}
