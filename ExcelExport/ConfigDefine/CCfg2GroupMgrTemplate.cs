using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Logic.TabConfig
{
    public abstract class CCfg2GroupMgrTemplate<TManager, TKey1, TKey2,TItem> : XSingleton<TManager>, IConfigManager
            where TManager : class, new()
        where TItem : ITabItemWith2Key<TKey1, TKey2>, new()
        {
        protected Dictionary<int, Dictionary<int, List<TItem>>> m_ItemTable = new Dictionary<int, Dictionary<int, List<TItem>>>();

        public virtual Dictionary<int, Dictionary<int, List<TItem>>>  ItemTable { get { return m_ItemTable; } }
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


                    if (GetGroup(item.GetKey1(), item.GetKey2()) == null)
                    {
                        if (m_ItemTable.ContainsKey(key1Hash))
                        {
                            m_ItemTable[key1Hash].Add(key2Hash, new List<TItem>());
                        }
                        else
                        {
                            Dictionary<int, List<TItem>> ls = new Dictionary<int, List<TItem>>();
                            ls.Add(key2Hash, new List<TItem>());
                            m_ItemTable.Add(key1Hash, ls);
                        }
                    }
                    m_ItemTable[key1Hash][key2Hash].Add(item);
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
				var data = ProtoBuf.Serializer.Deserialize<ProtoGroupKey2Array<TItem>>(stream);
				if (data.Items == null)
				{
					return false;
				}
				int length = data.Items.Length;
				for (int i = 0; i < length; ++i)
				{
					var item = data.Items[i];
					if (m_ItemTable.ContainsKey(item.Key))
					{
						continue;
					}
					Dictionary<int, List<TItem>> node = new Dictionary<int, List<TItem>>();
					int node_length = item.Item.Items.Length;
					for(int j = 0; j < length; ++j)
					{
						int hash_value = item.Item.Items[j].Key;
						if(node.ContainsKey(hash_value))
						{
							continue;
						}
						node.Add(hash_value, item.Item.Items[j].Item);
					}
					m_ItemTable.Add(item.Key, node);
				}
				return true;
			}
			public virtual void serializeData(string file_name)
			{
				if (m_ItemTable.Count == 0)
				{
					return;
				}
				var data = new ProtoGroupKey2Array<TItem>();
				data.Items = new ProtoGroupKey2Array<TItem>.ArrayItem<TItem>[m_ItemTable.Count];
				int index = 0;
				foreach (var group in m_ItemTable)
				{
					var node = new ProtoGroupArray<TItem>();
					node.Items = new ProtoGroupArray<TItem>.ArrayItem<TItem>[group.Value.Count];
					int inIndex = 0;
					foreach (var group_node in group.Value)
					{
						node.Items[inIndex++] = new ProtoGroupArray<TItem>.ArrayItem<TItem>() 
						{ Key = group_node.Key, Item = group_node.Value };
					}
					data.Items[index++] = new ProtoGroupKey2Array<TItem>.ArrayItem<TItem>() 
					{ Key = group.Key, Item = node };
				}
				using (System.IO.Stream s =
					new System.IO.FileStream(file_name, System.IO.FileMode.Create, System.IO.FileAccess.ReadWrite))
				{
					ProtoBuf.Serializer.Serialize<ProtoGroupKey2Array<TItem>>(s, data);
					s.Close();
				}
			}

            public virtual List<TItem> GetGroup(TKey1 key1, TKey2 key2)
            {
                return GetGroupByHashCode(ConfigHasher.GetHashCode(key1), ConfigHasher.GetHashCode(key2));
            }

            public virtual List<TItem> GetGroupByHashCode(int k1,int k2)
            {
                if (m_ItemTable.ContainsKey(k1))
                {
                    if(m_ItemTable[k1].ContainsKey(k2))
                    {
                        return m_ItemTable[k1][k2];
                    }
                }
                return null;
            }

            public virtual List<TItem> this[ TKey1 key1, TKey2 key2]
            {
                get { return GetGroup(key1,key2); }
            }
     }

	[ProtoContract]
	public class ProtoGroupKey2Array<TItem>
	{
		[ProtoContract]
		public struct ArrayItem<TItem>
		{
			[ProtoMember(1)]
			public int Key { get; set; }
			[ProtoMember(2)]
			public ProtoGroupArray<TItem> Item { get; set; }
		}

		[ProtoMember(1)]
		public ArrayItem<TItem>[] Items { get; set; }
	}
}
