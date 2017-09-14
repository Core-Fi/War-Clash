using System;
using System.Collections;

namespace Logic.TabConfig
{
	public class TabFile
	{
		private string 	FileName;
	    private string[] Title;
	    private ArrayList Body;
	    public int CurrentLine { get; private set; }
		
		public TabFile(string strFileName, string strContent)
		{
			FileName = strFileName;
			string[] content = strContent.Split(new string[]{"\r\n"}, StringSplitOptions.RemoveEmptyEntries);
			Title = content[0].Split(new char[]{'\t'});
			
			Body = new ArrayList();
			for(int i=1; i<content.Length; i++)
			{
				string[] line = content[i].Split(new char[]{'\t'});
				Body.Add(line);
			}
			Body.TrimToSize();
			Begin();
		}
		
		public void Begin()
		{
			CurrentLine = -1;
		}
		
		public bool Next()
		{
			CurrentLine++;
			if(CurrentLine >= Body.Count)
				return false;
			return true;
		}
	
	    public T Get<T>(string strColName)
	    {
			// temp: show tabfile error
			int n = Array.IndexOf(Title, strColName);
			if(-1 == n)
			{
				UnityEngine.DLog.LogError("tabfile error:  " + strColName);
			}
			return this.Get<T>(n);
	    }
	
	    public T Get<T>(int nColIndex)
	    {
	        string strValue = this.getValueString(nColIndex);
	        Type t = typeof(T);
			// temp: 
			if(null == strValue)
			{
				
			}
			else
			{
				return (T)Convert.ChangeType(strValue, t);
			}
			return default(T);
	    }
	
	    private string getValueString(int nColIndex)
	    {
			string[] line = (string[])Body[CurrentLine];
			if(nColIndex < 0 || nColIndex >= line.Length)
			{
				return null;
			}
			return line[nColIndex];
	    }
	}
}