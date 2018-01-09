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

        public static void LoadBuffIndexFiles()
        {
            buff_index = Logic.Skill.SkillUtility.LoadIndexFile("/Buffs/buff_index.bytes");
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
        private Dictionary<int, BuffRuntime> buffRuntimes = new Dictionary<int, BuffRuntime>();
        private List<BuffRuntime> _delayBuffs = new List<BuffRuntime>();
        public void AddBuff(int id)
        {
            if(buff_index.Count == 0)
                LoadBuffIndexFiles();
            string path;
            if (!buff_index.TryGetValue(id, out path))
            {
                throw new Exception("Buff ID 不存在");
            }
            Buff buff = GetBuff(path);
            BuffRuntime br = new BuffRuntime();
            br.Init(buff, new RuntimeData(so, null, null), null);
            if (_isTraversing)
            {
                _delayBuffs.Add(br);
            }
            else
            {
                buffRuntimes.Add(id, br);
            }
        }
        private List<int> _needDelBuffs = new List<int>();
        private bool _isTraversing;
        public void Update(float deltaTime)
        {
            _isTraversing = true;
            var itor = buffRuntimes.GetEnumerator();
            while (itor.MoveNext())
            {
                BuffRuntime br = itor.Current.Value;
                br.Breath(deltaTime);
                if (!br.isRunning)
                {
                    _needDelBuffs.Add(br.sourceData.ID);
                }
            }
            itor.Dispose();
            _isTraversing = false;
            for (int i = 0; i < _delayBuffs.Count; i++)
            {
                buffRuntimes.Add(_delayBuffs[i].sourceData.ID, _delayBuffs[i]);
            }
            if (_needDelBuffs.Count > 0)
            {
                for (int i = 0; i < _needDelBuffs.Count; i++)
                {
                    buffRuntimes.Remove(_needDelBuffs[i]);
                }
                _needDelBuffs.Clear();
            }
        }

        public void FixedUpdate()
        {
            _isTraversing = true;
            var itor = buffRuntimes.GetEnumerator();
            while (itor.MoveNext())
            {
                BuffRuntime br = itor.Current.Value;
                br.FixedBreath();
                if (!br.isRunning)
                {
                    _needDelBuffs.Add(br.sourceData.ID);
                }
            }
            itor.Dispose();
            _isTraversing = false;
            for (int i = 0; i < _delayBuffs.Count; i++)
            {
                buffRuntimes.Add(_delayBuffs[i].sourceData.ID, _delayBuffs[i]);
            }
            if (_needDelBuffs.Count > 0)
            {
                for (int i = 0; i < _needDelBuffs.Count; i++)
                {
                    buffRuntimes.Remove(_needDelBuffs[i]);
                }
                _needDelBuffs.Clear();
            }
        }
    }
}