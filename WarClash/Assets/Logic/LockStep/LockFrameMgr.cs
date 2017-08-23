using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Logic
{
    public class LockFrameMgr
    {
        private int currentFrame = 0;
        private LinkedList<LockFrameCommand> frames = new LinkedList<LockFrameCommand>();
        public void AddFrameCommand(LockFrameCommand cmd)
        {
            cmd.Frame = LogicCore.SP.RealFixedFrame + UnityEngine.Random.Range(2,3);
            frames.AddLast(cmd);
        }
        public void FixedUpdate()
        {
            currentFrame++;
            while (frames.First!=null && frames.First.Value.Frame<=currentFrame)
            {
                frames.First.Value.Execute();
                frames.RemoveFirst();
            }
        }
    }
}
