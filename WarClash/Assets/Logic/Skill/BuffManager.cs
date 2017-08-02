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
        private static Dictionary<int, string> buff_index = new Dictionary<int, string>();

        public static void LoadSkillIndexFiles()
        {
            buff_index = Logic.Skill.SkillUtility.LoadIndexFile("/Buffs");
        }

        public SceneObject so;
        //private static Dictionary<string, RuntimeSkill> runtimeskills = new Dictionary<string, RuntimeSkill>();
        private static Logic.Skill.Buff GetBuff(string path)
        {
            Logic.Skill.Buff b = null;
            if (buffs.ContainsKey(path))
            {
                b = buffs[path];
            }
            else
            {
                string text = File.ReadAllText(path);
                b = Newtonsoft.Json.JsonConvert.DeserializeObject<Logic.Skill.Buff>(text);
                buffs[path] = b;
            }
            return b;
        }
        private Dictionary<string, BuffRuntime> buffRuntimes = new Dictionary<string, BuffRuntime>();
        public void AddBuff(string path)
        {
            Buff buff = GetBuff(path);
            BuffRuntime br = new BuffRuntime();
            br.Init(buff, new RuntimeData(so, null, null), null);
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