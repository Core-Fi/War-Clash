using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Logic.TabConfig;

namespace ExcelConfigExport
{
    class XConfigData
    {
        #region static

        public static XConfigData CreateConfigData(object[,] head, LogEventHandler handler)
        {
            XConfigData cfgData = new XConfigData();
            cfgData.LogEvent += handler;
            if (null == head || cfgData.Init(head) == false)
            {
                return null;
            }
            return cfgData;
        }

        public static Hashtable DataTypeMap { get; private set; }
        public static Hashtable ShareTypeMap { get; private set; }

        static XConfigData()
        {
            DataTypeMap = new Hashtable();
            foreach (XConfigDataType dt in XConfigDefine.ALL_DATA_TYPE)
            {
                DataTypeMap.Add(dt.ConfigName, dt);
            }

            ShareTypeMap = new Hashtable();
            foreach (XConfigShareType share in XConfigDefine.ALL_SHARE_TYPE)
            {
                ShareTypeMap.Add(share.Name, share);
            }
        }

        private static XConfigDataType GetDataType(object s)
        {
            if (null == s)
            {
                return null;
            }
            if (DataTypeMap.Contains(s.ToString().Trim()))
            {
                return DataTypeMap[s] as XConfigDataType;
            }
            return null;
        }

        private static XConfigShareType GetShareType(object s)
        {
            if (null == s)
            {
                return null;
            }
            if (ShareTypeMap.Contains(s.ToString().Trim()))
            {
                return ShareTypeMap[s] as XConfigShareType;
            }
            return null;
        }

        #endregion

        #region Members

        public event LogEventHandler LogEvent;

        private XColumnInfo[] AllColumnInfo;
		private List<int> exportColClientList = null;
		private List<int> exportColServerList = null;

        private int ColumnNumber
        {
            get {
                return AllColumnInfo == null ? 0 : AllColumnInfo.Length;
            }
        }

        private List<object[]> m_LoadedData;

        #endregion

        #region Constructor and Init

        private XConfigData()
        {
            AllColumnInfo = null;
            m_LoadedData = new List<object[]>();
        }

        private bool Init(object[,] head)
        {
            if (null == head)
            {
                Log(ELogType.ERROR, "数据表表头为空");
                return false;
            }
            if (head.GetLength(0) < (int)EConfigHeadType.Count)
            {
                Log(ELogType.ERROR, "数据表表头行数不正确, 需求行数:{0}, 实际行数:{1}", (int)EConfigHeadType.Count, head.GetLength(0));
                return false;
            }

            AllColumnInfo = new XColumnInfo[head.GetLength(1)];
            int rowBase = head.GetLowerBound(0);
            int colBase = head.GetLowerBound(1);
            for (int col = 0; col < ColumnNumber; ++col)
            {
                if (SetColumnInfo(col,
                    head[rowBase + (int)EConfigHeadType.Desc,       col + colBase],
                    head[rowBase + (int)EConfigHeadType.CodeName,   col + colBase],
                    head[rowBase + (int)EConfigHeadType.DataType,   col + colBase],
                    head[rowBase + (int)EConfigHeadType.ShareType, col + colBase]) == false)
                {
                    Log(ELogType.ERROR, "获取数据表头信息失败, 列:{0}", col);
                    return false;
                }
            }
            return true;
        }

        #endregion

        #region Logical Interface

        public void ClearData()
        {
            m_LoadedData.Clear();
        }

		public bool CheckMultiLine(object[,] multiLine, int addRowBegin, ECfgMgrType csMgrType, ECfgMgrType cppMgrType)
        {
            return doAddMultiLine(multiLine, addRowBegin, false, csMgrType, cppMgrType);
        }

		public bool AddMultiLine(object[,] multiLine, int addRowBegin, ECfgMgrType csMgrType, ECfgMgrType cppMgrType)
        {
            return doAddMultiLine(multiLine, addRowBegin, true, csMgrType, cppMgrType);
        }

        public bool ExportConfigFile(XCfgExportInfo cfgInfo)
        {
            if (null == cfgInfo)
            {
                return false;
            }
            if (doExportConfig(EConfigShareFlag.Client, cfgInfo.ClientConfig) == false)
            {
                return false;
            }
            if (doExportConfig(EConfigShareFlag.Server, cfgInfo.ServerConfig) == false)
            {
                return false;
            }
            // -------------------------
            string[] path = cfgInfo.ClientConfig.Split('\\');
            string langpath = "";
            for (int i = 0; i < path.Length - 1; i++)
            {
                langpath += path[i] + "\\";
            }
            langpath += "Lang\\zh-cn\\" + path[path.Length - 1];
            if (doExportConfig(EConfigShareFlag.Client, langpath) == false)
            {
                return false;
            }
            // -------------------------
            return true;
        }

        public bool ExportSourceFile(XSrcExportInfo srcInfo)
        {
            if (null == srcInfo)
            {
                return false;
            }
            if (doExportSource(ECodeLanguageType.CS, srcInfo) == false)
            {
                return false;
            }
            if (doExportSource(ECodeLanguageType.CPP, srcInfo) == false)
            {
                return false;
            }
            return true;
        }

        #endregion

        #region Excel 表格导入

        private bool SetColumnInfo(int col, object desc, object name, object dt, object share)
        {
            if (col < 0 || col >= ColumnNumber)
            {
                return false;
            }
			string sDesc = null;
			string sName = null;
			XConfigShareType shareType = null;
			XConfigDataType dataType = null;
			string defName = string.Empty;
			int arrSize = 0;
			int arrIdx = -1;

			if(!XConfigDefine.IsSkipCodeName(name))
			{
				sName = name.ToString();

				// 检测名称
				if (XConfigDefine.IsValidCodeName(sName) == false)
				{
					Log(ELogType.ERROR, "数据表的列名(变量名)不符合要求, 列:{0}, 列名:{1}, 描述:{2}", col, name, desc);
					return false;
				}

				// 检测名称数组定义
				Match m = XConfigDefine.VAR_ARRAY_NAME_REGEX.Match(sName);
				if (m.Success)
				{
					defName = m.Groups[1].ToString();
					arrSize = int.Parse(m.Groups[2].ToString());
					arrIdx = int.Parse(m.Groups[3].ToString());
					if (arrSize <= arrIdx)
					{
						Log(ELogType.ERROR, "数据表的列名(数组定义)不符合要求, 列:{0}, 列名:{1}, 描述:{2}", col, name, desc);
						return false;
					}
				}
				else
				{
					defName = sName;
				}

				// 字段重复检测, ps: 复杂度高, 鉴于是工具不深究
				for (int i = 0; i < col; i++)
				{
					XColumnInfo columInfo = AllColumnInfo[i];
					if (null != columInfo && columInfo.ConfigName == sName)
					{
						Log(ELogType.ERROR, "数据表的列名重复, 列: {0}, {1}, 列名: {2}", i, col, defName);
						return false;
					}
				}

				// 检测共享类型
				shareType = GetShareType(share);
				if (null == shareType)
				{
					Log(ELogType.ERROR, "数据表的共享类型定义不正确, 列:{0}, 列名:{1}, 共享类型:{2}, 描述:{3}", col, name, share, desc);
					return false;
				}

				// 检测数据类型
				dataType = GetDataType(dt);
				if (null == dataType)
				{
					Log(ELogType.ERROR, "数据表的数据类型定义不正确, 列:{0}, 列名:{1}, 数据类型:{2}, 描述:{3}", col, name, dt, desc);
					return false;
				}

				// Description
				sDesc = (null != desc) ? desc.ToString() : string.Empty;
			}

			AllColumnInfo[col] = XColumnInfo.CreateColumnInfo(sDesc, sName, dataType, shareType, defName, arrIdx, arrSize);
            return true;
        }

        private object GetColumnData(int col, object data)
        {
            if (col < 0 || col >= ColumnNumber)
            {
                return string.Empty;
            }
            XColumnInfo info = AllColumnInfo[col];
            if (null == info || !info.IsValid)
            {
				return string.Empty;
            }
			object ret = info.DataType.GetData(data);
			if (null == ret)
			{
				Log(ELogType.ERROR, "加载数据行错误, 数据值和列定义不相符, 列:{0} 列名:{1} 数据值:{2}", col, info.ConfigName, null != data ? data.ToString() : string.Empty);
				return null;
			}
			return ret;
        }

		private bool checkKey_Head(ECfgMgrType mgrType, List<int> exportList)
		{
			switch (mgrType)
			{
				case(ECfgMgrType.NoKeyList):
					return true;

				case (ECfgMgrType.OneKeyMgr):
				case (ECfgMgrType.OneGroupMgr):
					{
						if (!AllColumnInfo[exportList[0]].DataType.CanBeKey)
						{
							Log(ELogType.ERROR, "第一列的数据类型不可以作为Key");
							return false;
						}
					}
					break;

				case (ECfgMgrType.TwoKeyMgr):
					if (exportList.Count < 2)
					{
						Log(ELogType.ERROR, "总列数不足以生成TwoKeyMgr, 当前列数: {0}, 需求列数: {1}", AllColumnInfo.Length, 2);
						return false;
					}
					if (!AllColumnInfo[exportList[0]].DataType.CanBeKey)
					{
						Log(ELogType.ERROR, "第一列的数据类型不可以作为Key");
						return false;
					}
					if(!AllColumnInfo[exportList[1]].DataType.CanBeKey)
					{
						Log(ELogType.ERROR, "第二列的数据类型不可以作为Key");
						return false;
					}
					break;
			}
			return true;
		}

		private bool checkKey_Data(int lineBase, int curLineNum, object[] curLine, List<object[]> allLine, List<int> exportList, ECfgMgrType mgrType)
		{
			if (mgrType == ECfgMgrType.NoKeyList) 
				return true;

			switch (mgrType)
			{
				case(ECfgMgrType.NoKeyList):
					return true;
				case (ECfgMgrType.OneKeyMgr):
					{
						object key1 = curLine[exportList[0]];
						int hash1 = ConfigHasher.GetHashCode(key1);
						for (int i = 0; i < allLine.Count; i++)
						{
							object keyO1 = allLine[i][exportList[0]];
							int hashO1 = ConfigHasher.GetHashCode(keyO1);
							if (hash1 == hashO1)
							{
								Log(ELogType.ERROR, @"第 {0} 行的Key : {1} (HashCode:{2}) 和 第 {3} 行的Key: {4} (HashCode:{5}) 发生重复",
								  i + lineBase, keyO1, hashO1, curLineNum, key1, hash1);
								return false;
							}

						}
					}
					break;

				case (ECfgMgrType.TwoKeyMgr):
					{
						object key1 = curLine[exportList[0]];
						object key2 = curLine[exportList[1]];
						int hash1 = ConfigHasher.GetHashCode(key1);
						int hash2 = ConfigHasher.GetHashCode(key2);
						for (int i = 0; i < allLine.Count; i++)
						{
							object keyO1 = allLine[i][exportList[0]];
							object keyO2 = allLine[i][exportList[1]];
							int hashO1 = ConfigHasher.GetHashCode(keyO1);
							int hashO2 = ConfigHasher.GetHashCode(keyO2);
							if (hash1 == hashO1 && hash2 == hashO2)
							{
								Log(ELogType.ERROR, @"第 {0} 行的Key1: {1} (HashCode:{2}), 
Key2: {3} (HashCode:{4}) 和 第 {5} 行的Key1: {6} (HashCode:{7}), Key2: {8} (HashCode:{9}) 发生重复",
i + lineBase, keyO1, hashO1, keyO2, hashO2, curLineNum, key1, hash1, key2, hash2);
								return false;
							}
						}
					}
					break;

				case (ECfgMgrType.OneGroupMgr):
					{
						object key1 = curLine[exportList[0]];
						int hash1 = ConfigHasher.GetHashCode(key1);
						for (int i = 0; i < allLine.Count; i++)
						{
							object keyO1 = allLine[i][exportList[0]];
							if(!key1.Equals(keyO1))
							{
								int hashO1 = ConfigHasher.GetHashCode(keyO1);
								if (hash1 == hashO1)
								{
									Log(ELogType.ERROR, @"第 {0} 行的Key : {1} (HashCode:{2}) 和 第 {3} 行的Key: {4} (HashCode{5}) 发生重复",
					i + lineBase, keyO1, hashO1, curLineNum, key1, hash1);
									return false;
								}
							}
						}
					}
					break;
			}
			return true;
		}

	    private bool doAddMultiLine(object[,] multiLine, int addRowBegin, bool bAddData, ECfgMgrType csMgrType, ECfgMgrType cppMgrType)
        {
			// stupid method: 分别提取要导出的列信息, 不应该放在这里
			exportColClientList = getExportColumnList(EConfigShareFlag.Client);
			exportColServerList = getExportColumnList(EConfigShareFlag.Server);

			if (!checkKey_Head(csMgrType, exportColClientList))
			{
				return false;
			}
			if(!checkKey_Head(cppMgrType, exportColServerList))
			{
				return false;
			}

            if (null == multiLine)
            {
                Log(ELogType.ERROR, "加载数据行错误, 空行");
                return false;
            }
            Log(ELogType.INFO, "--4> 开始加载数据");

            if (addRowBegin < multiLine.GetLowerBound(0))
            {
                Log(ELogType.ERROR, "读取数据失败, 数据下标范围错误");
                return false;
            }
            if (multiLine.GetLength(1) != ColumnNumber)
            {
                Log(ELogType.ERROR, "加载数据行错误, 数据项和表的列数不一致, 需求列数:{0}, 实际列数:{1}", ColumnNumber, multiLine.GetLength(1));
                return false;
            }
            int addRowLast = multiLine.GetUpperBound(0);
            int colBase = multiLine.GetLowerBound(1);

			List<int> exportColBothList = getExportColumnList(EConfigShareFlag.Both);

            List<object[]> allDataLine = new List<object[]>();
            for (int row = addRowBegin; row <= addRowLast; ++row)
            {
				// 在第一个有效列处判断当前行是否要忽略
				object cell = multiLine[row, exportColBothList[0] + colBase];
				if (XConfigDefine.IsSkipCodeName(cell))
					continue;

				// 提取一行数据
                object[] dataLine = new object[ColumnNumber];
                for (int col = 0; col < ColumnNumber; ++col)
                {
					// 单元格数据与表头是否匹配
                    dataLine[col] = GetColumnData(col, multiLine[row, col + colBase]);
                    if (null == dataLine[col])
                    {
                        Log(ELogType.ERROR, "加载第 {0} 行数据失败", row);
                        return false;
                    }
                }

				// 单条数据重复性验证, ps: stupid method 复杂度高, 鉴于是工具, 不深究, 代码结构也不好. 就这样吧, 反正是工具
				if(!checkKey_Data(addRowBegin, row, dataLine, allDataLine, exportColClientList, csMgrType))
				{
					return false;
				}
				if (!checkKey_Data(addRowBegin, row, dataLine, allDataLine, exportColServerList, cppMgrType))
				{
					return false;
				}
                allDataLine.Add(dataLine);
            }
            if (bAddData)
            {
                m_LoadedData.AddRange(allDataLine);
                Log(ELogType.INFO, "成功加载 {0} 行数据, 加载的有效数据总行数:{1}", allDataLine.Count, m_LoadedData.Count);
            }

            Log(ELogType.INFO, "--4> 成功加载数据");
            return true;
        }

        #endregion

        #region 导出txt配置文件

        private bool doExportConfig(EConfigShareFlag flag, string sFileName)
        {
            Log(ELogType.INFO, "--4> 开始导出 {0} 配置文件, 文件名:{1}", flag.ToString(), sFileName);

            // 如果没有相关配置列导出, 不创建目标文件
            List<int> exColList = getExportColumnList(flag);
            if (exColList.Count > 0)
            {
                if (createFile(sFileName) == false)
                {
                    return false;
                }
                try
                {
					UTF8Encoding encoding = (EConfigShareFlag.Server == flag) ? XConfigDefine.UTF8_WITHBOM : XConfigDefine.UTF8_WITHOUTBOM;
					using (StreamWriter fs = new StreamWriter(sFileName, false, encoding))
                    {

                        string oLine = string.Empty;
                        
                        // 写表头
                        for (int i = 0; i < exColList.Count; ++i)
                        {
                            if (i > 0)
                            {
                                oLine += XConfigDefine.SPLIT_STRING;
                            }
                            oLine += AllColumnInfo[exColList[i]].ConfigName;
                        }
                        fs.WriteLine(oLine);

                        // 写数据
                        foreach (object[] line in m_LoadedData)
                        {
                            //--4>TODO: 可以改成 string.Join() 来实现
                            oLine = string.Empty;
                            for (int i = 0; i < exColList.Count; ++i)
                            {
                                if (i > 0)
                                {
                                    oLine += XConfigDefine.SPLIT_STRING;
                                }
                                oLine += line[exColList[i]].ToString();
                            }
                            fs.WriteLine(oLine);
                        }
                        fs.Close();
                    }
                }
                catch (System.Exception ex)
                {
                    Log(ELogType.ERROR, "导出 {0} 配置文件失败, 文件名:{1}, 错误:{2}", flag.ToString(), sFileName, ex.ToString());
                    return false;
                }
            }
            Log(ELogType.INFO, "--4> 成功导出 {0} 配置文件, 文件名:{1}, 数据行数:{2}", flag.ToString(), sFileName, m_LoadedData.Count);
            return true;
        }

        #endregion

        #region 导出源代码

        private bool doExportSource(ECodeLanguageType e, XSrcExportInfo cfgSrc)
        {
            string sFileName = cfgSrc.GetSourceName(e);
            Log(ELogType.INFO, "--4> 开始导出 {0} 代码文件, 文件名:{1}", e.ToString(), sFileName);

            bool bRet = false;

            // 如果没有相关配置列导出, 不创建目标文件
            List<int> exColList = getExportColumnList(e == ECodeLanguageType.CPP ? EConfigShareFlag.Server : EConfigShareFlag.Client);
            if (null != exColList && exColList.Count > 0)
            {
                switch (e)
                {
                    case ECodeLanguageType.CS:
                        bRet = doExportCSharpSource(cfgSrc.CsInfo);
                        break;
                    case ECodeLanguageType.CPP:
                        bRet = doExportCppSource(cfgSrc.CppInfo);
                        break;
                    default:
                        bRet = false;
                        break;
                }
            }
            else
            {
                bRet = true;
            }
            if (bRet)
            {
                Log(ELogType.INFO, "--4> 成功导出 {0} 代码文件, 文件名:{1}", e.ToString(), sFileName);
            }
            else
            {
                Log(ELogType.ERROR, "--4> 导出 {0} 代码文件失败, 文件名:{1}", e.ToString(), sFileName);
            }
            return bRet;
        }

        #region 导出 CSharp 代码

        private bool doExportCSharpSource(XCsSrcInfo cfgSrc)
        {
            string sClassName = cfgSrc.ClassName;
            string sFileName = cfgSrc.FilePath;

            if (XConfigDefine.IsValidCodeName(sClassName) == false)
            {
                Log(ELogType.ERROR, "导出 C# 代码文件失败, 类名不符合规则, 文件名:{0}, 类名:{1}", sFileName, sClassName);
                return false;
            }

            if (createFile(sFileName) == false)
            {
                return false;
            }

            try
            {
                using (StreamWriter fs = new StreamWriter(sFileName, false, XConfigDefine.UTF8_WITHOUTBOM))
                {
                    List<int> exColList = getExportColumnList(EConfigShareFlag.Client);

                    // 头部数据
                    writeCode(fs, 0, XConfigDefine.SOURCE_FILE_HEAD);
                    fs.WriteLine();
                    writeCode(fs, 0, XConfigDefine.CSHARP_INCLUDE);
                    fs.WriteLine();
					writeCode(fs, 0, XConfigDefine.CSHARP_NAME_SPACE);
					writeCode(fs, 0, "{");

                    // 管理器定义
                    writeCsMgrDef(fs, cfgSrc);

                    // 类定义
                    writeCsClassDef(fs, cfgSrc);
                    writeCode(fs, 1, "{");

                    // 导出列的字符串索引
                    foreach (int col in exColList)
                    {
                        XColumnInfo info = AllColumnInfo[col];
                        writeCode(fs, 2, "public static readonly string {0}{1} = \"{2}\";",
                            XConfigDefine.CSHARP_KEY_PREFIX, info.ConfigName, info.ConfigName);
                    }
                    fs.WriteLine();

                    // 成员变量定义
					int protoIndex = 1;
                    foreach (int col in exColList)
                    {
                        XColumnInfo info = AllColumnInfo[col];
                        // 数组只有下标为 0 的需要定义
                        if (false == info.IsArray || 0 == info.ArrayIndex)
                        {
							string arrayTag = "";
							if (info.IsArray)
							{
								arrayTag = ", OverwriteList=true";
							}
							writeCode(fs, 2, "[ProtoMember({0}{1})]", protoIndex++, arrayTag);
                            writeCode(fs, 2, "public {0} {{ get; private set; }}\t\t\t\t// {1}", info.GetVarDefineStr(ECodeLanguageType.CS), info.ColDesc.Replace('\r', ' ').Replace('\n', ' '));
                        }
                    }
                    fs.WriteLine();

                    // 构造函数定义(用于初始化数组变量)
                    writeCode(fs, 2, "public {0}()", sClassName);
                    writeCode(fs, 2, "{");
                    foreach (int col in exColList)
                    {
                        XColumnInfo info = AllColumnInfo[col];
                        if (info.IsArray && 0 == info.ArrayIndex)
                        {
                            writeCode(fs, 3, "{0} = new {1}[{2}];", info.DefName, info.DataType.CsName, info.ArraySize);
                        }
                    }
                    writeCode(fs, 2, "}");
                    fs.WriteLine();

                    writeCsKeyInterface(fs, cfgSrc);

                    // 数据读取函数
                    writeCode(fs, 2, XConfigDefine.CSHARP_READ_METHOD);
                    writeCode(fs, 2, "{");
                    foreach (int col in exColList)
                    {
                        XColumnInfo info = AllColumnInfo[col];
                        if (info.DataType is XConfigDataType_Normal)
                        {
                            writeCode(fs, 3, "{0} = {1}.Get<{2}>({3}{4});", info.VarName, XConfigDefine.CSHARP_READ_READER,
                                info.DataType.CsName, XConfigDefine.CSHARP_KEY_PREFIX, info.ConfigName);
                        }
                        else if (info.DataType is XConfigDataType_Special)
                        {
                            writeCode(fs, 3, "{0} = {5}({1}.Get<string>({3}{4}));", info.VarName, XConfigDefine.CSHARP_READ_READER,
                                info.DataType.CsName, XConfigDefine.CSHARP_KEY_PREFIX, info.ConfigName, (info.DataType as XConfigDataType_Special).Reader_CS);
                        }
                    }

                    // 函数末尾
                    writeCode(fs, 3, "return true;");
                    writeCode(fs, 2, "}");

                    writeCode(fs, 1, "}");
					writeCode(fs, 0, "}");
                    fs.WriteLine();
                    fs.Close();
                }
            }
            catch (System.Exception ex)
            {
                Log(ELogType.ERROR, "导出 c# 代码文件失败, 文件名:{0}, 类名:{1}, 错误:{2}", sFileName, sClassName, ex.ToString());
                return false;
            }
            return true;
        }

        // 管理器类定义
        private void writeCsMgrDef(StreamWriter fs, XCsSrcInfo cfgSrc)
        {
            string sClassName = cfgSrc.ClassName;
            string sMgrName = sClassName + XConfigDefine.MANAGER_CLASS_SUFFIX;
            switch (cfgSrc.MgrType.MgrEnum)
            {
                case ECfgMgrType.NoKeyList:
                    writeCode(fs, 1, "public class {0} : {1}<{0}, {2}> {{ }};", sMgrName, cfgSrc.MgrType.CsMgrBaseName, sClassName);
                    fs.WriteLine();
                    break;

                case ECfgMgrType.OneKeyMgr:
                    writeCode(fs, 1, "public class {0} : {1}<{0}, {2}, {3}>", 
						sMgrName, cfgSrc.MgrType.CsMgrBaseName, AllColumnInfo[exportColClientList[0]].DataType.CsName, sClassName);
					writeCode(fs, 1, "{");
					writeCode(fs, 0, "#pragma warning disable");
					writeCode(fs, 2, XConfigDefine.CSHARP_AOT_FUNC_DEF);
					writeCode(fs, 2, "{");
					writeCode(fs, 3, "Dictionary<int, {0}> _un = new Dictionary<int, {0}>();", sClassName);
					writeCode(fs, 2, "}");
					writeCode(fs, 0, "#pragma warning restore");
					writeCode(fs, 1, "}");
                    fs.WriteLine();
                    break;

                case ECfgMgrType.OneGroupMgr:
                    writeCode(fs, 1, "public class {0} : {1}<{0}, {2}, {3}>",
						sMgrName, cfgSrc.MgrType.CsMgrBaseName, AllColumnInfo[exportColClientList[0]].DataType.CsName, sClassName);
					writeCode(fs, 1, "{");
					writeCode(fs, 0, "#pragma warning disable");
					writeCode(fs, 2, XConfigDefine.CSHARP_AOT_FUNC_DEF);
					writeCode(fs, 2, "{");
					writeCode(fs, 3, "Dictionary<int, List<{0}>> _un = new Dictionary<int, List<{0}>>();", sClassName);
					writeCode(fs, 2, "}");
					writeCode(fs, 0, "#pragma warning restore");
					writeCode(fs, 1, "}");
                    fs.WriteLine();
                    break;

                case ECfgMgrType.TwoKeyMgr:
                    writeCode(fs, 1, "public class {0} : {1}<{0}, {2}, {3}, {4}>",
						sMgrName, cfgSrc.MgrType.CsMgrBaseName, AllColumnInfo[exportColClientList[0]].DataType.CsName,
						AllColumnInfo[exportColClientList[1]].DataType.CsName, sClassName);
					writeCode(fs, 1, "{");
					writeCode(fs, 0, "#pragma warning disable");
					writeCode(fs, 2, XConfigDefine.CSHARP_AOT_FUNC_DEF);
					writeCode(fs, 2, "{");
					writeCode(fs, 3, "Dictionary<int, {0}> _un1 = new Dictionary<int, {0}>();", sClassName);
					writeCode(fs, 3, "Dictionary<int, Dictionary<int, {0}>> _un2 = new Dictionary<int, Dictionary<int, {0}>>();", sClassName);
					writeCode(fs, 2, "}");
					writeCode(fs, 0, "#pragma warning restore");
					writeCode(fs, 1, "}");
                    fs.WriteLine();
                    break;

                default:
                    break;
            }
        }

        private void writeCsClassDef(StreamWriter fs, XCsSrcInfo cfgSrc)
        {
			writeCode(fs, 1, "[ProtoContract]");
            string sClassName = cfgSrc.ClassName;
            switch (cfgSrc.MgrType.MgrEnum)
            {
                case ECfgMgrType.OneKeyMgr:
                case ECfgMgrType.OneGroupMgr:
                    writeCode(fs, 1, "public class {0} : {1}<{2}>",
                        sClassName, XConfigDefine.CSHARP_CLASS_BASE_1KEY,
						AllColumnInfo[exportColClientList[0]].DataType.CsName);
                    break;
                case ECfgMgrType.TwoKeyMgr:
                    writeCode(fs, 1, "public class {0} : {1}<{2}, {3}>",
                        sClassName, XConfigDefine.CSHARP_CLASS_BASE_2KEY,
						AllColumnInfo[exportColClientList[0]].DataType.CsName,
						AllColumnInfo[exportColClientList[1]].DataType.CsName);
                    break;
                default:
                    writeCode(fs, 1, "public class {0} : {1}",
                        sClassName, XConfigDefine.CSHARP_CLASS_BASE_0KEY);
                    break;
            }
        }

        // 根据管理器类别输出结构体的键值接口
        private void writeCsKeyInterface(StreamWriter fs, XCsSrcInfo cfgSrc)
        {
            switch (cfgSrc.MgrType.MgrEnum)
            {
                case ECfgMgrType.OneKeyMgr:
                case ECfgMgrType.OneGroupMgr:
                    writeCode(fs, 2, "public {0} {1}() {{ return {2}; }}",
						AllColumnInfo[exportColClientList[0]].DataType.CsName,
						XConfigDefine.KEY1_FUN, AllColumnInfo[exportColClientList[0]].VarName);
                    fs.WriteLine();
                    break;
                case ECfgMgrType.TwoKeyMgr:
                    writeCode(fs, 2, "public {0} {1}() {{ return {2}; }}",
						AllColumnInfo[exportColClientList[0]].DataType.CsName,
						XConfigDefine.KEY1_FUN, AllColumnInfo[exportColClientList[0]].VarName);
                    fs.WriteLine();
                    writeCode(fs, 2, "public {0} {1}() {{ return {2}; }}",
						AllColumnInfo[exportColClientList[1]].DataType.CsName,
                        XConfigDefine.KEY2_FUN, AllColumnInfo[exportColClientList[1]].VarName);
                    fs.WriteLine();
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region 导出 C++ 代码

        private bool doExportCppSource(XCppSrcInfo cfgSrc)
        {
            string sClassName = cfgSrc.ClassName;
            string sFileName = cfgSrc.FilePath;

            if (XConfigDefine.IsValidCodeName(sClassName) == false)
            {
                Log(ELogType.ERROR, "导出 C++ 代码文件失败, 类名不符合规则, 文件名:{0}, 类名:{1}", sFileName, sClassName);
                return false;
            }

            if (createFile(sFileName) == false)
            {
                return false;
            }

            try
            {
                // 服务器代码加上 UTF8 标记, 不然 VS 可能识别报错
                using (StreamWriter fs = new StreamWriter(sFileName, false, Encoding.UTF8))
                {
                    List<int> exColList = getExportColumnList(EConfigShareFlag.Server);

                    // 头部数据
                    writeCode(fs, 0, XConfigDefine.SOURCE_FILE_HEAD);
                    fs.WriteLine();
                    writeCppHead(fs, cfgSrc);

                    // 类定义
                    //--4>TODO: 考虑加上大小
                    writeCode(fs, 0, "struct {0} : public {1}<{2}, {3}>", sClassName, XConfigDefine.CPP_CLASS_BASE, sClassName, cfgSrc.Capacity);
                    writeCode(fs, 0, "{");

                    // 成员变量定义
                    foreach (int col in exColList)
                    {
                        XColumnInfo info = AllColumnInfo[col];
                        // 数组只有下标为 0 的需要定义
                        if (false == info.IsArray || 0 == info.ArrayIndex)
                        {
							writeCode(fs, 1, "{0};\t\t\t\t// {1}", info.GetVarDefineStr(ECodeLanguageType.CPP), info.ColDesc.Replace('\r', ' ').Replace('\n', ' '));
                        }
                    }
                    fs.WriteLine();

                    // 定义获取主键值的函数
                    writeCppKeyInterface(fs, cfgSrc);

                    // 数据读取函数
                    writeCode(fs, 1, XConfigDefine.CPP_READ_METHOD);
                    writeCode(fs, 1, "{");

                    // 所有字段读取
                    // 临时变量定义标记
                    bool[] bTempVarFlag = new bool[] { false, false, false }; // int, float, string
                    foreach (int col in exColList)
                    {
                        XColumnInfo info = AllColumnInfo[col];
                        switch (info.DataType.ConfigName)
                        {
                            case "int":
                            case "uint":
                            case "short":
                            case "ushort":
                            case "word":
                            case "byte":
                            case "char":
                                if (false == bTempVarFlag[0])
                                {
                                    writeCode(fs, 2, "INT {0};", XConfigDefine.CPP_TEMP_INT);
                                    bTempVarFlag[0] = true;
                                }
                                writeCode(fs, 2, "{0}.GetInteger(\"{1}\", 0, (int*)&{2}); this->{3} = ({4}){2};",
                                    XConfigDefine.CPP_READ_READER, info.ConfigName, XConfigDefine.CPP_TEMP_INT, info.VarName, info.DataType.CppName);
                                break;
                            case "float":
                                if (false == bTempVarFlag[1])
                                {
                                    writeCode(fs, 2, "FLOAT {0};", XConfigDefine.CPP_TEMP_FLOAT);
                                    bTempVarFlag[1] = true;
                                }
                                writeCode(fs, 2, "{0}.GetFloat(\"{1}\", 0, (float*)&{2}); this->{3} = ({4}){2};",
                                    XConfigDefine.CPP_READ_READER, info.ConfigName, XConfigDefine.CPP_TEMP_FLOAT, info.VarName, info.DataType.CppName);
                                break;
                            case "string":
                            case "vector3":
                                if (false == bTempVarFlag[2])
                                {
                                    writeCode(fs, 2, "char {0}[{2}];", XConfigDefine.CPP_TEMP_STRING, XConfigDefine.CPP_TEMP_STRING, XConfigDefine.CPP_TEMP_STR_LEN);
                                    bTempVarFlag[2] = true;
                                }
                                if (info.DataType is XConfigDataType_Normal)
                                {
                                    writeCode(fs, 2, "{0}.GetString(\"{1}\", \"\", {2}, {3}); this->{4} = ({5}){2};",
                                        XConfigDefine.CPP_READ_READER, info.ConfigName, XConfigDefine.CPP_TEMP_STRING,
                                        XConfigDefine.CPP_TEMP_STR_LEN, info.VarName, info.DataType.CppName);
                                }
                                else
                                {
                                    writeCode(fs, 2, "{0}.GetString(\"{1}\", \"\", {2}, {3}); this->{4} = {5}({2});",
                                        XConfigDefine.CPP_READ_READER, info.ConfigName, XConfigDefine.CPP_TEMP_STRING,
                                        XConfigDefine.CPP_TEMP_STR_LEN, info.VarName, (info.DataType as XConfigDataType_Special).Reader_CPP);
                                }
                                break;
                            default:
                                Log(ELogType.ERROR, "导出 c++ 代码文件失败, 字段类型不对, 字段名:{0}, 字段类型:{1}, 文件名:{2}, 类名:{3}",
                                    info.VarName, info.DataType.ConfigName, sFileName, sClassName);
                                return false;
                        }
                    }

                    // 函数末尾
                    writeCode(fs, 2, "return TRUE;");
                    writeCode(fs, 1, "}");

                    // 类末尾
                    writeCode(fs, 0, "};");
                    fs.WriteLine();

                    // 管理器定义
                    writeCppMgrDef(fs, cfgSrc);

                    fs.Close();
                }
            }
            catch (System.Exception ex)
            {
                Log(ELogType.ERROR, "导出 c# 代码文件失败, 文件名:{0}, 类名:{1}, 错误:{2}", sFileName, sClassName, ex.ToString());
                return false;
            }
            return true;
        }

        // 头文件
        private void writeCppHead(StreamWriter fs, XCppSrcInfo cfgSrc)
        {
            writeCode(fs, 0, XConfigDefine.CPP_INCLUDE);
            writeCode(fs, 0, cfgSrc.MgrType.CppMgrHead);
            fs.WriteLine();
        }

        // 根据管理器类别输出结构体的键值接口
        private void writeCppKeyInterface(StreamWriter fs, XCppSrcInfo cfgSrc)
        {
            switch (cfgSrc.MgrType.MgrEnum)
            {
                case ECfgMgrType.OneKeyMgr:
                case ECfgMgrType.OneGroupMgr:
                    writeCode(fs, 1, "inline {0} {1}() const {{ return {2}; }}", AllColumnInfo[exportColServerList[0]].DataType.CppName,
						XConfigDefine.KEY1_FUN, AllColumnInfo[exportColServerList[0]].VarName);
                    fs.WriteLine();
                    break;
                case ECfgMgrType.TwoKeyMgr:
					writeCode(fs, 1, "inline {0} {1}() const {{ return {2}; }}", AllColumnInfo[exportColServerList[0]].DataType.CppName,
						XConfigDefine.KEY1_FUN, AllColumnInfo[exportColServerList[0]].VarName);
                    fs.WriteLine();
					writeCode(fs, 1, "inline {0} {1}() const {{ return {2}; }}", AllColumnInfo[exportColServerList[1]].DataType.CppName,
						XConfigDefine.KEY2_FUN, AllColumnInfo[exportColServerList[1]].VarName);
                    fs.WriteLine();
                    break;
                default:
                    break;
            }
        }

        // 管理器类定义
        private void writeCppMgrDef(StreamWriter fs, XCppSrcInfo cfgSrc)
        {
            string sClassName = cfgSrc.ClassName;
            string sMgrName = sClassName + XConfigDefine.MANAGER_CLASS_SUFFIX;
            switch (cfgSrc.MgrType.MgrEnum)
            {
                case ECfgMgrType.NoKeyList:
                    writeCode(fs, 0, "class {0} : public {1}<{0}, {2}, {3}, {4}> {{ }};", sMgrName,
                        cfgSrc.MgrType.CppMgrBaseName, sClassName, cfgSrc.Capacity, cfgSrc.StepSize);
                    fs.WriteLine();
                    break;
                case ECfgMgrType.OneKeyMgr:
                    writeCode(fs, 0, "class {0} : public {1}<{0}, {2}, {3}, {4}, {5}> {{ }};", sMgrName, cfgSrc.MgrType.CppMgrBaseName,
						AllColumnInfo[exportColServerList[0]].DataType.CppName, sClassName, cfgSrc.Capacity, cfgSrc.StepSize);
                    fs.WriteLine();
                    break;
                case ECfgMgrType.OneGroupMgr:
                    writeCode(fs, 0, "class {0} : public {1}<{0}, {2}, {3}, {4}, {5}> {{ }};", sMgrName, cfgSrc.MgrType.CppMgrBaseName,
						AllColumnInfo[exportColServerList[0]].DataType.CppName, sClassName, cfgSrc.KeyCount, cfgSrc.GroupSize);
                    fs.WriteLine();
                    break;
                case ECfgMgrType.TwoKeyMgr:
                    writeCode(fs, 0, "class {0} : public {1}<{0}, {2}, {3}, {4}, {5}, {6}> {{ }};", sMgrName, cfgSrc.MgrType.CppMgrBaseName,
						AllColumnInfo[exportColServerList[0]].DataType.CppName, AllColumnInfo[exportColServerList[1]].DataType.CppName,
                        sClassName, cfgSrc.KeyCount, cfgSrc.GroupSize);
                    fs.WriteLine();
                    break;
                default:
                    break;
            }
        }

        #endregion

        private void writeCode(StreamWriter fs, int tabNum, string data)
        {
            writeCode(fs, tabNum, data, null);
        }

        private void writeCode(StreamWriter fs, int tabNum, string format, params object[] args)
        {
            if (tabNum < 0 || tabNum > XConfigDefine.TAB_STRING_MAX.Length)
            {
                tabNum = 0;
            }
            if (null == args)
            {
                fs.WriteLine(XConfigDefine.TAB_STRING_MAX.Substring(0, tabNum) + format);
            }
            else
            {
                fs.WriteLine(XConfigDefine.TAB_STRING_MAX.Substring(0, tabNum) + string.Format(format, args));
            }
        }

        #endregion

        private List<int> getExportColumnList(EConfigShareFlag flag)
        {
            List<int> exColList = new List<int>();
            for (int col = 0; col < ColumnNumber; ++col)
            {
				XColumnInfo info = AllColumnInfo[col];
                if (info.IsValid && (info.ShareType.Flag & flag) != EConfigShareFlag.None)
                {
                    exColList.Add(col);
                }
            }
            return exColList;
        }

        private bool createFile(string sFileName)
        {
            if (null == sFileName || sFileName == string.Empty)
            {
                Log(ELogType.ERROR, "创建文件失败, 文件名为空");
                return false;
            }
            try
            {
                if (File.Exists(sFileName))
                {
                    if (FileAttributes.ReadOnly == (File.GetAttributes(sFileName) & FileAttributes.ReadOnly))
                    {
                        Log(ELogType.ERROR, "文件:{0} 是只读的, 请先从 Perforce 版本库 checkout.", sFileName);
                        return false;
                    }
                    else
                    {
                        File.Delete(sFileName);
                    }
                }
                using (FileStream fs = File.Create(sFileName))
                {
                    fs.Close();
                }
            }
            catch (System.Exception ex)
            {
                Log(ELogType.ERROR, "创建文件失败, 文件:{0}, 错误:{1}", sFileName, ex.ToString());
                return false;
            }
            return true;
        }

        public void Log(ELogType t, string format, params object[] args)
        {
            if (LogEvent != null)
            {
                LogEvent(t, format, args);
            }
        }
    }
}
