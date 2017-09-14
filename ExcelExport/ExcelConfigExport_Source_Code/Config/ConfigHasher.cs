using System;
using System.Collections;
using System.Collections.Generic;
using Logic;

namespace Logic.TabConfig
{
	public static class ConfigHasher
	{
		public static int GetHashCode(object val)
		{
			if(val is string)
			{
				string str = val as string;
				uint[] magic = new uint[] {113, 127, 131, 137};
				char[] cha = str.ToCharArray();
				uint num = 0;
				int i = 0;
				for(int k=0; k<cha.Length; k++)
				{
					uint magicNum = magic[i++];
					if(i >= 4) i=0;
					num += magicNum * (byte)(cha[k]);
				}
				return (int)(num & 0x7fffffff);
			}
			else
			{
		        try
		        {
					uint num = (uint)Convert.ChangeType(val, typeof(uint));
					uint magic = 199;
					num *= magic;
					return (int)num & 0x7fffffff;
		        }
	        	catch (Exception)
	        	{
	        	}
			}
			return 0;
		}
	}
}