#! /usr/bin/env python
#coding=utf-8
import sys
import os
import glob
import xlrd
if __name__ == '__main__' :


    xlslist = glob.glob("xls/*.xls")
    configBat = "python xls_deploy_tool.py BuildingConf xls/Building.xls"
    configBat +="\n"
    configBat +="protoc BuildingConf.proto --descriptor_set_out=BuildingConf.protodesc"
    configBat +="\n"
    configBat +="tools\ProtoGen\protogen -i:BuildingConf.protodesc -o:../../WarClash/Assets/Logic/Config/BuildingConf.cs"
    configBat +="\n"
    configBat +="del BuildingConf.proto"
    configBat +="\n"
    configBat +="del BuildingConf.protodesc"
    configBat +="\n"
    configBat +="del BuildingConf.txt"
    configBat +="\n"
    configBat +="del BuildingConf_pb2.py"
    configBat +="\n"
    configBat +="del BuildingConf_pb2.pyc"
    configMangerFile ="using System.Collections.Generic;"
    configMangerFile +="\n" 
    configMangerFile += "using System.IO;"
    configMangerFile +="\n"
    configMangerFile += "using Config;"
    configMangerFile +="\n"
    configMangerFile += "using UnityEngine;"
    configMangerFile +="\n"
    configMangerFile += "namespace Logic.Config"
    configMangerFile +="\n"
    configMangerFile += "{"
    configMangerFile +="\n"
    configMangerFile += "class ConfigMap<T> where T : class "
    configMangerFile +="\n"
    configMangerFile += "{"
    configMangerFile +="\n"
    configMangerFile += "private static readonly Dictionary<int, T> ConfDic = new Dictionary<int, T>();"
    configMangerFile +="\n"
    configMangerFile += "public static T Get(int id)"
    configMangerFile +="\n"
    configMangerFile += "{"
    configMangerFile +="\n"
    configMangerFile += "   T item = null;"
    configMangerFile +="\n"
    configMangerFile += "   if (ConfDic.TryGetValue(id, out item))"
    configMangerFile +="\n"
    configMangerFile += "   {}"
    configMangerFile +="\n"
    configMangerFile += "   else{DLog.LogError(typeof(T)+"+""" "的id" """+"+id+"+""" "不存在" """+");}"
    configMangerFile +="\n"
    configMangerFile += "   return item;"
    configMangerFile +="\n"
    configMangerFile += "}"
    configMangerFile +="\n"
    ConfigFunc = "12"
    ConfigFunc ="public static void Load#()"
    ConfigFunc +="\n"
    ConfigFunc +="{"
    ConfigFunc +="\n"
    ConfigFunc += "    var txtAsset = AssetResources.LoadAssetImmediatly("+""" "*.bytes" """+") as TextAsset;"
    ConfigFunc +="\n"
    ConfigFunc += "   using (MemoryStream ms = new MemoryStream(txtAsset.bytes)){"
    ConfigFunc +="\n"
    ConfigFunc += "   #_ARRAY array = ProtoBuf.Serializer.Deserialize<#_ARRAY>(ms);"
    ConfigFunc +="\n"
    ConfigFunc += "   foreach (var conf in array.items){"
    ConfigFunc +="\n"
    ConfigFunc += "   ConfigMap<#>.ConfDic.Add(conf.Id, conf);}"
    ConfigFunc +="\n"
    ConfigFunc += "   };"
    ConfigFunc +="\n"
    ConfigFunc += "}"
    ConfigFunc +="\n"
    for xls in xlslist :
        workbook = xlrd.open_workbook(xls)
        for sheet in workbook.sheet_names():
            sheetConfigBat = configBat.replace("BuildingConf", sheet)
            sheetConfigBat = sheetConfigBat.replace("xls/Building.xls", xls)
            print configBat
            pb_file = open(sheet+".bat", "w+")
            pb_file.writelines(sheetConfigBat)
            pb_file.close()

            sheetConfigcs = ConfigFunc.replace("#", sheet)
            sheetConfigcs = sheetConfigcs.replace("*", sheet.lower())
           # sheetConfigcs = sheetConfigcs.replace("'", """)
            sheetConfigcs = sheetConfigcs.decode()
            sheetConfigcs = sheetConfigcs.encode('utf-8')
            #sheetConfigcs = unicode(sheetConfigcs, 'utf-8')
            configMangerFile += sheetConfigcs
    configMangerFile +="\n"
    configMangerFile += "}"
    configMangerFile +="\n"
    configMangerFile += "}"
    configMangerFile +="\n"
    cs_file = open("../../WarClash/Assets/Logic/Config/ConfigManager.cs", "w+")
    cs_file.writelines(configMangerFile)
    cs_file.close()
    filelist = (glob.glob("*.bat"))
    for fileInfo in filelist :
        if  fileInfo != "install.bat":
            print fileInfo
            os.system(fileInfo)
    
    for xls in xlslist :
        workbook = xlrd.open_workbook(xls)
        for sheet in workbook.sheet_names():
            pb_file = open(sheet+".bat", "w+")
            sheetConfigBat = configBat.replace("BuildingConf", sheet)
            sheetConfigBat = sheetConfigBat.replace("xls/Building.xls", xls)
            sheetConfigBat+="\n"
            sheetConfigBat+="pause"
            pb_file.writelines(sheetConfigBat)
            pb_file.close()
    print "Config Parse Finish!!!"
    sys.stdin.readline()
    sys.exit
    # for fileInfo in filelist
    #     os.system(fileInfo)
  

