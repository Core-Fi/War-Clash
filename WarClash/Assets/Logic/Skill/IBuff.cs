using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Logic.Skill
{
    public  interface IBuff
    {
        BuffManager BuffManager { get; }
        void AddBuff(int id);
    }
}
