using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.Collections;

namespace ExcelConfigExport
{
    class XExportHistory
    {
        private SortedList<string, SortedList<string, XExportInfo>> m_AllHistory;

        public XExportHistory() : this(XConfigDefine.EXPORT_HISTORY_FILE) { }

        public XExportHistory(string sHistoryFile)
        {
            m_AllHistory = new SortedList<string, SortedList<string, XExportInfo>>();
            this.Load(sHistoryFile);
        }

        internal static string GetRelativePath(string basePath, string targetPath)
        {
            if (string.IsNullOrEmpty(basePath) || string.IsNullOrEmpty(targetPath))
            {
                return string.Empty;
            }
            if (Path.IsPathRooted(targetPath) == false)
            {
                return targetPath;
            }
            try
            {
                Uri baseUri = new Uri(basePath);
                Uri targetUri = new Uri(targetPath);
                return Uri.UnescapeDataString(baseUri.MakeRelativeUri(targetUri).ToString());
            }
            catch (System.Exception)
            {
                return string.Empty;
            }
        }

        internal static string GetRelativePath(string sPath)
        {
            if (string.IsNullOrEmpty(sPath))
            {
                return string.Empty;
            }
            if (File.Exists(sPath) || Directory.Exists(sPath))
            {
                if (Path.IsPathRooted(sPath))
                {
                    return GetRelativePath(System.AppDomain.CurrentDomain.BaseDirectory, sPath);
                }
            }
            return sPath;
        }

        internal static string GetFullPath(string sPath)
        {
            if (string.IsNullOrEmpty(sPath))
            {
                return string.Empty;
            }
            if (File.Exists(sPath) || Directory.Exists(sPath))
            {
                if (Path.IsPathRooted(sPath) == false)
                {
                    return Path.GetFullPath(sPath);
                }
            }
            return sPath;
        }

        internal object[] GetAllFile()
        {
            ArrayList fileList = new ArrayList(m_AllHistory.Keys.Count);
            foreach (string str in m_AllHistory.Keys)
            {
                fileList.Add(GetFullPath(str));
            }
            return fileList.ToArray();
        }

        internal bool Load(string sFileName)
        {
            m_AllHistory.Clear();

            if (string.IsNullOrEmpty(sFileName))
            {
                return false;
            }

            try
            {
    			XmlDocument doc = new XmlDocument();
                doc.Load(sFileName);

                XmlNode root = doc.SelectSingleNode("ExcelConfigExport");
                XmlNodeList configList = root.SelectNodes("ConfigItem");
    			
    			foreach(XmlNode itemNode in configList)
    			{
                    string sExcelFile = itemNode.Attributes["ExcelFile"].Value;
                    if (m_AllHistory.ContainsKey(sExcelFile) == false)
                    {
                        m_AllHistory.Add(sExcelFile, new SortedList<string , XExportInfo>());
                    }
                    string sSheetName = itemNode.Attributes["SheetName"].Value;
                    if (m_AllHistory[sExcelFile].ContainsKey(sSheetName) == false)
                    {
                        m_AllHistory[sExcelFile].Add(sSheetName, null);
                    }

                    XmlElement infoElement = itemNode["ConfigInfo"];
                    string sClientConfig = infoElement["Client"].InnerText;
                    string sServerConfig = infoElement["Server"].InnerText;

                    string sClassName = string.Empty;
                    string sSrcFile = string.Empty;
                    XCfgMgrType mgr = XConfigDefine.ALL_MGR_TYPE[0];

                    infoElement = itemNode["SourceInfo"]["CS"];
                    sClassName = infoElement.Attributes["ClassName"].Value;
                    sSrcFile = infoElement["FilePath"].InnerText;
                    if (infoElement.Attributes.GetNamedItem("Manager") != null)
                    {
                        mgr = XConfigDefine.GetMgrType(infoElement.Attributes["Manager"].Value);
                    }
                    else
                    {
                        mgr = XConfigDefine.ALL_MGR_TYPE[0];
                    }
                    XCsSrcInfo csInfo = new XCsSrcInfo(sClassName, mgr, sSrcFile);

                    infoElement = itemNode["SourceInfo"]["CPP"];
                    sClassName = infoElement.Attributes["ClassName"].Value;
                    sSrcFile = infoElement["FilePath"].InnerText;
                    if (infoElement.Attributes.GetNamedItem("Manager") != null)
                    {
                        mgr = XConfigDefine.GetMgrType(infoElement.Attributes["Manager"].Value);
                    }
                    else
                    {
                        mgr = XConfigDefine.ALL_MGR_TYPE[0];
                    }

                    uint cap = XSrcExportInfo.DEFAULT_CAPACITY;
                    if (infoElement.Attributes.GetNamedItem("Capacity") != null)
                    {
                        cap = uint.Parse(infoElement.Attributes["Capacity"].Value);
                    }
                    uint step = XSrcExportInfo.DEFAULT_STEP_SIZE;
                    if (infoElement.Attributes.GetNamedItem("StepSize") != null)
                    {
                        step = uint.Parse(infoElement.Attributes["StepSize"].Value);
                    }
                    uint keyCount = XSrcExportInfo.DEFAULT_KEY_COUNT;
                    if (infoElement.Attributes.GetNamedItem("KeyCount") != null)
                    {
                        keyCount = uint.Parse(infoElement.Attributes["KeyCount"].Value);
                    }
                    uint groupSize = XSrcExportInfo.DEFAULT_GROUP_SIZE;
                    if (infoElement.Attributes.GetNamedItem("GroupSize") != null)
                    {
                        groupSize = uint.Parse(infoElement.Attributes["GroupSize"].Value);
                    }
                    XCppSrcInfo cppInfo = new XCppSrcInfo(sClassName, mgr, sSrcFile, cap, step, keyCount, groupSize);

                    m_AllHistory[sExcelFile][sSheetName] = new XExportInfo(sExcelFile, sSheetName,
                        new XCfgExportInfo(sClientConfig, sServerConfig), new XSrcExportInfo(csInfo, cppInfo));
    			}
            }
            catch (System.Exception)
            {
                return false;
            }
            return true;
        }

        internal bool Save()
        {
            return Save(XConfigDefine.EXPORT_HISTORY_FILE);
        }

        internal bool Save(string sFileName)
        {
            if (string.IsNullOrEmpty(sFileName))
            {
                return false;
            }
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.Encoding = System.Text.Encoding.UTF8;

            try
            {
                XmlWriter xml = XmlWriter.Create(sFileName, settings);
                xml.WriteStartElement("ExcelConfigExport");
                foreach (string sExcelFile in m_AllHistory.Keys)
                {
                    foreach (string sSheetName in m_AllHistory[sExcelFile].Keys)
                    {
                        XExportInfo info = m_AllHistory[sExcelFile][sSheetName];
                        if (null == info)
                        {
                            continue;
                        }
                        xml.WriteStartElement("ConfigItem");
                            xml.WriteAttributeString("ExcelFile", GetRelativePath(sExcelFile));
                            xml.WriteAttributeString("SheetName", sSheetName);

                            xml.WriteStartElement("ConfigInfo");
                                xml.WriteElementString("Client", GetRelativePath(info.ConfigInfo.ClientConfig));
                                xml.WriteElementString("Server", GetRelativePath(info.ConfigInfo.ServerConfig));
                            xml.WriteEndElement();

                            xml.WriteStartElement("SourceInfo");
                                xml.WriteStartElement("CS");
                                    xml.WriteAttributeString("ClassName", info.SourceInfo.CsInfo.ClassName);
                                    xml.WriteAttributeString("Manager", info.SourceInfo.CsInfo.MgrType.MgrEnum.ToString());
                                    xml.WriteElementString("FilePath", GetRelativePath(info.SourceInfo.CsInfo.FilePath));
                                xml.WriteEndElement();
                                xml.WriteStartElement("CPP");
                                    xml.WriteAttributeString("ClassName", info.SourceInfo.CppInfo.ClassName);
                                    xml.WriteAttributeString("Manager", info.SourceInfo.CppInfo.MgrType.MgrEnum.ToString());
                                    xml.WriteAttributeString("Capacity", info.SourceInfo.CppInfo.Capacity.ToString());
                                    xml.WriteAttributeString("StepSize", info.SourceInfo.CppInfo.StepSize.ToString());
                                    xml.WriteAttributeString("KeyCount", info.SourceInfo.CppInfo.KeyCount.ToString());
                                    xml.WriteAttributeString("GroupSize", info.SourceInfo.CppInfo.GroupSize.ToString());
                                    xml.WriteElementString("FilePath", GetRelativePath(info.SourceInfo.CppInfo.FilePath));
                                xml.WriteEndElement();
                            xml.WriteEndElement();
                        xml.WriteEndElement();
                    }
                }
                xml.WriteEndElement();
                xml.Close();
            }
            catch (System.Exception)
            {
                return false;
            }
            return true;
        }

        internal bool AddHistory(XExportInfo info)
        {
            if (string.IsNullOrEmpty(info.ExcelFile) || string.IsNullOrEmpty(info.SheetName))
            {
                return false;
            }
            if (null == info)
            {
                return false;
            }
            if (m_AllHistory.ContainsKey(info.ExcelFile) == false)
            {
                m_AllHistory.Add(info.ExcelFile, new SortedList<string, XExportInfo>());
            }
            if (m_AllHistory[info.ExcelFile].ContainsKey(info.SheetName) == false)
            {
                m_AllHistory[info.ExcelFile].Add(info.SheetName, null);
            }
            m_AllHistory[info.ExcelFile][info.SheetName] = info;
            return true;
        }

        internal XExportInfo GetConfig(string sExcelFile, string sSheetName)
        {
            if (m_AllHistory.ContainsKey(sExcelFile) == false)
            {
                return null;
            }
            if (m_AllHistory[sExcelFile].ContainsKey(sSheetName) == false)
            {
                return null;
            }
            return m_AllHistory[sExcelFile][sSheetName];
        }
    }
}
