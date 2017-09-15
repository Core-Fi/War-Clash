using System;
using System.Windows.Forms;
using System.Reflection;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using NPOI;
namespace ExcelConfigExport
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            this.Init();
        }

        private void Init()
        {
            this.ClearAll();

            m_History = new XExportHistory();

            comboBox_ExcelList.Items.Add(XDefine.OPEN_NEW_EXCEL);
            comboBox_ExcelList.Items.AddRange(m_History.GetAllFile());

            m_LoadedCheckTimer.Interval = XDefine.AUTO_CHECK_UPDATE_TIME;
            m_LoadedCheckTimer.Tick += updateTimer_Tick;

            // 管理器分类
            foreach (XCfgMgrType mgr in XConfigDefine.ALL_MGR_TYPE)
            {
                comboBox_ManagerCS.Items.Add(mgr);
                comboBox_ManagerCPP.Items.Add(mgr);
            }
        }

        #region Logical Members
        private Excel._Application m_ExcelApp;
        private Excel.WorkbookClass m_Workbook;
        private Hashtable m_WorksheetMap;

        // 当前选择的配置表单
        private Excel.Worksheet m_SelectedWorksheet;
        private XExcelConfig m_SelectedConfigSheet;
        private XExportHistory m_History;

        // 保存相对路径
        public string LoadedExcelFile { get; private set; }
        private DateTime m_ExcelLoadTime = DateTime.Now;
        private Timer m_LoadedCheckTimer = new Timer();

        #endregion

        #region Logical Functions

        void ClearAll()
        {
            // 清空界面
            listBox_Sheets.Items.Clear();
            this.ResetWorksheetConfig();

            richTextBox_Log.Clear();

            // 清空数据
            LoadedExcelFile = string.Empty;

            m_WorksheetMap = new Hashtable();

            try
            {
                if (null != m_Workbook)
                {
                    m_Workbook.Close(false, false, Missing.Value);
                }
            m_Workbook = null;
            }
            catch (System.Exception ex)
            {
                Log(ELogType.ERROR, ex.ToString());
            }
            finally
            {
                if (null != m_ExcelApp)
                {
                    m_ExcelApp.Quit();
                }
                m_ExcelApp = null;
            }
        }

        private bool EnableConfigOper
        {
            set
            {
                textBox_ClientConfig.Enabled = value;
                textBox_ServerConfig.Enabled = value;
            }
        }

        private bool EnableSourceOper
        {
            get
            {
                return checkBox_ExportSrc.Checked;
            }
            set
            {
                comboBox_ManagerCS.Enabled = value;
                textBox_ClassNameCS.Enabled = value;
                textBox_ClientSrc.Enabled = value;

                comboBox_ManagerCPP.Enabled = value;
                textBox_ClassNameCPP.Enabled = value;
                textBox_ServerSrc.Enabled = value;

                UpdateMgrConfig();
            }
        }

        private bool EnableFuncOper
        {
            set
            {
                checkBox_ExportConfig.Enabled = value;
                checkBox_ExportSrc.Enabled = value;
                button_CheckData.Enabled = value;
                button_Export.Enabled = value;
            }
        }

        void ResetWorksheetConfig()
        {
            m_SelectedConfigSheet = null;
            m_SelectedWorksheet = null;

            // 清空配置导出设置
            textBox_ClientConfig.Text = string.Empty;
            textBox_ServerConfig.Text = string.Empty;

            // 清空代码导出设置
            textBox_ClassNameCS.Text = string.Empty;
            textBox_ClassNameCPP.Text = string.Empty;
            numericUpDown_Capacity.Value = XSrcExportInfo.DEFAULT_CAPACITY;
            numericUpDown_StepSize.Value = XSrcExportInfo.DEFAULT_STEP_SIZE;
            numericUpDown_KeyCount.Value = XSrcExportInfo.DEFAULT_KEY_COUNT;
            numericUpDown_GroupSize.Value = XSrcExportInfo.DEFAULT_GROUP_SIZE;
            textBox_ClientSrc.Text = string.Empty;
            textBox_ServerSrc.Text = string.Empty;

            // 清空导出类型设置
            checkBox_ExportConfig.Checked = false;
            checkBox_ExportSrc.Checked = false;

            // 禁用操作
            EnableConfigOper = false;
            EnableSourceOper = false;
            EnableFuncOper = false;
        }

        void UpdateMgrConfig()
        {
            numericUpDown_Capacity.Enabled = EnableSourceOper;
            numericUpDown_StepSize.Enabled = EnableSourceOper;
            numericUpDown_KeyCount.Enabled = EnableSourceOper;
            numericUpDown_GroupSize.Enabled = EnableSourceOper;
            if (EnableSourceOper)
            {
                XCfgMgrType mgr = comboBox_ManagerCPP.SelectedItem as XCfgMgrType;
                if (null == mgr || mgr.MgrEnum == ECfgMgrType.None)
                {
                    numericUpDown_Capacity.Enabled = false;
                    numericUpDown_StepSize.Enabled = false;
                    numericUpDown_KeyCount.Enabled = false;
                    numericUpDown_GroupSize.Enabled = false;
                }
                switch (mgr.MgrEnum)
                {
                    case ECfgMgrType.NoKeyList:
                    case ECfgMgrType.OneKeyMgr:
                        numericUpDown_KeyCount.Enabled = false;
                        numericUpDown_GroupSize.Enabled = false;
                        break;
                    case ECfgMgrType.TwoKeyMgr:
                    case ECfgMgrType.OneGroupMgr:
                        numericUpDown_StepSize.Enabled = false;
                        break;
                    default:
                        break;
                }
            }
        }

        void LoadExcel(string sFileName, bool isReload)
        {
            this.ClearAll();

            if (string.IsNullOrEmpty(sFileName))
            {
                return;
            }

            LoadedExcelFile = XExportHistory.GetRelativePath(sFileName);
            m_ExcelLoadTime = DateTime.Now;

            try
            {
				if (null != m_ExcelApp) m_ExcelApp.Quit();
                m_ExcelApp = new Excel.Application();
                object miss = System.Reflection.Missing.Value;
                m_Workbook = (Excel.WorkbookClass)m_ExcelApp.Workbooks.Open(XExportHistory.GetFullPath(LoadedExcelFile),
                    miss, true, miss, miss, miss, true, miss, miss, true, miss, miss, miss, miss, miss); 
            }
            catch (System.Exception)
            {
                m_Workbook = null;
                m_ExcelApp.Quit();
            }

            if (m_Workbook == null)
            {
                Log(ELogType.ERROR, "载入文件失败, 文件名:{0}", LoadedExcelFile);
                return;
            }
            else
            {
                if (isReload)
                {
                    Log(ELogType.WARN, "检测到 Excel 文件更新, 重新载入.");
                }
                Log(ELogType.INFO, "{1} 载入EXCEL文件:{0}", LoadedExcelFile, m_ExcelLoadTime.ToString());
                m_LoadedCheckTimer.Start();
            }

            // 更新界面
            foreach (Excel.Worksheet sheet in m_Workbook.Worksheets)
            {
                listBox_Sheets.Items.Add(sheet.Name);
                m_WorksheetMap.Add(sheet.Name, sheet);
            }
        }

        void OnSelectWorksheet()
        {
            Excel.Worksheet worksheet = getSelectWorksheet();
            if (worksheet == m_SelectedWorksheet)
            {
                return;
            }

            // 清空日志和导出配置
            richTextBox_Log.Clear();
            ResetWorksheetConfig();

            m_SelectedWorksheet = worksheet;
            if (null == m_SelectedWorksheet)
            {
                //--4>TODO:
                Log(ELogType.ERROR, "没有找到配置表");
                return;
            }

            // 加载配置文件
            m_SelectedConfigSheet = XExcelConfig.LoadExcelConfig(worksheet, new LogEventHandler(Log));
            if (null == m_SelectedConfigSheet)
            {
                Log(ELogType.ERROR, "加载配置数据表失败, {0} @ {1}", LoadedExcelFile, worksheet.Name);
                return;
            }

            //--4>TODO: 如果配置里有记录, 将以前的输出记录加载到界面里
            //  根据 excel 的相对路径名和 sheet 名 来判断
            XExportInfo info = m_History.GetConfig(LoadedExcelFile, m_SelectedWorksheet.Name);
            if (null != info)
            {
                textBox_ClassNameCS.Text = info.SourceInfo.CsInfo.ClassName;
                comboBox_ManagerCS.SelectedIndex = comboBox_ManagerCS.Items.IndexOf(info.SourceInfo.CsInfo.MgrType);
                textBox_ClassNameCPP.Text = info.SourceInfo.CppInfo.ClassName;
                comboBox_ManagerCPP.SelectedIndex = comboBox_ManagerCPP.Items.IndexOf(info.SourceInfo.CppInfo.MgrType);
                numericUpDown_Capacity.Value = info.SourceInfo.CppInfo.Capacity;
                numericUpDown_StepSize.Value = info.SourceInfo.CppInfo.StepSize;
                numericUpDown_KeyCount.Value = info.SourceInfo.CppInfo.KeyCount;
                numericUpDown_GroupSize.Value = info.SourceInfo.CppInfo.GroupSize;
                textBox_ClientConfig.Text = XExportHistory.GetFullPath(info.ConfigInfo.ClientConfig);
                textBox_ServerConfig.Text = XExportHistory.GetFullPath(info.ConfigInfo.ServerConfig);
                textBox_ClientSrc.Text = XExportHistory.GetFullPath(info.SourceInfo.CsInfo.FilePath);
                textBox_ServerSrc.Text = XExportHistory.GetFullPath(info.SourceInfo.CppInfo.FilePath);
            }
            else
            {
                textBox_ClassNameCS.Text = "TC" + worksheet.Name;
                comboBox_ManagerCS.SelectedIndex = 0;
				textBox_ClassNameCPP.Text = "TC" + worksheet.Name;
                comboBox_ManagerCPP.SelectedIndex = 0;
                numericUpDown_Capacity.Value = XSrcExportInfo.DEFAULT_CAPACITY;
                numericUpDown_StepSize.Value = XSrcExportInfo.DEFAULT_STEP_SIZE;
                numericUpDown_KeyCount.Value = XSrcExportInfo.DEFAULT_KEY_COUNT;
                numericUpDown_GroupSize.Value = XSrcExportInfo.DEFAULT_GROUP_SIZE;
                textBox_ClientConfig.Text = XExportHistory.GetFullPath(XConfigDefine.DEFAULT_CLIENT_CONFIG_PATH) + "TC" + worksheet.Name + ".txt";
				textBox_ServerConfig.Text = XExportHistory.GetFullPath(XConfigDefine.DEFAULT_SERVER_CONFIG_PATH) + "TC" + worksheet.Name + ".txt";
				textBox_ClientSrc.Text = XExportHistory.GetFullPath(XConfigDefine.DEFAULT_CLIENT_SOURCE_PATH) + "TC" + worksheet.Name + ".cs";
				textBox_ServerSrc.Text = XExportHistory.GetFullPath(XConfigDefine.DEFAULT_SERVER_SOURCE_PATH) + "TC" + worksheet.Name + ".h";
            }

            // 允许功能按钮
            checkBox_ExportConfig.Checked = true;
            EnableFuncOper = true;
        }

        private Excel.Worksheet getSelectWorksheet()
        {
            string sSheetName = listBox_Sheets.SelectedItem as string;
            if (null != sSheetName && m_WorksheetMap.Contains(sSheetName))
            {
                return m_WorksheetMap[sSheetName] as Excel.Worksheet;
            }
            return null;
        }

        public void Log(ELogType t, string format, params object[] args)
        {
            string strData = string.Format(format, args);
            string strAppend = string.Format("[{0}] {1}", t.ToString(), strData + Environment.NewLine);
            int oldLen = richTextBox_Log.Text.Length;
            richTextBox_Log.AppendText(strAppend);
            richTextBox_Log.Select(oldLen, strAppend.Length);
            switch (t)
            {
                case ELogType.INFO:
                case ELogType.DEBUG:
                    richTextBox_Log.SelectionColor = Color.Black;
                    break;
                case ELogType.ERROR:
                case ELogType.FATAL:
                    richTextBox_Log.SelectionColor = Color.Red;
                    break;
                case ELogType.WARN:
                    richTextBox_Log.SelectionColor = Color.Blue;
                    break;
                default:
                    break;
            }
            richTextBox_Log.Select(richTextBox_Log.Text.Length, 0);
            richTextBox_Log.SelectionColor = Color.Black;
        }

        #endregion

        private void button_OpenExcel_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("excel.exe", LoadedExcelFile);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (null != m_History)
            {
                if (false == m_History.Save())
                {
                    Log(ELogType.WARN, "配置文件更新失败");
                }
            }
            this.ClearAll();
        }

        private void listBox_Sheets_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.OnSelectWorksheet();
        }

        private void button_Export_Click(object sender, EventArgs e)
        {
            if (m_SelectedConfigSheet != null)
            {
                richTextBox_Log.Clear();
                XCfgExportInfo cfgInfo = new XCfgExportInfo(textBox_ClientConfig.Text, textBox_ServerConfig.Text);
                XCsSrcInfo csInfo = new XCsSrcInfo(textBox_ClassNameCS.Text,
                    comboBox_ManagerCS.SelectedItem as XCfgMgrType, textBox_ClientSrc.Text);
                XCppSrcInfo cppInfo = new XCppSrcInfo(textBox_ClassNameCPP.Text,
                    comboBox_ManagerCPP.SelectedItem as XCfgMgrType, textBox_ServerSrc.Text,
                    (uint)numericUpDown_Capacity.Value, (uint)numericUpDown_StepSize.Value,
                    (uint)numericUpDown_KeyCount.Value, (uint)numericUpDown_GroupSize.Value);
                XSrcExportInfo srcInfo = new XSrcExportInfo(csInfo, cppInfo);
                    
                // 导出信息并保存配置信息
                m_History.AddHistory(new XExportInfo(LoadedExcelFile, m_SelectedWorksheet.Name, cfgInfo, srcInfo));

                if (m_SelectedConfigSheet.DoExport(cfgInfo, srcInfo, checkBox_ExportConfig.Checked, checkBox_ExportSrc.Checked))
                {
                    if (false == m_History.Save())
                    {
                        Log(ELogType.WARN, "配置文件更新失败");
                    }
                }
            }
        }

        private void button_CheckData_Click(object sender, EventArgs e)
        {
            if (m_SelectedConfigSheet != null)
            {
                richTextBox_Log.Clear();
				m_SelectedConfigSheet.DoCheck((comboBox_ManagerCS.SelectedItem as XCfgMgrType).MgrEnum,
					(comboBox_ManagerCPP.SelectedItem as XCfgMgrType).MgrEnum);
            }
        }

        private void checkBox_ExportConfig_CheckedChanged(object sender, EventArgs e)
        {
            EnableConfigOper = checkBox_ExportConfig.Checked;;
        }

        private void checkBox_ExportSrc_CheckedChanged(object sender, EventArgs e)
        {
            EnableSourceOper = checkBox_ExportSrc.Checked;
        }

        private void comboBox_ExcelList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender == comboBox_ExcelList)
            {
                string sSelectedFile = comboBox_ExcelList.SelectedItem as string;
                if (XDefine.OPEN_NEW_EXCEL == sSelectedFile)
                {
                    OpenFileDialog dialog = new OpenFileDialog();
                    dialog.Filter = XDefine.EXCEL_FILTER;
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        string sFileName = XExportHistory.GetFullPath(dialog.FileName);

                        if (comboBox_ExcelList.Items.Contains(sFileName) == false)
                        {
                            comboBox_ExcelList.Items.Add(sFileName);
                        }
                        comboBox_ExcelList.Text = sFileName;

                        this.LoadExcel(sFileName, false);
                    }
                }
                else
                {
                    if (sSelectedFile != XExportHistory.GetFullPath(LoadedExcelFile))
                    {
                        this.LoadExcel(sSelectedFile, false);
                    }
                }
            }
        }

        private void updateTimer_Tick(object sender, EventArgs e)
        {
            if (File.Exists(LoadedExcelFile) == false)
            {
                return;
            }
            if (File.GetLastWriteTime(LoadedExcelFile) > m_ExcelLoadTime)
            {
                m_LoadedCheckTimer.Stop();
                LoadExcel(LoadedExcelFile, true);
            }
        }

        private void comboBox_ManagerCPP_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender == comboBox_ManagerCPP)
            {
                UpdateMgrConfig();
            }
        }
    }
}
