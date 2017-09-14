using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FastCollections;

namespace Logic.Map
{
    public class Map
    {
        public Dictionary<int, MapItem> MapDic = new BiDictionary<int, MapItem>();
    }
}
