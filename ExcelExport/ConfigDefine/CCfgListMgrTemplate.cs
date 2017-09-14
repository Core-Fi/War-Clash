using System;
using System.Collections.Generic;
using System.IO;
using ProtoBuf;

namespace Logic.TabConfig
{
	public abstract class CCfgListMgrTemplate<TManager, TItem> : XSingleton<TManager>, IConfigManager
	    where TManager : class, new()
	    where TItem : ITabItem, new()
	{
	    protected List<TItem> m_ItemTable = new List<TItem>();
	
	    public virtual List<TItem> ItemTable { get { return m_ItemTable; } }
	
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
	                continue;
	            }
	            m_ItemTable.Add(item);
	        }
	        return true;
	    }
		public virtual bool Init(byte[] buffer)
		{
			if(buffer == null || buffer.Length == 0)
			{
				return false;
			}
			System.IO.MemoryStream stream = new System.IO.MemoryStream(buffer);
			var data = ProtoBuf.Serializer.Deserialize<List<TItem>>(stream);
			if (data == null)
			{
				return false;
			}
			m_ItemTable = data;
			return true;
		}
		public virtual void serializeData(string file_name)
		{
			if (m_ItemTable.Count == 0)
			{
				return;
			}
			using (System.IO.Stream s =
				new System.IO.FileStream(file_name, System.IO.FileMode.Create, System.IO.FileAccess.ReadWrite))
			{
				ProtoBuf.Serializer.Serialize<List<TItem>>(s, m_ItemTable);
				s.Close();
			}
		}
	}
}
