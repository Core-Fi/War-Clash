using System.Collections.Generic;
using Logic.LogicObject;
using UnityEngine;
using Logic.Components;

namespace Logic.Skill
{
    public class SkillManager : SceneObjectBaseComponent
    {
        public enum Event
        {
            Startskill,
            Cancelskill,
            Endskill,
            Executedisplayaction,
            Stopdisplayaction,
        }
        private static readonly Dictionary<string, Skill> skills =new Dictionary<string, Skill>();
        private static Dictionary<int, string> skill_index = new Dictionary<int, string>();

        public static void LoadSkillIndexFiles()
        {
            if (Application.isPlaying)
            {
                skill_index = Logic.Skill.SkillUtility.LoadIndexFile("skill_index.bytes");
            }
            else
            {
                skill_index = Logic.Skill.SkillUtility.LoadIndexFile("Skills/skill_index.bytes");
            }
            
        }
        public static Skill GetSkill(string path)
        {
            Skill skill = null;
            if (Skills.ContainsKey(path))
            {
                skill = Skills[path];
            }
            else
            {
                skill = Logic.Skill.SkillUtility.GetTimelineGroup<Skill>(path);
                Skills[path] = skill;
            }
            return skill;
        }
        public RuntimeSkill RunningSkill { get; private set; }
        private SceneObject so;

        public bool IsRunningSkill
        {
            get { return RunningSkill != null; }
        }

        public static Dictionary<string, Skill> Skills
        {
            get
            {
                return skills;
            }
        }

        public void CancelSkill()
        {
            this.so.EventGroup.FireEvent((int)Event.Cancelskill, so, EventGroup.NewArg<EventSingleArgs<string>, string>(RunningSkill.SourceData.path));
            RunningSkill.Cancel();
            Pool.SP.Recycle(RunningSkill);
            RunningSkill = null;
        }
        public SkillManager(SceneObject so)
        {
            this.so = so;
        }
        public void ReleaseSkill(int id)
        {
            if(skill_index.Count==0)
                LoadSkillIndexFiles();
            RuntimeData srd = new RuntimeData(so, null, null);
            string path = skill_index[id];
            ReleaseSkill(path, srd);
        }
        public void ReleaseSkill(string path)
        {
            RuntimeData srd = new RuntimeData(so, null, null);
            ReleaseSkill(path, srd);
        }
        internal void ReleaseSkill(int id, SceneObject target)
        {
            RuntimeData srd = new RuntimeData(so, target, null);
        
            ReleaseSkill(id, srd);
        }
        private void ReleaseSkill(int id, RuntimeData srd)
        {
            if (skill_index.Count == 0)
                LoadSkillIndexFiles();
            string path = skill_index[id];
            ReleaseSkill(path, srd);
        }
        private void ReleaseSkill(string path, RuntimeData srd)
        {
            var skill = GetSkill(path);
            if (srd.receiver != null && skill.ForceFaceToTarget)
            {
                so.Forward = (srd.receiver.Position - so.Position).Normalize();
            }
            RunningSkill = Pool.SP.Get(typeof(RuntimeSkill)) as RuntimeSkill;
            RunningSkill.Init(skill, srd);
            RunningSkill.FinishAction = OnFinish;
            this.so.EventGroup.FireEvent((int)Event.Startskill, so, EventGroup.NewArg<EventSingleArgs<string>, string>(path));
        }
        internal void Update(float deltaTime)
        {
            if (RunningSkill != null)
            {
                RunningSkill.Breath(deltaTime);
            }
        }

        internal void FixedUpdate()
        {
            if (RunningSkill != null)
            {
                RunningSkill.FixedBreath();
            }
        }
        internal void OnFinish()
        {
            so.EventGroup.FireEvent((int)Event.Endskill, so, EventGroup.NewArg<EventSingleArgs<string>, string>(RunningSkill.SourceData.path));
            Pool.SP.Recycle(RunningSkill);
            RunningSkill = null;
        }
    }
}
