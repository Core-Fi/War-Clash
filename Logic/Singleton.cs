using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Logic
{
    public abstract class Singleton<T>
    {
        public static T SP
        {
            get
            {
                if (_SP == null)
                {
                    _SP = Activator.CreateInstance<T>();
                }
                return _SP;
            }
        }

        private static T _SP;
    }
}
