using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Logic.Components
{
    class NpcComponent:SceneObjectBaseComponent
    {
        [JsonProperty]
        public int test;
    }
}
