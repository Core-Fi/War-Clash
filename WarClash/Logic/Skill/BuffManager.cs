using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Logic.Skill;
using System.IO;
using Logic.LogicObject;

namespace Logic.Skill
{
    public class BuffManager
    {
        private static Dictionary<string, Logic.Skill.Buff> buffs = new Dictionary<string, Logic.Skill.Buff>();

        public SceneObject so;
        //private static Dictionary<string, RuntimeSkill> runtimeskills = new Dictionary<string, RuntimeSkill>();
        private static Logic.Skill.Buff GetBuff(string path)
        {
            Logic.Skill.Buff skill = null;
            if (buffs.ContainsKey(path))
            {
                skill = buffs[path];
            }
            else
            {
                string text = File.ReadAllText(path);
                skill = Newtonsoft.Json.JsonConvert.DeserializeObject<Logic.Skill.Buff>(text);
                buffs[path] = skill;
            }
            return skill;
        }
        private Dictionary<string, BuffRuntime> buffRuntimes = new Dictionary<string, BuffRuntime>();
        public void AddBuff(string path)
        {
            Buff buff = GetBuff(path);
            BuffRuntime br = new BuffRuntime();
            br.Init(buff, new SkillRunningData(so, null, null), null);
            buffRuntimes.Add(path, br);
        }
        private List<string> needDelBuffs = new List<string>();
        public void Update(float deltaTime)
        {
            var itor = buffRuntimes.GetEnumerator();
            while (itor.MoveNext())
            {
                BuffRuntime br = itor.Current.Value;
                br.Breath(deltaTime);
            }
            itor.Dispose();
            if (needDelBuffs.Count > 0)
            {
                for (int i = 0; i < needDelBuffs.Count; i++)
                {
                    buffRuntimes.Remove(needDelBuffs[i]);
                }
                needDelBuffs.Clear();
            }
        }
    }
}