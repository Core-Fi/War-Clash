using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LiteNetLib.Utils;
using Lockstep;
using UnityEngine;

namespace Logic
{
    public class LockFrameMgr
    {
        public static readonly int FixedFrameRate = 15;
        private int m_PingAverage;
        private int m_PingVariance;
        private List<int> m_pingRecords = new List<int>();
        private readonly Queue<LinkedListNode<LockFrameCommand>> _cachCmds = new Queue<LinkedListNode<LockFrameCommand>>(); 
        private readonly LinkedList<LockFrameCommand> _frames = new LinkedList<LockFrameCommand>();

        public LockFrameMgr ()
        {
            EventDispatcher.ListenEvent((int)NetEventList.BattleStart, (sender, msg) => LogicCore.SP.RealFixedFrame=0);
            EventDispatcher.ListenEvent((int)NetEventList.PlayerMoveMsg, (sender, e)=> {OnGetReceiveLockstepMsg<MoveCommand>(e);});
            EventDispatcher.ListenEvent((int)NetEventList.PlayerRotateMsg, (sender, e) => { OnGetReceiveLockstepMsg<ChangeForwardCommand>(e); });
            EventDispatcher.ListenEvent((int)NetEventList.PlayerStopMsg, (sender, e) => { OnGetReceiveLockstepMsg<StopCommand>(e); });
            EventDispatcher.ListenEvent((int)NetEventList.CreatePlayer, (sender, e) => { OnGetReceiveLockstepMsg<CreatePlayerCommand>(e); });
            EventDispatcher.ListenEvent((int)NetEventList.SaveToLog, (sender, e) => { File.WriteAllText(Application.dataPath+"/log.txt", LogicCore.SP.Writer.ToString()); });
        }

        private void OnGetReceiveLockstepMsg<T>(EventMsg e) where T : LockFrameCommand
        {
            var msg = e as EventSingleArgs<NetDataReader>;
            var cmd = Pool.SP.Get<T>();
            cmd.Deserialize(msg.value);
            AddFrameCommand(cmd);
        }
       
        public void SendCommand(LockFrameCommand cmd)
        {
            NetDataWriter w = new NetDataWriter(true);
            cmd.Serialize(w);
            EventDispatcher.FireEvent((int)UIEventList.SendNetMsg, this, EventGroup.NewArg<EventSingleArgs<NetDataWriter>, NetDataWriter>(w));
        }
        private void AddFrameCommand(LockFrameCommand cmd)
        {
            LinkedListNode<LockFrameCommand> cmdNode = null;
            if (_cachCmds.Count > 0)
            {
                cmdNode = _cachCmds.Dequeue();
                cmdNode.Value = cmd;
            }
            else
            {
                cmdNode = new LinkedListNode<LockFrameCommand>(cmd);
            }
            _frames.AddLast(cmdNode);
        }
        private int record = 0;
        public void FixedUpdate()
        {
            while (_frames.First!=null && _frames.First.Value.Frame<=LogicCore.SP.RealFixedFrame)
            {
                _frames.First.Value.Execute();
                _frames.First.Value = null;
                _cachCmds.Enqueue(_frames.First);
                _frames.RemoveFirst();
            }
            record++;
            if (record%2 == 0)
            {
                m_pingRecords.Add(UnityEngine.Random.Range(200, 300));
                PingVariance();
                PingVariance2();
            }
        }
        public void PingVariance()
        {
            this.m_PingAverage = 0;
            this.m_PingVariance = 0;
            if ((this.m_pingRecords != null) && (this.m_pingRecords.Count > 0))
            {
                double num = 0.0;
                double num2 = 0.0;
                for (int i = 0; i < this.m_pingRecords.Count; i++)
                {
                    num += (double)this.m_pingRecords[i];
                }
                num2 = num / ((double)this.m_pingRecords.Count);
                this.m_PingAverage = (int)num2;
                num2 = 0.0;
                num = 0.0;
                for (int j = 0; j < this.m_pingRecords.Count; j++)
                {
                    num += Math.Pow(((double)this.m_pingRecords[j]) - num2, 2.0);
                }
                num2 = num / ((double)this.m_pingRecords.Count);
                this.m_PingVariance = (int)num2;
            }
        }

        public void PingVariance2()
        {
            double num = 0.0;
            double num2 = 0.0;
            for (int i = 0; i < this.m_pingRecords.Count; i++)
            {
                num += (double)this.m_pingRecords[i];
            }
            num2 = num / ((double)this.m_pingRecords.Count);
            int num4 = Mathf.FloorToInt(((float)num2) / 100f) * 100;
            this.m_PingAverage = num4;
            num = 0.0;
            for (int j = 0; j < this.m_pingRecords.Count; j++)
            {
                num += Math.Pow(((double)this.m_pingRecords[j]) - num2, 2.0);
            }
            num2 = num / ((double)this.m_pingRecords.Count);
            num4 = Mathf.FloorToInt(((float)num2) / 1000f) * 0x3e8;
            this.m_PingVariance = num4;
        }
    }
}
