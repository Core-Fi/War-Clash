using Logic.LogicObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Logic
{
    public abstract class LockFrameCommand
    {
        public int frame;
        public Vector3 position;
        public int sender;
        public int receiver;
        public abstract void Execute();
    }

    public class ReleaseSkillCommand : LockFrameCommand
    {
        public string path;
        public override void Execute()
        {
            var c = LogicCore.SP.sceneManager.currentScene.GetObject<Character>(sender);
            c.ReleaseSkill(path);
        }
    }
}
