using System;
using Excel = Microsoft.Office.Interop.Excel;

namespace ExcelConfigExport
{
    class XExcelConfig
    {
        public static XExcelConfig LoadExcelConfig(Excel.Worksheet worksheet, LogEventHandler handler)
        {
            XExcelConfig excelConfig = new XExcelConfig();
            excelConfig.LogEvent += handler;
            if (worksheet == null || excelConfig.Init(worksheet) == false)
            {
                return null;
            }
            return excelConfig;
        }

        public event LogEventHandler LogEvent;

        private Excel.Worksheet m_Worksheet;
        private XConfigData m_ConfigData;
        private object[,] m_AllData;

        private XExcelConfig() { }

        private void Reset()
        {
            m_Worksheet = null;
            m_ConfigData = null;
            m_AllData = null;
        }

        private bool Init(Excel.Worksheet worksheet)
        {
            if (worksheet == null)
            {
                Log(ELogType.ERROR, "配置表单为空");
                return false;
            }

            m_Worksheet = worksheet;

            Excel.Range range = worksheet.UsedRange.Cells;
            int nRowNum = range.Rows.Count;
            int nColNum = range.Columns.Count;

            Log(ELogType.INFO, "--4>: 开始加载配置表单文件, 表单名:{0}, 行数:{1} 列数:{2}", worksheet.Name, nRowNum, nColNum);

            //--4>: 读取数据表表头信息
            if (nRowNum < 4 || nColNum < 1)
            {
                Log(ELogType.ERROR, "数据表格式有误");
                return false;
            }

            m_AllData = range.Value as object[,];

            m_ConfigData = XConfigData.CreateConfigData(m_AllData, LogEvent);

            if (null == m_ConfigData)
            {
                Log(ELogType.ERROR, "加载数据表表头失败, 表单名:{0}", worksheet.Name);
                return false;
            }

            Log(ELogType.INFO, "--4>: 成功加载数据表表头信息, 表单名:{0}", worksheet.Name);
            return true;
        }

        public bool DoCheck(ECfgMgrType csMgrType, ECfgMgrType cppMgrType)
        {
            if (null == m_ConfigData || null == m_AllData)
            {
                Log(ELogType.ERROR, "检验数据表失败, 尚未载入数据表信息");
                return false;
            }
            if (m_ConfigData.CheckMultiLine(m_AllData, m_AllData.GetLowerBound(0) + (int)EConfigHeadType.Count, csMgrType, cppMgrType) == false)
            {
                Log(ELogType.ERROR, "检验数据表失败");
                return false;
            }
            Log(ELogType.INFO, "检验数据表成功");
            return true;
        }

        public bool DoExport(XCfgExportInfo cfgInfo, XSrcExportInfo srcInfo, bool bExportCfg, bool bExportSrc)
        {
            if (null == cfgInfo && null == srcInfo)
            {
                Log(ELogType.WARN, "没有导出任何数据");
                return true;
            }

            if (null == m_ConfigData || null == m_AllData)
            {
                Log(ELogType.ERROR, "导出数据失败, 尚未载入数据表信息");
                return false;
            }
            if (bExportCfg && null != cfgInfo)
            {
                m_ConfigData.ClearData();
                if (m_ConfigData.AddMultiLine(m_AllData, m_AllData.GetLowerBound(0) + (int)EConfigHeadType.Count, 
					srcInfo.CsInfo.MgrType.MgrEnum, srcInfo.CppInfo.MgrType.MgrEnum) == false)
                {
                    Log(ELogType.ERROR, "加载数据表数据失败");
                    return false;
                }
                Log(ELogType.INFO, "成功加载数据表的配置数据");

                if (m_ConfigData.ExportConfigFile(cfgInfo) == false)
                {
                    return false;
                }
            }
			if (bExportSrc && null != srcInfo)
            {
                if (m_ConfigData.ExportSourceFile(srcInfo) == false)
                {
                    return false;
                }
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
