using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Logic
{
    public class IDManager : Singleton<IDManager>
    {
        private int CurID = 1000;
        public int GetID()
        {
            return ++CurID;
        }


    }
}
