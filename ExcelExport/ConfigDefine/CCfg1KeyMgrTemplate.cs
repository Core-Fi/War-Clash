using System;
using System.Collections.Generic;
using ProtoBuf;
using System.IO;
using UnityEngine;

namespace Logic.TabConfig
{
	public  class CCfg1KeyMgrTemplate<TManager, TKey, TItem> : XSingleton<TManager>, IConfigManager
	    where TManager : class, new()
	    where TItem : ITabItemWith1Key<TKey>, new()
	{
        protected Dictionary<int, TItem> m_ItemTable = new Dictionary<int, TItem>();

        public virtual Dictionary<int, TItem> ItemTable { get { return m_ItemTable; } }
	
	    public virtual bool Init(string text)
	    {
	        if (null == text)
	        {
	            return false;
	        }
            m_ItemTable.Clear();
            //if (null != m_ItemTable && m_ItemTable.Count > 0)
            //{
            //    return false;
            //}	
	        TabFile tf = new TabFile("", text);
	        while (tf.Next())
	        {
	            TItem item = new TItem();
	            if (item.ReadItem(tf) == false)
	            {
	                continue;
	            }
	            if (m_ItemTable.ContainsKey(ConfigHasher.GetHashCode(item.GetKey1())))
	            {
	                continue;
	            }
	            m_ItemTable.Add(ConfigHasher.GetHashCode(item.GetKey1()), item);
	        }
	        return true;
	    }

		public virtual bool Init(byte[] buffer)
		{
			if(buffer == null)
			{
				return false;
			}
			System.IO.MemoryStream stream = new System.IO.MemoryStream(buffer);
			var data = ProtoBuf.Serializer.Deserialize<ProtoArray<TItem>>(stream);
			if(data.Items == null)
			{
				return false;
			}
			int length = data.Items.Length;
			for (int i = 0; i < length; ++i)
			{
				var item = data.Items[i];
				int hash_value = item.Key;// ConfigHasher.GetHashCode(item.Key);
				if (m_ItemTable.ContainsKey(hash_value))
				{
					continue;
				}
				m_ItemTable.Add(hash_value, item.Item);
			}
			return true;
		}

		public virtual void serializeData(string file_name)
		{
			if(m_ItemTable.Count == 0)
			{
				return;
			}
			ProtoArray<TItem> data = new ProtoArray<TItem>();
			data.Items = new ProtoArray<TItem>.ArrayItem<TItem>[m_ItemTable.Count];
			int index = 0;
			foreach(var pair in m_ItemTable)
			{
				data.Items[index++] = 
					new ProtoArray<TItem>.ArrayItem<TItem> { Key = pair.Key, Item = pair.Value};
			}
			using (System.IO.Stream s =
				new System.IO.FileStream(file_name, System.IO.FileMode.Create, System.IO.FileAccess.ReadWrite))
			{
                UnityEngine.Debug.LogError(file_name);
				ProtoBuf.Serializer.Serialize<ProtoArray<TItem>>(s, data);
				s.Close();
			}
		}
	
	    public virtual TItem GetConfig(TKey key)
	    {
			return GetConfigByHashCode(ConfigHasher.GetHashCode(key));
	    }

		public virtual TItem GetConfig(int key)
		{
			return GetConfigByHashCode(key);
		}
	
		public virtual TItem GetConfigByHashCode(int hashCode)
		{
	        if (m_ItemTable.ContainsKey(hashCode))
	        {
	            return m_ItemTable[hashCode];
	        }
	        return default(TItem);			
		}
		
	    public virtual bool Has(TKey key)
	    {
	        return m_ItemTable.ContainsKey(ConfigHasher.GetHashCode(key));
	    }
	
	    public virtual TItem this[TKey key]
	    {
	        get { return GetConfig(key); }
	    }
	}

	[ProtoContract]
	public class ProtoArray<TItem>
	{
		[ProtoContract]
		public struct ArrayItem<TItem>
		{
			[ProtoMember(1)]
			public int Key { get; set; }
			[ProtoMember(2)]
			public TItem Item { get; set; }
		}

		[ProtoMember(1)]
		public ArrayItem<TItem>[] Items { get; set; }
	}
}
