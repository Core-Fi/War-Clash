using System;
using System.Collections.Generic;
using System.IO;
using ProtoBuf;

namespace Logic.TabConfig
{
	public abstract class CCfg2KeyMgrTemplate<TManager, TKey1, TKey2, TItem> : XSingleton<TManager>, IConfigManager
	    where TManager : class, new()
	    where TItem : ITabItemWith2Key<TKey1, TKey2>, new()
	{
	    protected Dictionary<int, Dictionary<int, TItem>> m_ItemTable = new Dictionary<int, Dictionary<int, TItem>>();
	
	    public virtual Dictionary<int, Dictionary<int, TItem>> ItemTable { get { return m_ItemTable; } }
	
	    public virtual bool Init(string text)
	    {
	        if (null == text)
	        {
	            return false;
	        }
            
            //if (null != m_ItemTable && m_ItemTable.Count > 0)
            //{
            //    return false;
            //}
            m_ItemTable.Clear();
	        TabFile tf = new TabFile("", text);
	        while (tf.Next())
	        {
	            TItem item = new TItem();
	            if (item.ReadItem(tf) == false)
	            {
	                return false;
	            }
				object key1 = item.GetKey1();
				int key1Hash = ConfigHasher.GetHashCode(key1);
				object key2 = item.GetKey2();
				int key2Hash = ConfigHasher.GetHashCode(key2);
				
	            if (GetGroup(item.GetKey1()) == null)
	            {
	                m_ItemTable.Add(key1Hash, new Dictionary<int, TItem>());
	            }
	            else if (m_ItemTable[key1Hash].ContainsKey(key2Hash))
	            {
	                return false;
	            }
	            m_ItemTable[key1Hash].Add(key2Hash, item);
	        }
	        return true;
	    }
		public virtual bool Init(byte[] buffer)
		{
			if (buffer == null)
			{
				return false;
			}
			System.IO.MemoryStream stream = new System.IO.MemoryStream(buffer);
			var data = ProtoBuf.Serializer.Deserialize<ProtoKey2Array<TItem>>(stream);
			if (data.Items == null)
			{
				return false;
			}
			int length = data.Items.Length;
			int hash_value = 0;
			for (int i = 0; i < length; ++i)
			{
				var node = data.Items[i];
				hash_value = node.Key;
				if(m_ItemTable.ContainsKey(hash_value))
				{
					continue;
				}
				int node_length = node.Item.Items.Length;
				var group_data = new Dictionary<int, TItem>();
				for(int j = 0; j < node_length; ++j)
				{
					var _innode = node.Item.Items[j];
					if(group_data.ContainsKey(_innode.Key))
					{
						continue;
					}
					group_data.Add(_innode.Key, _innode.Item);
				}
				m_ItemTable.Add(hash_value, group_data);
			}
			return true;
		}
		public virtual void serializeData(string file_name)
		{
			if (m_ItemTable.Count == 0)
			{
				return;
			}
			var data = new ProtoKey2Array<TItem>();
			data.Items = new ProtoKey2Array<TItem>.ArrayItem<TItem>[m_ItemTable.Count];
			int index = 0;
			foreach(var group in m_ItemTable)
			{
				var node = new ProtoArray<TItem>();
				node.Items = new ProtoArray<TItem>.ArrayItem<TItem>[group.Value.Count];
				int inIndex = 0;
				foreach(var group_node  in group.Value)
				{
					node.Items[inIndex++] = new ProtoArray<TItem>.ArrayItem<TItem>() 
					{ Key = group_node.Key, Item = group_node.Value };
				}
				data.Items[index++] = new ProtoKey2Array<TItem>.ArrayItem<TItem>() 
				{ Key = group.Key, Item = node };
			}
			using (System.IO.Stream s =
				new System.IO.FileStream(file_name, System.IO.FileMode.Create, System.IO.FileAccess.ReadWrite))
			{
				ProtoBuf.Serializer.Serialize<ProtoKey2Array<TItem>>(s, data);
				s.Close();
			}
		}
	
	    public virtual TItem GetConfig(TKey1 key1, TKey2 key2)
	    {
			return GetConfigByHashCode(ConfigHasher.GetHashCode(key1), ConfigHasher.GetHashCode(key2));
	    }

        public virtual TItem GetConfig(int key1, int key2)
        {
            return GetConfigByHashCode(key1, key2);
        }
	
		public virtual TItem GetConfigByHashCode(int key1Hash, int key2Hash)
		{
			if(m_ItemTable.ContainsKey(key1Hash) && m_ItemTable[key1Hash].ContainsKey(key2Hash))
			{
				return m_ItemTable[key1Hash][key2Hash];
			}
			return default(TItem);
		}
		
	    public virtual Dictionary<int, TItem> GetGroup(TKey1 key1)
	    {
			return GetGroupByHashCode(ConfigHasher.GetHashCode(key1));
	    }
		
		public virtual Dictionary<int, TItem> GetGroupByHashCode(int key1Hash)
		{
			if(m_ItemTable.ContainsKey(key1Hash))
			{
				return m_ItemTable[key1Hash];
			}
			return null;
		}
	
	    public virtual bool Has(TKey1 key1, TKey2 key2)
	    {
			int key1Hash = ConfigHasher.GetHashCode(key1);
			int key2Hash = ConfigHasher.GetHashCode(key2);
	        return m_ItemTable.ContainsKey(key1Hash) && m_ItemTable[key1Hash].ContainsKey(key2Hash);
	    }
	
	    public virtual Dictionary<int, TItem> this[TKey1 key1]
	    {
	        get { return GetGroup(key1); }
	    }
	
	    public virtual TItem this[TKey1 key1, TKey2 key2]
	    {
	        get { return GetConfig(key1, key2); }
	    }
	}

	[ProtoContract]
	public class ProtoKey2Array<TItem>
	{
		[ProtoContract]
		public struct ArrayItem<TItem>
		{
			[ProtoMember(1)]
			public int Key { get; set; }
			[ProtoMember(2)]
			public ProtoArray<TItem> Item { get; set; }
		}

		[ProtoMember(1)]
		public ArrayItem<TItem>[] Items { get; set; }
	}
}
