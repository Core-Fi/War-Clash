using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace ExcelConfigExport
{
    enum EConfigHeadType
    {
        Begin = 0,
        CodeName = Begin,
        DataType,
        ShareType,
        Desc,
        Count,
    }

    enum EConfigShareFlag
    {
        None = 0,
        Client = (1 << 1),
        Server = (1 << 2),
        Both = Client + Server,
        Skip = (1 << 3),
    }

    enum ECfgMgrType
    {
        None = 0,
        OneKeyMgr,      // 第一列键值唯一索引
        TwoKeyMgr,      // 前两列键值组唯一索引
        NoKeyList,      // 无键值配置数据列表
        OneGroupMgr,    // 根据第一列分组
    }

    enum ECodeLanguageType
    {
        CS = 0,
        CPP = 1,
    }

    abstract class XConfigDataType
    {
        internal string ConfigName { get; private set; }
		internal string CppName { get; private set; }
		internal string CsName { get; private set; }
		internal abstract bool CanBeKey { get; }

        internal XConfigDataType(string cfg, string cpp, string cs)
        {
            CppName = cpp;
            CsName = cs;
            ConfigName = cfg;
        }

        internal abstract object GetData(object data);
    }

    class XConfigDataType_Normal : XConfigDataType
    {
		internal Type CsType { get; private set; }

		internal override bool CanBeKey { get { return true;  } }

        internal XConfigDataType_Normal(string cfg, string cpp, string cs, Type t)
            : base(cfg, cpp, cs)
        {
            CsType = t;
        }

        internal override object GetData(object data)
        {
			if (null == data)
			{
				if (typeof(int) == CsType)
				{
					return 0;
				}
				else if (typeof(string) == CsType)
				{
					return string.Empty;
				}
			}

            try
            {
                return Convert.ChangeType(data, CsType);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

    class XConfigDataType_Special : XConfigDataType
    {
        internal Regex FmtReg { get; private set; }
        internal string Reader_CS { get; private set; }
        internal string Reader_CPP { get; private set; }

		internal override bool CanBeKey { get { return false;  } }
        internal XConfigDataType_Special(string cfg, string cpp, string cs, Regex sReg, string csReader, string cppReader) : base(cfg, cpp, cs)
        {
            FmtReg = sReg;
            Reader_CS = csReader;
            Reader_CPP = cppReader;
        }

        internal override object GetData(object data)
        {
            if (data == null || data.ToString().Length == 0)
            {
                return null;
            }
            return FmtReg.IsMatch(data.ToString()) ? data.ToString() : null;
        }
    }

    class XConfigShareType
    {
        internal string Name { get; private set; }
        internal EConfigShareFlag Flag { get; private set; }

        internal XConfigShareType(string name, EConfigShareFlag f)
        {
            Name = name;
            Flag = f;
        }
    }

    class XCfgMgrType
    {
        internal ECfgMgrType MgrEnum { get; private set; }
        internal string MgrDesc { get; private set; }
        internal string CsMgrBaseName { get; private set; }
        internal string CppMgrBaseName { get; private set; }
        internal string CppMgrHead { get; private set; }    // 头文件包含语句

        internal bool IsCreateMgr { get { return MgrEnum != ECfgMgrType.None; } }

        internal XCfgMgrType(ECfgMgrType e, string desc, string csbase, string cppbase, string cpphead)
        {
            MgrEnum = e;
            MgrDesc = desc;
            CsMgrBaseName = csbase;
            CppMgrBaseName = cppbase;
            CppMgrHead = cpphead;
        }

        public override string ToString()
        {
            return MgrDesc;
        }
    }

    class XColumnInfo
    {
        internal string ColDesc { get; private set; }
        internal string ConfigName { get; private set; }
        internal XConfigDataType DataType { get; private set; }
        internal XConfigShareType ShareType { get; private set; }

        //--4>: 数组的配置名格式是 varName_Size_Index
        internal int ArrayIndex { get; private set; }
        internal int ArraySize { get; private set; }
        internal bool IsArray { get { return ArrayIndex >= 0; } }

        internal string DefName { get; private set; }
        internal string VarName
        {
            get { return IsArray ? DefName + "[" + ArrayIndex + "]" : DefName; }
        }
        internal string GetVarDefineStr(ECodeLanguageType e)
        {
            switch (e)
            {
                case ECodeLanguageType.CS:
                    return string.Format(IsArray ? "{0}[] {1}" : "{0} {1}", DataType.CsName, DefName);
                case ECodeLanguageType.CPP:
                    return string.Format(IsArray ? "{0} {1}[{2}]" : "{0} {1}", DataType.CppName, DefName, ArraySize);
                default:
                    break;
            }
            return string.Empty;
        }

		internal bool IsValid
		{
			get 
			{
				if (XConfigDefine.IsSkipCodeName(ConfigName)) return false;
				if (ShareType.Flag == EConfigShareFlag.Skip) return false;
				return true;
			}
		}

        private XColumnInfo()
        {
            ArrayIndex = -1;
            ArraySize = 0;
        }

        internal static XColumnInfo CreateColumnInfo(string desc, string name, XConfigDataType dt, XConfigShareType share, 
			string defName, int arrIdx, int arrSize)
        {
            XColumnInfo info = new XColumnInfo();
			info.ColDesc = desc;
			info.ConfigName = name;
			info.DataType = dt;
			info.ShareType = share;
			info.DefName = defName;
			info.ArrayIndex = arrIdx;
			info.ArraySize = arrSize;
            return info;
        }
    }

    class XCfgExportInfo
    {
        public string ClientConfig { get; private set; }
        public string ServerConfig { get; private set; }

        public XCfgExportInfo(string ccfg, string scfg)
        {
            ClientConfig = ccfg;
            ServerConfig = scfg;
        }
    }

    abstract class XSrcInfoBase
    {
        internal string ClassName { get; private set; }
        internal XCfgMgrType MgrType { get; private set; }
        internal string FilePath { get; private set; }

        internal XSrcInfoBase(string sName, XCfgMgrType mgr, string sPath)
        {
            ClassName = sName;
            MgrType = mgr;
            FilePath = sPath;
        }
    }

    class XCppSrcInfo : XSrcInfoBase
    {
        internal uint Capacity { get; private set; }
        internal uint StepSize { get; private set; }
        // 如果第一列不是唯一主键, 设置第一列的种类数量, 用于分组管理器或者多键值管理器
        internal uint KeyCount { get; private set; }
        // 如果第一列不是唯一主键, 设置第二分类的数量, 用于分组管理器或者多键值管理器
        internal uint GroupSize { get; private set; }

        internal XCppSrcInfo(string sName, XCfgMgrType mgr, string sPath, uint cap, uint step, uint keyCount, uint groupSize)
            : base(sName, mgr, sPath)
        {
            Capacity = cap;
            StepSize = step;
            KeyCount = keyCount;
            GroupSize = groupSize;
        }
    }

    class XCsSrcInfo : XSrcInfoBase
    {
        internal XCsSrcInfo(string sName, XCfgMgrType mgr, string sPath)
            : base(sName, mgr, sPath)
        {
        }
    }

    class XSrcExportInfo
    {
        internal static readonly uint DEFAULT_CAPACITY = 1024;
        internal static readonly uint DEFAULT_STEP_SIZE = 512;
        internal static readonly uint DEFAULT_KEY_COUNT = 64;
        internal static readonly uint DEFAULT_GROUP_SIZE = 64;

        internal XCsSrcInfo CsInfo { get; private set; }
        internal XCppSrcInfo CppInfo { get; private set; }

        internal string GetSourceName(ECodeLanguageType e)
        {
            switch (e)
            {
                case ECodeLanguageType.CS:
                    return CsInfo.FilePath;
                case ECodeLanguageType.CPP:
                    return CppInfo.FilePath;
                default:
                    break;
            }
            return string.Empty;
        }

        internal XSrcExportInfo(XCsSrcInfo cs, XCppSrcInfo cpp)
        {
            CsInfo = cs;
            CppInfo = cpp;
        }
    }

    class XExportInfo
    {
        internal string ExcelFile { get; private set; }
        internal string SheetName { get; private set; }
        internal XCfgExportInfo ConfigInfo { get; private set; }
        internal XSrcExportInfo SourceInfo { get; private set; }

        internal XExportInfo(string sExcelFile, string sSheetName, XCfgExportInfo cfg, XSrcExportInfo src)
        {
            ExcelFile = sExcelFile;
            SheetName = sSheetName;
            ConfigInfo = cfg;
            SourceInfo = src;
        }
    }


    class XConfigDefine
    {
        // 如果某列的共享字段为 X 就跳过该列
        // 如果某数据行第一个字段以 SKIP_LINE_FLAG 就跳过
        internal static readonly List<string> SKIP_LINE_FLAG = new List<string>(new string[]{"(#skip)", "skip", "#skip", "(skip)"});

        // UTF8Encoding without EmitUTF8Identifier
        // 防止输出的配置文件和代码里包含 UTF8 编码标记(bom)
        internal static UTF8Encoding UTF8_WITHOUTBOM = new UTF8Encoding(false);

		internal static UTF8Encoding UTF8_WITHBOM = new UTF8Encoding(true);
        // 变量名规范
        private static Regex VAR_NAME_REGEX = new Regex(@"^[a-zA-Z_]\w+$");
        //--4>: 数组的配置名格式是 varName_Size_Index
        internal static Regex VAR_ARRAY_NAME_REGEX = new Regex(@"^([a-zA-Z_]\w+)_([0-9]+)_([0-9]+)$");

		//  Vector3定义 1.0,2,3.5
		internal static Regex VAR_VECTOR3_NAME_REGEX = new Regex(@"-?[\d.]+\s*,\s*-?[\d.]+\s*,\s*-?[\d.]+");

		internal static bool IsSkipCodeName(object name)
		{
			if (null == name) return true;
			string str = name.ToString().ToLower();
			if(SKIP_LINE_FLAG.Contains(str)) return true;
			return false;
		}

        internal static bool IsValidCodeName(object name)
        {
            if (null == name)
            {
                return false;
            }
            return VAR_NAME_REGEX.IsMatch(name.ToString().Trim());
        }

        internal static readonly string SPLIT_STRING = "\t";

        internal static readonly string TAB_STRING_MAX = "\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t";

		internal static readonly string SOURCE_FILE_HEAD = @"
//============================================
//    Exported by ExcelConfigExport
//
//    此代码为工具根据配置自动生成, 建议不要修改
//
//============================================";

        // 获取主键值的函数, 默认第一列为主键
        internal static readonly int KEY1_INDEX = 0;
        internal static readonly int KEY2_INDEX = 1;
        internal static readonly string KEY1_FUN = "GetKey1";
        internal static readonly string KEY2_FUN = "GetKey2";

        internal static readonly string MANAGER_CLASS_SUFFIX = "Mgr";

        internal static readonly string CSHARP_CLASS_BASE_0KEY = "ITabItem";
        internal static readonly string CSHARP_CLASS_BASE_1KEY = "ITabItemWith1Key";
        internal static readonly string CSHARP_CLASS_BASE_2KEY = "ITabItemWith2Key";
        internal static readonly string CSHARP_MANAGER_BASE = "XConfigOneKeyManager";
		internal static readonly string CSHARP_INCLUDE = @"using System;
using System.Collections.Generic;
using ProtoBuf;";
		internal static readonly string CSHARP_AOT_FUNC_DEF = @"private void __unused()";
		internal static readonly string CSHARP_NAME_SPACE = "namespace Logic.TabConfig";
        internal static readonly string CSHARP_KEY_PREFIX = "__";
        internal static readonly string CSHARP_READ_READER = "tf";
        internal static readonly string CSHARP_READ_METHOD = "public bool ReadItem(TabFile " + CSHARP_READ_READER + ")";
        internal static readonly string CSHARP_VECTOR3_READER = "XUtil.String2Vector3";

        internal static readonly string CPP_CLASS_BASE = "System::Memory::KPortableStepPool";
        internal static readonly string CPP_MANAGER_BASE = "CConfigOneKeyManager";
		internal static readonly string CPP_INCLUDE = @"#pragma once
#include ""System/KType.h""
#include ""System/Memory/KStepObjectPool.h""";
        internal static readonly string CPP_KEY_PREFIX = "_KEY_";
        internal static readonly string CPP_READ_READER = "tf";
        internal static readonly string CPP_READ_METHOD = "BOOL ReadItem(KTabFile2& " + CPP_READ_READER + ")";
        internal static readonly string CPP_TEMP_INT = "nValue";
        internal static readonly string CPP_TEMP_FLOAT = "fValue";
        internal static readonly string CPP_TEMP_STRING = "strTemp";
        internal static readonly string CPP_TEMP_STR_LEN = "256";
        internal static readonly string CPP_VECTOR3_READER = "String2Vector3";

		internal static readonly string DEFAULT_CLIENT_CONFIG_PATH = "../../Client/Assets/StreamingAssets/Config/";
        internal static readonly string DEFAULT_SERVER_CONFIG_PATH = "../../Server/Bin/Settings/";
        internal static readonly string DEFAULT_CLIENT_SOURCE_PATH = "../../LogicCore/TabConfig/";
        internal static readonly string DEFAULT_SERVER_SOURCE_PATH = "../../Server/GameServer/TabConfig/";

        //--4>TODO: 考虑增加 vector3, 数据等
        internal static readonly XConfigDataType[] ALL_DATA_TYPE = new XConfigDataType[] {
            new XConfigDataType_Normal("int", "INT", "int", typeof(int)),
            new XConfigDataType_Normal("uint", "DWORD", "uint", typeof(uint)),
            new XConfigDataType_Normal("short", "SHORT", "short", typeof(short)),
            new XConfigDataType_Normal("ushort", "USHORT", "ushort", typeof(ushort)),
            new XConfigDataType_Normal("word", "WORD", "ushort", typeof(ushort)),
            new XConfigDataType_Normal("byte", "BYTE", "byte", typeof(byte)),
            new XConfigDataType_Normal("char", "CHAR", "char", typeof(char)),
            new XConfigDataType_Normal("float", "FLOAT", "float", typeof(float)),
            //--4>TODO: 考虑 C++ 的 string 用小点
            new XConfigDataType_Normal("string", "KSTRING", "string", typeof(string)),

            new XConfigDataType_Special("vector3", "Vector3", "Vector3", VAR_VECTOR3_NAME_REGEX, CSHARP_VECTOR3_READER, CPP_VECTOR3_READER),

        };

        internal static readonly XConfigShareType[] ALL_SHARE_TYPE = new XConfigShareType[] {
            // 如果某列的共享字段为 x 就不读取该列的数据
            new XConfigShareType("skip", EConfigShareFlag.Skip),
            new XConfigShareType("c", EConfigShareFlag.Client),
            new XConfigShareType("s", EConfigShareFlag.Server),
            new XConfigShareType("cs", EConfigShareFlag.Both),
        };

        internal static XCfgMgrType GetMgrType(string eName)
        {
            foreach (XCfgMgrType mgr in ALL_MGR_TYPE)
            {
                if (mgr.MgrEnum.ToString() == eName)
                {
                    return mgr;
                }
            }
            return ALL_MGR_TYPE[0];
        }

        internal static readonly XCfgMgrType[] ALL_MGR_TYPE = new XCfgMgrType[] {
            new XCfgMgrType(ECfgMgrType.None, "不创建管理器", string.Empty, string.Empty, string.Empty),

            //--4>: 为啥增加下面这行 360 会报木马?
            new XCfgMgrType(ECfgMgrType.OneKeyMgr, "第一列索引管理器", "CCfg1KeyMgrTemplate",
                "CCfg1KeyMgrTemplate", @"#include ""TabConfigDefine/Config1KeyMgrTemplate.h"""),

            new XCfgMgrType(ECfgMgrType.TwoKeyMgr, "前两列索引管理器", "CCfg2KeyMgrTemplate",
                "CCfg2KeyMgrTemplate", @"#include ""TabConfigDefine/Config2KeyMgrTemplate.h"""),

            new XCfgMgrType(ECfgMgrType.NoKeyList, "无索引链表管理器", "CCfgListMgrTemplate",
                "CCfgListMgrTemplate", @"#include ""TabConfigDefine/ConfigListMgrTemplate.h"""),

            new XCfgMgrType(ECfgMgrType.OneGroupMgr, "第一列分组管理器", "CCfg1GroupMgrTemplate",
                "CCfg1GroupMgrTemplate", @"#include ""TabConfigDefine/Config1GroupMgrTemplate.h"""),
        };

        internal static readonly string EXPORT_HISTORY_FILE = "ExcelConfigExport.xml";
    }
}
