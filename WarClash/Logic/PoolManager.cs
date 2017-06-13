using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Logic
{
    public interface IPool
    {
        void Reset();
    }
    public class PoolManager : Singleton<PoolManager>
    {
        public Dictionary<Type, Queue<IPool>> pool_dic = new Dictionary<Type, Queue<IPool>>();
        public void Recycle(IPool iPool)
        {
            Type type = iPool.GetType();
            if (!pool_dic.ContainsKey(type))
            {
                pool_dic[type] = new Queue<IPool>();
            }
            pool_dic[type].Enqueue(iPool);
        }

        public IPool Get(Type type) 
        {
            IPool obj = null;
            if (pool_dic.ContainsKey(type) && pool_dic[type].Count > 0)
            {
                obj = pool_dic[type].Dequeue();
            }
            else
            {
                obj = Activator.CreateInstance(type) as IPool;
            }
            obj.Reset();
            return obj;
        }
    }
}
