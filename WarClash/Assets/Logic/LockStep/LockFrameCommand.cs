using Logic.LogicObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lockstep;
using UnityEngine;

namespace Logic
{
    public abstract class LockFrameCommand : IPool
    {
        public int Sender;
        public int Frame;

        public void Execute()
        {
            OnExecute();
            Pool.SP.Recycle(this);
        }

        public virtual void OnExecute()
        {
           
        }
        public virtual void Reset()
        {
            
        }
    }

    public class ReleaseSkillCommand : LockFrameCommand
    {
        public int frame;
        public Vector3 position;
        public int receiver;
        public int id;
        public override void OnExecute()
        {
            var c = LogicCore.SP.SceneManager.currentScene.GetObject<Character>(Sender);
           // c.ReleaseSkill(path);
        }
    }

    public class MoveCommand : LockFrameCommand
    {
        public override void OnExecute()
        {
            var player = LogicCore.SP.SceneManager.currentScene.GetObject(Sender) as Player;
            player.StateMachine.Start(new MoveState());
        }
    }

    public class StopCommand : LockFrameCommand
    {
        public override void OnExecute()
        {
            var player = LogicCore.SP.SceneManager.currentScene.GetObject(Sender) as Player;
            player.StateMachine.Start(new IdleState());
        }
    }

    public class ChangeForwardCommand : LockFrameCommand
    {
        public Vector3d Forward;
        public override void OnExecute()
        {
            var player = LogicCore.SP.SceneManager.currentScene.GetObject(Sender) as Player;
            player.Forward = Forward;
        }
    }

}
