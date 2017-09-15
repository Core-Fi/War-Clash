#! /usr/bin/env python
#coding=utf-8
import sys
import os
import glob
import xlrd
if __name__ == '__main__' :
   

    xlslist = glob.glob("xls/*.xls")
    for xls in xlslist :
        workbook = xlrd.open_workbook(xls)
        for sheet in workbook.sheet_names():
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
            configBat.replace("BuildingConf", sheet)
            configBat.replace("xls/Building.xls", xls)
            pb_file = open(sheet+".bat", "w+")
            pb_file.writelines(configBat)
            pb_file.close()
    filelist = (glob.glob("*.bat"))
    for fileInfo in filelist :
        if  fileInfo != "install.bat":
            print fileInfo
            os.system(fileInfo)
            
    sys.stdin.readline()
    # for fileInfo in filelist
    #     os.system(fileInfo)
  

