using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Logic
{
    public class LockFrameMgr
    {
        private int currentFrame = 0;
        private LinkedList<LockFrameCommand> frames = new LinkedList<LockFrameCommand>();
        public void AddFrameCommand(LockFrameCommand cmd)
        {
            frames.AddLast(cmd);
        }
        public void Update()
        {
            while (frames.First!=null && frames.First.Value.frame<=currentFrame)
            {
                frames.First.Value.Execute();
                frames.RemoveFirst();
            }
        }
    }
}
