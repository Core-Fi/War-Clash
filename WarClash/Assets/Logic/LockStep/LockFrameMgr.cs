//#define LocalDebug
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FastCollections;
using LiteNetLib.Utils;
using Lockstep;
using Logic.Skill;
using UnityEngine;

namespace Logic
{
    public class LockFrameMgr
    {
        public enum LockFrameEvent
        {
            LockStepFrame,
            SaveToLog,
            BattleStart,
            PlayerMoveMsg,
            PlayerStopMsg,
            PlayerRotateMsg,
            CreateMainPlayer,
            CreatePlayer,
            CreateNpc,
            ReleaseSkill,
            CreateBuilding
        }
        public static readonly int FixedFrameRate = 15;

        public int ServerFrameCount
        {
            get { return _serverFrameCount; }
        }
        public int LocalFrameCount
        {
            get { return _localFrameCount; }
        }
        private readonly Dictionary<int, Action<int, NetDataReader>> _lockstepCommandDic = new Dictionary<int, Action<int, NetDataReader>>(); 
        private int _serverFrameCount;
        private int _localFrameCount;
        private int m_PingAverage;
        private int m_PingVariance;
        private List<int> m_pingRecords = new List<int>();
        private readonly FastQueue<LockFrameCommand> _frames = new FastQueue<LockFrameCommand>();
        public LockFrameMgr ()
        {
            EventDispatcher.ListenEvent((int)NetEventList.LockStepMsg, OnGetLockstepMsg);
            _lockstepCommandDic.Add((int)LockFrameEvent.BattleStart, OnBattleStart);
            _lockstepCommandDic.Add((int)LockFrameEvent.PlayerMoveMsg, OnGetReceiveLockstepMsg<MoveCommand>);
            _lockstepCommandDic.Add((int)LockFrameEvent.ReleaseSkill, OnGetReceiveLockstepMsg<ReleaseSkillCommand>);
            _lockstepCommandDic.Add((int)LockFrameEvent.CreateMainPlayer, OnGetReceiveLockstepMsg<CreateMainPlayerCommand>);
            _lockstepCommandDic.Add((int)LockFrameEvent.PlayerRotateMsg, OnGetReceiveLockstepMsg<ChangeForwardCommand>);
            _lockstepCommandDic.Add((int)LockFrameEvent.PlayerStopMsg, OnGetReceiveLockstepMsg<StopCommand>);
            _lockstepCommandDic.Add((int)LockFrameEvent.CreatePlayer, OnGetReceiveLockstepMsg<CreatePlayerCommand>);
            _lockstepCommandDic.Add((int)LockFrameEvent.CreateNpc, OnGetReceiveLockstepMsg<CreateNpcCommand>);
            _lockstepCommandDic.Add((int)LockFrameEvent.CreateBuilding, OnGetReceiveLockstepMsg<CreateBuildingCommand>);
            _lockstepCommandDic.Add((int)LockFrameEvent.SaveToLog, (i, e) => { File.WriteAllText(Application.dataPath+"/log.txt", LogicCore.SP.Writer.ToString()); });
        }

        private void OnGetLockstepMsg(object o, EventMsg e)
        {
            var msg = e as EventSingleArgs<NetDataReader>;
            var reader = msg.value;
            var frameCount = reader.GetShort();
            _serverFrameCount = Mathf.Max(_serverFrameCount, frameCount);
            while (reader.Position<msg.value.Data.Length-1)
            {
                var cmdId = reader.GetShort();
                _lockstepCommandDic[cmdId].Invoke(frameCount, reader);
            }
        }

        private void OnBattleStart(int frame, NetDataReader reader)
        {
            _localFrameCount = 0;
            var randomSeed = reader.GetInt();
            UnityEngine.Random.InitState(randomSeed);
        }
        private void OnGetReceiveLockstepMsg<T>(int frame, NetDataReader reader) where T : LockFrameCommand
        {
            var cmd = Pool.SP.Get<T>();
            cmd.Frame = frame;
            cmd.Deserialize(reader);
            _frames.Add(cmd);
        }
        public void SendCommand(LockFrameCommand cmd)
        {
#if LocalDebug
            cmd.Execute();
            return;
#endif
           
            NetDataWriter w = new NetDataWriter(true);
            cmd.Serialize(w);
            EventDispatcher.FireEvent(UIEventList.SendNetMsg.ToInt(), this, EventGroup.NewArg<EventSingleArgs<NetDataWriter>, NetDataWriter>(w));
        }
        private int record = 0;
        public void FixedUpdate()
        {
#if LocalDebug
           _serverFrameCount++;
#endif
            for (int i = 0; i < 2; i++)
            {
                if (_localFrameCount > _serverFrameCount || _serverFrameCount == 0) return;
                while (_frames.Count > 0 && _frames.Peek().Frame == _localFrameCount)
                {
                    var cmd = _frames.Pop();
                    cmd.Execute();
                }
                LogicCore.SP.SceneManager.FixedUpdate();
                EventManager.FixedUpdate();
                _localFrameCount++;
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
