using System;
using System.Collections.Generic;
using System.Text;
using Logic.Skill;
using UnityEngine;

namespace Logic.LogicObject
{
    public class Character : SceneObject
    {
        public enum CharacterEvent 
        {
            STARTSKILL = 100,
            CANCELSKILL,
            ENDSKILL,
            EXECUTEDISPLAYACTION,
            STOPDISPLAYACTION
        }
        public SkillManager skillManager { get; private set; }

        internal override void OnInit()
        {
            skillManager = new SkillManager(this);
        }
        internal override void ListenEvents()
        {
            base.ListenEvents();
        }
        public void ReleaseSkill(string path)
        {
            if (!skillManager.IsRunningSkill)
            {
                skillManager.ReleaseSkill(path);
            }
        }
        
        public bool IsRunningSkill
        {
            get { return skillManager.IsRunningSkill; }
        }
        public void CancelSkill()
        {
            if (IsRunningSkill)
            {
                this.skillManager.CancelSkill();
            }
        }

        internal override void OnUpdate(float deltaTime)
        {
            skillManager.Update(deltaTime);
            base.OnUpdate(deltaTime);
        }

    }
}
