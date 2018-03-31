#! /usr/bin/env python
#coding=utf-8
import sys
import os
import glob
import xlrd, re
def getFieldType( type):
    if type == 'int32':
        return "int"
    elif type == 'float':
        return "float"
    elif type == 'string':
        return "string"
    elif type == 'int32[]' or type == 'int[]':
         return "int[]"
    else:
        ss = str.split(str(type), '(')
        ss[0][0].upper()
        return ss[0]
def getFieldParseFunc( type):
    if type == 'int32':
        return "ToInt"
    elif type == 'float':
        return "ToFloat"
    elif type == 'string':
        return "ToString"
    elif type == 'int32[]' or type == 'int[]':
        return "ToIntArray"
    else:
        f = getFieldType( type)
        return "To"+f
def isEnum(type):
    if type == 'int32':
        return False
    elif type == 'float':
        return False
    elif type == 'string':
        return False
    elif type == 'int32[]' or type == 'int[]':
         return False
    else:
        return True
def GetEnumList(type):
    l = []
    s = re.search(r'\((.*?)\)',type).group(1)
    ss =str.split(str(s), ',')
    for e in ss:
        l.append(e)
    return l
def CodeGenerate(sheetName, fieldTypeList, fieldNameList):
    text = "///Code Generated Automatically, it would be dangerous if code changed\n"
    text += "using System;\n"
    text += "using System.Text;\n"
    text += "using UnityEngine;\n"
    text += "using System.Collections.Generic;\n"
    text += "public class "+sheetName+" : BaseConfig\n"
    text += "{"+"\n"
    fieldIndex = 0;
    for field in fieldTypeList:
        fieldType = getFieldType(field)
        if(isEnum(field)):
            text += "   public enum "+ fieldType+"\n"
            text += "   {"+"\n"
            el = GetEnumList(field)
            for e in el:
                 text += "     "+e+","+"\n"
            text += "   }"+"\n"
            text += "   public "+ fieldType+" To"+fieldType+"(byte[] bytes, ref int startIndex)\n"
            text += "   {"+"\n"
            text += "       if (bytes[startIndex] == 0)"+"\n"
            text += "       {"+"\n"
            text += "           return 0;"+"\n"
            text += "       }"+"\n"
            text += "       startIndex++;"+"\n"
            text += "       var v = BitConverter.ToInt32(bytes, startIndex);"+"\n"
            text += "       startIndex += 4;"+"\n"
            text += "       return ("+fieldType+")v;"+"\n"
            text += "   }"+"\n"
        text += "   public "+fieldType+" "+fieldNameList[fieldIndex]+";"+"\n"
        fieldIndex+=1
    text += "   public void Desearize(byte[] bytes, ref int startIndex)"+"\n"
    text += "   {"+"\n"
    fieldIndex = 0;
    for field in fieldTypeList:
        text +=  "      "+fieldNameList[fieldIndex]+" = "+getFieldParseFunc(field)+"(bytes, ref startIndex);"+"\n"
        fieldIndex+=1
    text += "   }"+"\n"
    text += "   private static void Deserialize(byte[] bytes)\n"
    text += "   {"+"\n"
    text += "       int startIndex = 0;\n"
    text += "       while (startIndex<bytes.Length)\n"
    text += "       {\n"
    text += "           var conf = new "+sheetName+"();\n"
    text += "           conf.Desearize(bytes, ref startIndex);\n"
    text += "           Configs.Add(conf.Id, conf);\n"
    text += "       }\n"
    text += "   }\n"
    text += "   public static void Init()\n"
    text += "   {"+"\n"
    text += "       var txt = AssetResources.LoadAssetImmediatly"+r'("'+sheetName.lower()+""".bytes" """+") as TextAsset;\n"
    text += "       Deserialize(txt.bytes);\n"
    text += "   }\n"
    key = fieldTypeList[0]
    keytype = getFieldType(key)
    text += "   public static "+sheetName+" Get("+keytype+" id)\n"
    text += "   {"+"\n"
    text += "       "+sheetName+" conf;\n"
    text += "       if(Configs.TryGetValue(id, out conf))\n"
    text += "       {"+"\n"
    text += "           return conf;\n"
    text += "       }"+"\n"
    text += "       Debug.LogError(id + "r'"'+ " Not Exsit In "+sheetName+r'"'+");\n"
    text += "       return null;\n"
    text += "   }"+"\n"
    text +="    private static Dictionary<"+keytype+", "+sheetName+"> Configs = new Dictionary<"+keytype+", "+sheetName+">();\n"
    text += "}\n"
    f = open("../../WarClash/Assets/Logic/Config/"+sheetName+'.cs', 'wb')
    f.write(text)
    f.close()
   

