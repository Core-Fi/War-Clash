using Logic.LogicObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiteNetLib.Utils;
using Lockstep;
using Logic.Map;
using UnityEngine;

namespace Logic
{
    public abstract class LockFrameCommand : IPool
    {
        public int Frame;

        public void Execute()
        {
            OnExecute();
            if (LogicCore.SP.WriteToLog)
            {
                WriteToLog(LogicCore.SP.Writer);
                LogicCore.SP.Writer.AppendLine();
            }
            Pool.SP.Recycle(this);
        }

        public virtual void WriteToLog(StringBuilder writer)
        {
            writer.Append(Frame);
        }
        public virtual void OnExecute()
        {
           
        }
        public virtual void Reset()
        {
            
        }
        public virtual void Serialize(NetDataWriter writer)
        {
        }
        public virtual void Deserialize(NetDataReader reader)
        {
        }
    }

    public abstract class PlayerOperateCommand : LockFrameCommand
    {
        public int Sender;
        public override void Serialize(NetDataWriter writer)
        {
            base.Serialize(writer);
            writer.Put(Sender);
        }
        public override void Deserialize(NetDataReader reader)
        {
            base.Deserialize(reader);
            Sender = reader.GetInt();
        }

        public override void WriteToLog(StringBuilder writer)
        {
            base.WriteToLog(writer);
            writer.Append("  " + Sender + "  " + this.GetType() + "  ");
        }
    }
    public class ReleaseSkillCommand : PlayerOperateCommand
    {
        public Vector3 Position;
        public int Receiver;
        public int Id;
        public override void OnExecute()
        {
            var c = LogicCore.SP.SceneManager.CurrentScene.GetObject<Character>(Sender);
            c.ReleaseSkill(Id);
        }

        public override void Deserialize(NetDataReader reader)
        {
            base.Deserialize(reader);
            Id = reader.GetInt();
        }

        public override void Serialize(NetDataWriter writer)
        {
            var msgid = (int)LockFrameMgr.LockFrameEvent.ReleaseSkill;
            writer.Put((short)msgid);
            base.Serialize(writer);
            writer.Put(Id);
        }
    }

    public class MoveCommand : PlayerOperateCommand
    {
        public override void OnExecute()
        {
            var player = LogicCore.SP.SceneManager.CurrentScene.GetObject(Sender) as Player;
            player.StateMachine.Start<MoveState>();
        }

        public override void WriteToLog(StringBuilder writer)
        {
            base.WriteToLog(writer);
        }

        public override void Serialize(NetDataWriter writer)
        {
            var msgid = (int)LockFrameMgr.LockFrameEvent.PlayerMoveMsg ;
            writer.Put((short)msgid);
            base.Serialize(writer);
        }

        public override void Deserialize(NetDataReader reader)
        {
            base.Deserialize(reader);
        }
    }

    public class StopCommand : PlayerOperateCommand
    {
        public override void OnExecute()
        {
            var player = LogicCore.SP.SceneManager.CurrentScene.GetObject(Sender) as Player;
            player.StateMachine.Start<IdleState>();
        }

        public override void Deserialize(NetDataReader reader)
        {
            base.Deserialize(reader);
        }

        public override void Serialize(NetDataWriter writer)
        {
            var msgid = (int)LockFrameMgr.LockFrameEvent.PlayerStopMsg ;
            writer.Put((short)msgid);
            base.Serialize(writer);
        }
    }

    public class ChangeForwardCommand : PlayerOperateCommand
    {
        public Vector3d Forward;
        public override void OnExecute()
        {
            var player = LogicCore.SP.SceneManager.CurrentScene.GetObject(Sender) as Player;
            player.Forward = Forward;
            player.Velocity = player.Forward * player.Speed;
        }
        public override void WriteToLog(StringBuilder writer)
        {
            base.WriteToLog(writer);
            writer.Append(Forward.ToStringRaw());
            writer.AppendLine();
        }
        public override void Deserialize(NetDataReader reader)
        {
            base.Deserialize(reader);
            Forward.x = reader.GetLong();
            Forward.y = reader.GetLong();
            Forward.z = reader.GetLong();
        }

        public override void Serialize(NetDataWriter writer)
        {
            var msgid = (int)LockFrameMgr.LockFrameEvent.PlayerRotateMsg ;
            writer.Put((short)msgid);
            base.Serialize(writer);
            writer.Put(Forward.x);
            writer.Put(Forward.y);
            writer.Put(Forward.z);
        }
    }

    public class ChangeStrategyCommand : PlayerOperateCommand
    {
        public byte Strategy;
        public override void OnExecute()
        {
            base.OnExecute();
            var player = LogicCore.SP.SceneManager.CurrentScene.GetObject<Player>(Sender);
            var strategy = (LockFrameMgr.Strategy)Strategy;
            if (strategy == LockFrameMgr.Strategy.FollowPlayer)
            {
                LogicCore.SP.SceneManager.CurrentScene.ForEachDo<Npc>((c) =>
                {
                    c.AiAgent.Blackboard.SetItem("strategy", (int)strategy);   
                });
            }
        }
        public override void Deserialize(NetDataReader reader)
        {
            base.Deserialize(reader);
            Strategy = reader.GetByte();
        }

        public override void Serialize(NetDataWriter writer)
        {
            var msgid = (int)LockFrameMgr.LockFrameEvent.CreateBuilding;
            writer.Put((short)msgid);
            base.Serialize(writer);
            writer.Put(Strategy);
        }
    }
    public class CreateBuildingCommand : PlayerOperateCommand
    {
        public int BuildingId;
        public Vector3d Position;
        public override void OnExecute()
        {
            var senderSo = LogicCore.SP.SceneManager.CurrentScene.GetObject<SceneObject>(Sender);
            var createInfo = Pool.SP.Get<BuildingCreateInfo>();
            createInfo.BuildingId = BuildingId;
            createInfo.Position = Position;// mapItem.Position;
            var barack = LogicCore.SP.SceneManager.CurrentScene.CreateBuilding(createInfo);
            barack.Radius = FixedMath.One*3;
            if(senderSo!=null)
                barack.Team = senderSo.Team;
            else
            {
                barack.Team = Team.Team2;
            }
            GridService.TagAs(barack.Position, barack, NodeType.BeTaken);
            JPSAStar.active.SetUnWalkable(barack.Position);
        }
        public override void WriteToLog(StringBuilder writer)
        {
            base.WriteToLog(writer);
        }
        public override void Deserialize(NetDataReader reader)
        {
            base.Deserialize(reader);
            BuildingId = reader.GetInt();
        }

        public override void Serialize(NetDataWriter writer)
        {
            var msgid = (int)LockFrameMgr.LockFrameEvent.CreateBuilding;
            writer.Put((short)msgid);
            base.Serialize(writer);
            writer.Put(BuildingId);
        }
    }
    public class CreateNpcCommand : PlayerOperateCommand
    {
        public int NpcId;
        public byte Team;
        public long PosiX;
        public long PosiZ;
        public override void OnExecute()
        {
            var sender = LogicCore.SP.SceneManager.CurrentScene.GetObject(Sender);
            var createInfo = Pool.SP.Get<NpcCreateInfo>();
            createInfo.NpcId = NpcId;
            createInfo.Position =
                new Vector3d(PosiX, 0, PosiZ);
            var npc = LogicCore.SP.SceneManager.CurrentScene.CreateSceneObject<Npc>(createInfo);
            if(sender!=null)
                npc.Team = sender.Team;
            else
            {
                npc.Team = LogicObject.Team.Team2;
            }
            npc.Radius = FixedMath.One/2;
            npc.Forward = new Vector3d(Vector3.forward);
        }
        public override void WriteToLog(StringBuilder writer)
        {
            base.WriteToLog(writer);
        }
        public override void Deserialize(NetDataReader reader)
        {
            base.Deserialize(reader);
            NpcId = reader.GetInt();
            PosiX = reader.GetLong();
            PosiZ = reader.GetLong();
        }

        public override void Serialize(NetDataWriter writer)
        {
            var msgid = (int)LockFrameMgr.LockFrameEvent.CreateNpc ;
            writer.Put((short)msgid);
            base.Serialize(writer);
            writer.Put(NpcId);
            writer.Put(PosiX);
            writer.Put(PosiZ);
        }
    }
    public class CreateMainPlayerCommand : PlayerOperateCommand
    {
        public override void OnExecute()
        {
            var mainPlayer = LogicCore.SP.SceneManager.CurrentScene.CreateSceneObject<MainPlayer>(Sender);
            mainPlayer.Team = Team.Team1;
            mainPlayer.Position = Vector3d.zero;
            mainPlayer.Radius = FixedMath.One/2;
        }
        public override void WriteToLog(StringBuilder writer)
        {
            base.WriteToLog(writer);
        }
        public override void Deserialize(NetDataReader reader)
        {
            base.Deserialize(reader);

        }

        public override void Serialize(NetDataWriter writer)
        {
            var msgid = (int)LockFrameMgr.LockFrameEvent.CreatePlayer;
            writer.Put((short)msgid);
            base.Serialize(writer);
        }
    }
    public class CreatePlayerCommand : PlayerOperateCommand
    {
        public override void OnExecute()
        {
            var player = LogicCore.SP.SceneManager.CurrentScene.CreateSceneObject<Player>(Sender);
            player.Position = Vector3d.zero;
            player.Team = Team.Team1;
        }
        public override void WriteToLog(StringBuilder writer)
        {
            base.WriteToLog(writer);
        }
        public override void Deserialize(NetDataReader reader)
        {
            base.Deserialize(reader);
        }

        public override void Serialize(NetDataWriter writer)
        {
           
            var msgid = (int)LockFrameMgr.LockFrameEvent.CreatePlayer ;
            writer.Put((short)msgid);
            base.Serialize(writer);
        }
    }

}
