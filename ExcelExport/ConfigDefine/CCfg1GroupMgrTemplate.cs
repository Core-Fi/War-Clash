using System;
using System.Collections.Generic;
using System.IO;
using ProtoBuf;

namespace Logic.TabConfig
{
	public abstract class CCfg1GroupMgrTemplate<TManager, TKey1, TItem> : XSingleton<TManager>, IConfigManager
	    where TManager : class, new()
	    where TItem : ITabItemWith1Key<TKey1>, new()
	{
	    protected Dictionary<int, List<TItem>> m_ItemTable = new Dictionary<int, List<TItem>>();
	
	    public virtual Dictionary<int, List<TItem>> ItemTable { get { return m_ItemTable; } }
	
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
				int key1Hash = ConfigHasher.GetHashCode(item.GetKey1());
	            if (GetGroup(item.GetKey1()) == null)
	            {
	                m_ItemTable.Add(key1Hash, new List<TItem>());
	            }
	            m_ItemTable[key1Hash].Add(item);
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
			var data = ProtoBuf.Serializer.Deserialize<ProtoGroupArray<TItem>>(stream);
			if (data.Items == null)
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
			if (m_ItemTable.Count == 0)
			{
				return;
			}
			ProtoGroupArray<TItem> data = new ProtoGroupArray<TItem>();
			data.Items = new ProtoGroupArray<TItem>.ArrayItem<TItem>[m_ItemTable.Count];
			int index = 0;
			foreach (var pair in m_ItemTable)
			{
				data.Items[index++] =
					new ProtoGroupArray<TItem>.ArrayItem<TItem> { Key = pair.Key, Item = pair.Value };
			}
			using (System.IO.Stream s =
				new System.IO.FileStream(file_name, System.IO.FileMode.Create, System.IO.FileAccess.ReadWrite))
			{
				ProtoBuf.Serializer.Serialize<ProtoGroupArray<TItem>>(s, data);
				s.Close();
			}
		}
	
	    public virtual List<TItem> GetGroup(TKey1 key1)
	    {
			return GetGroupByHashCode(ConfigHasher.GetHashCode(key1));
	    }
		
		public virtual List<TItem> GetGroupByHashCode(int keyHash)
		{
			if(m_ItemTable.ContainsKey(keyHash))
				return m_ItemTable[keyHash];
			return null;
		}
	
	    public virtual List<TItem> this[TKey1 key1]
	    {
	        get { return GetGroup(key1); }
	    }
	}

	[ProtoContract]
	public class ProtoGroupArray<TItem>
	{
		[ProtoContract]
		public struct ArrayItem<TItem>
		{
			[ProtoMember(1)]
			public int Key { get; set; }
			[ProtoMember(2)]
			public List<TItem> Item { get; set; }
		}

		[ProtoMember(1)]
		public ArrayItem<TItem>[] Items { get; set; }
	}
}