using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

struct Tuple<T, T1>
{
    public T t1;
    public T1 t2;
    public Tuple(T t1, T1 t2)
    {
        this.t1 = t1;
        this.t2 = t2;
    }
    public override string ToString()
    {
        return string.Format("t1={0}, t2={1}", t1, t2);
    }
}
