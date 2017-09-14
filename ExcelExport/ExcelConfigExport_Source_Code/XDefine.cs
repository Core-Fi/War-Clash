using System;

namespace ExcelConfigExport
{
    public enum ELogType
    {
        INFO,
        DEBUG,
        WARN,
        ERROR,
        FATAL,
    }

    public delegate void LogEventHandler(ELogType t, string format, params object[] args);

    class XDefine
    {
        public static readonly string EXCEL_FILTER = "Excel文件(*.xlsx)|*.xlsx";

        public static readonly string OPEN_NEW_EXCEL = "==== 加载新的 Excel ====";

        // 自动检测当前加载的 EXCEL 更新的频率
        public static readonly int AUTO_CHECK_UPDATE_TIME = 1000;

    }
}
