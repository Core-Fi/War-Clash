using System;

namespace Logic.TabConfig
{
	public interface ITabItem
	{
	    bool ReadItem(TabFile tf);
	}
	
	public interface ITabItemWith1Key<TKey1> : ITabItem
	{
	    TKey1 GetKey1();
	}
	
	public interface ITabItemWith2Key<TKey1, TKey2> : ITabItemWith1Key<TKey1>
	{
	    TKey2 GetKey2();
	}
	
	public interface IConfigManager
	{
	    bool Init(string text);
		bool Init(byte[] buffer);
		void serializeData(string file_name);
	}
}