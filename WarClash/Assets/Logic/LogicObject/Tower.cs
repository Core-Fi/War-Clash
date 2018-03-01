using System.Collections.Generic;
using Brainiac;
using Lockstep;
using Logic.Skill;

namespace Logic.LogicObject
{
    public class Tower : Building, ISkillable
    {
        private long _previousTime;
        private long _timeout = FixedMath.One;
        private List<IFixedAgent> _enemyList = new List<IFixedAgent>();
        public SkillManager SkillManager { get; private set; }
        public AIAgent AiAgent { get; private set; }

        internal override void OnInit(CreateInfo createInfo)
        {
            base.OnInit(createInfo);
            SkillManager = new SkillManager(this);
            var obj = AssetResources.LoadAssetImmediatly(Conf.Param2);
            OnBtLoad(Conf.Param2, obj);
        }
        private void OnBtLoad(string name, UnityEngine.Object obj)
        {
            BTAsset bt = UnityEngine.Object.Instantiate(obj) as BTAsset;
            AiAgent = new AIAgent(this, bt);
            AiAgent.Start();
        }
        internal override void ListenEvents()
        {
            base.ListenEvents();
        }

        internal override void OnFixedUpdate(long deltaTime)
        {
            AiAgent.Tick();
            SkillManager.FixedUpdate();
            base.OnFixedUpdate(deltaTime);
        }

        public bool ReleaseSkill(int id, SceneObject target)
        {
            if (!SkillManager.IsRunningSkill && Hp>0)
            {
                SkillManager.ReleaseSkill(id, target);
                return true;
            }
            else
                return false;
        }
    }
}
