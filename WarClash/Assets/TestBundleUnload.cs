
using Logic.Components;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class TestBundleUnload : MonoBehaviour
{
    public class TestComps
    {
        List<SceneObjectBaseComponent> l = new List<SceneObjectBaseComponent>();
        public TestComps()
        {
            var t = new TransformComponent();
            var n = new NpcComponent();
            l.Add(t);
            l.Add(n);
        }
    }
	// Use this for initialization
	void OnEnable ()
	{
        TestComps t = new TestComps();
        JsonSerializerSettings setting = new JsonSerializerSettings();
        setting.ContractResolver = new ShouldSerializeContractResolver();
        setting.Formatting = Formatting.Indented;
        setting.TypeNameHandling = TypeNameHandling.All;
        var str = Newtonsoft.Json.JsonConvert.SerializeObject(t, setting);
        var nl = JsonConvert.DeserializeObject(str, setting) as TestComps;

        //var a = GetComponent<Animator>();
        //   a.Play("Standing 1H Magic Attack 01", -1, 0);
        //	    AssetResources.LoadAssetImmediatly("WK_archer.prefab");
    }
	
	// Update is called once per frame
	void OnDisable () {
		//AssetResources.UnloadAsset("WK_archer.prefab");
  //      GC.Collect();
	 //   Resources.UnloadUnusedAssets();
	}
    public class ShouldSerializeContractResolver : DefaultContractResolver
    {
        public new static readonly ShouldSerializeContractResolver Instance = new ShouldSerializeContractResolver();

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);
            var attributes = member.GetCustomAttributes(false);
            property.ShouldSerialize = instance => {
                return attributes.Length > 0 && attributes[0] is JsonPropertyAttribute;
            };

            return property;
        }
    }
}
