using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FastCollections;
using UnityEngine;

namespace Logic.Config
{
    public interface IBaseConf
    {
        ConfItem GetItem(int id);
    }
    public abstract class ConfItem
    {
        public int Id;

    }
    public class BuildingConfItem : ConfItem
    {
        public string ResPath;
        public int ArmyId;
    }
    public class BuildingConf : ScriptableObject, IBaseConf
    {
        public List<BuildingConfItem> Dic = new List<BuildingConfItem>();
        public ConfItem GetItem(int id)
        {
            return Dic[id];
        }
    }
}
