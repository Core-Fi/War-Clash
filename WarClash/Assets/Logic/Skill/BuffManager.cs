using System;
using System.Collections.Generic;
using Logic.LogicObject;
using Logic.Components;

namespace Logic.Skill
{
    public class BuffManager : SceneObjectBaseComponent
    {

        private static readonly Dictionary<string, Logic.Skill.Buff> buffs = new Dictionary<string, Logic.Skill.Buff>();
        private static Dictionary<int, string> _buffIndex = new Dictionary<int, string>();
        private readonly List<BuffRuntime> _buffRuntimeList = new List<BuffRuntime>();
        private readonly List<BuffRuntime> _toAddBuffRuntimeList = new List<BuffRuntime>();
        public SceneObject SceneObject;
        public static void LoadBuffIndexFiles()
        {
            if (UnityEngine.Application.isPlaying)
            {
                _buffIndex = Logic.Skill.SkillUtility.LoadIndexFile("buff_index.bytes");
            }
            else
            {
                _buffIndex = Logic.Skill.SkillUtility.LoadIndexFile("/Buffs/buff_index.bytes");
            }
        }
        private static Logic.Skill.Buff GetBuff(string path)
        {
            Logic.Skill.Buff b = null;
            if (buffs.ContainsKey(path))
            {
                b = buffs[path];
            }
            else
            {
                b = Logic.Skill.SkillUtility.GetTimelineGroup<Buff>(path);
                buffs[path] = b;
            }
            return b;
        }

        public BuffManager(SceneObject so)
        {
            SceneObject = so;
        }

        public BuffRuntime GetBuffRuntime(int id)
        {
            for (int i = 0; i < _buffRuntimeList.Count; i++)
            {
                var br = _buffRuntimeList[i];
                if (br.SourceData.ID.Equals(id))
                {
                    return br;
                }
            }
            return null;
        }

        public bool HasBuff(int id)
        {
            for (int i = 0; i < _buffRuntimeList.Count; i++)
            {
                var br = _buffRuntimeList[i];
                if (br.SourceData.ID.Equals(id))
                {
                    return true;
                }
            }
            return false;
        }
        public void AddBuff(int id)
        {
            if(HasBuff(id))return;
            if(_buffIndex.Count == 0)
                LoadBuffIndexFiles();
            string path;
            if (!_buffIndex.TryGetValue(id, out path))
            {
                throw new Exception("Buff ID 不存在 "+id);
            }
            Buff buff = GetBuff(path);
            BuffRuntime br = new BuffRuntime();
            br.Init(buff, new RuntimeData(SceneObject, null, null));
            if (_isTraversing)
            {
                _toAddBuffRuntimeList.Add(br);
               //todo:呼吸一帧
            }
            else
            {
                _buffRuntimeList.Add(br);
               // _buffRuntimes.Add(id, br);
            }
        }
        private bool _isTraversing;
        public void Update(float deltaTime)
        {
            _isTraversing = true;
            for (int i = 0; i < _buffRuntimeList.Count; i++)
            {
                BuffRuntime br = _buffRuntimeList[i];
                br.Breath(deltaTime);
                if (!br.isRunning)
                {
                    _buffRuntimeList.RemoveAt(i);
                  //  _buffRuntimes.Remove(br.SourceData.ID);
                    i--;
                }
            }
            _isTraversing = false;
        }

        public void FixedUpdate()
        {
            _isTraversing = true;
            for (int i = 0; i < _buffRuntimeList.Count; i++)
            {
                BuffRuntime br = _buffRuntimeList[i];
                br.FixedBreath();
                if (!br.isRunning)
                {
                    _buffRuntimeList.RemoveAt(i);
                   // _buffRuntimes.Remove(br.SourceData.ID);
                    i--;
                }
            }
            _isTraversing = false;
            AfterFixedUpdate();
        }

        private void AfterUpdate(float deltaTime)
        {
            for (int i = 0; i < _toAddBuffRuntimeList.Count; i++)
            {
                var br = _toAddBuffRuntimeList[i];
                br.Breath(deltaTime);
                if (br.isRunning)
                {
                    _buffRuntimeList.Add(br);
                  //  _buffRuntimes.Add(br.SourceData.ID, br);
                }
            }
            _toAddBuffRuntimeList.Clear();
        }
        private void AfterFixedUpdate()
        {
            for (int i = 0; i < _toAddBuffRuntimeList.Count; i++)
            {
                var br = _toAddBuffRuntimeList[i];
                br.FixedBreath();
                if (br.isRunning)
                {
                    _buffRuntimeList.Add(br);
                   // _buffRuntimes.Add(br.SourceData.ID, br);
                }
            }
            _toAddBuffRuntimeList.Clear();
        }

      
    }
}