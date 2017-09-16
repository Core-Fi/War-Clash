using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Logic
{
    public class IDManager : Singleton<IDManager>
    {
        private int CurID = 1000;
        private Queue<int> IdPool = new Queue<int>(); 
        public int GetID()
        {
            if (IdPool.Count > 0)
            {
                return IdPool.Dequeue();
            }
            else
            {
                return ++CurID;
            }
        }

        public void ReturnID(int id)
        {
            IdPool.Enqueue(id);
        }


    }
}
