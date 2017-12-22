using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lockstep;

namespace Logic
{
    public interface IAlignmentAgent
    {
        Vector3d Position { get; set; }
        int AlignmentX { get; set; }
        int AlignmentY { get; set; }
    }
}
