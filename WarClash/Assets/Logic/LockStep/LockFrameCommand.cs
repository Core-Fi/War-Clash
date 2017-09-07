using Logic.LogicObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiteNetLib.Utils;
using Lockstep;
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
            Frame = reader.GetShort();
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
        public Vector3 position;
        public int receiver;
        public int id;
        public override void OnExecute()
        {
            var c = LogicCore.SP.SceneManager.currentScene.GetObject<Character>(Sender);
           // c.ReleaseSkill(path);
        }
    }

    public class MoveCommand : PlayerOperateCommand
    {
        public override void OnExecute()
        {
            var player = LogicCore.SP.SceneManager.currentScene.GetObject(Sender) as Player;
            player.StateMachine.Start<MoveState>();
            player.StateMachine.Update();
        }

        public override void WriteToLog(StringBuilder writer)
        {
            base.WriteToLog(writer);
        }

        public override void Serialize(NetDataWriter writer)
        {
            var msgid = (int) NetEventList.PlayerMoveMsg ;
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
            var player = LogicCore.SP.SceneManager.currentScene.GetObject(Sender) as Player;
            player.StateMachine.Start<IdleState>();
        }

        public override void Deserialize(NetDataReader reader)
        {
            base.Deserialize(reader);
        }

        public override void Serialize(NetDataWriter writer)
        {
            var msgid = (int)NetEventList.PlayerStopMsg ;
            writer.Put((short)msgid);
            base.Serialize(writer);
        }
    }

    public class ChangeForwardCommand : PlayerOperateCommand
    {
        public Vector3d Forward;
        public override void OnExecute()
        {
            var player = LogicCore.SP.SceneManager.currentScene.GetObject(Sender) as Player;
            player.Forward = Forward;
            
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
            var msgid = (int)NetEventList.PlayerRotateMsg ;
            writer.Put((short)msgid);
            base.Serialize(writer);
            writer.Put(Forward.x);
            writer.Put(Forward.y);
            writer.Put(Forward.z);
        }
    }
    public class CreateNpcCommand : PlayerOperateCommand
    {
        public override void OnExecute()
        {
            var npc = LogicCore.SP.SceneManager.currentScene.CreateSceneObject<Npc>(Sender);
            npc.Team = Team.Team2;
            npc.Position = new Vector3d(Vector3.left * 6);
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

            var msgid = (int)NetEventList.CreateNpc ;
            writer.Put((short)msgid);
            base.Serialize(writer);
        }
    }
    public class CreatePlayerCommand : PlayerOperateCommand
    {
        public override void OnExecute()
        {
            var player = LogicCore.SP.SceneManager.currentScene.CreateSceneObject<Player>(Sender);
            player.Position = new Vector3d(-Vector3.left*6);
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
           
            var msgid = (int)NetEventList.CreatePlayer ;
            writer.Put((short)msgid);
            base.Serialize(writer);
        }
    }

}
