#! /usr/bin/env python
#coding=utf-8
import sys
import os
import glob
import xlrd
import struct
import pickle, CSharpCodeGenerator
import re
reload(sys)
sys.setdefaultencoding('utf-8')
def addBytes(ba, bs):
    for b in bs:
        ba.append(b)
def IntToBytes(value):
    v = int(value)
    bl = struct.pack('i', v);
    return bl
def FloatToBytes(value):
    v = float(value)
    bl = struct.pack('f', v);
    return bl
def ParserEnum(type, value):
    s = re.search(r'\((.*?)\)',type).group(1)
    ss =str.split(str(s), ',')
    index = 0
    for e in ss:
        if e == value:
            return index
        index+=1
def ParseCell(ba, type, value):
    if(value == ''):
        v = struct.pack('?', 0)
        addBytes(ba, v)
    else:
        v = struct.pack('?', 1)
        addBytes(ba, v)
        if type == 'int32' or type == 'int':
            bl = IntToBytes(value)
            addBytes(ba, bl);
        elif type == 'float':
            bl = FloatToBytes(value)
            addBytes(ba, bl);
        elif type == 'string':
            s = str(value)
            bl = s.encode()
            lbl = struct.pack('i', len(bl));
            addBytes(ba, lbl);
            addBytes(ba, bl)
        elif type == 'int32[]' or type == 'int[]':
            l = str.split(str(value), ',')
            lenl = IntToBytes(len(l))
            addBytes(ba, lenl)
            for n in l:
                bl = IntToBytes(n)
                addBytes(ba, bl)
        elif type == 'float[]':
            l = str.split(str(value), ',')
            lenl = IntToBytes(len(l))
            addBytes(ba, lenl)
            for n in l:
                bl = FloatToBytes(n)
                addBytes(ba, bl)
        else:
            v = ParserEnum(type, value)
            bl = IntToBytes(v)
            addBytes(ba, bl);
xlslist = glob.glob("xls/*.xls")
for xls in xlslist :
    workbook = xlrd.open_workbook(xls)
    for sheet in workbook._sheet_list:
        ba = bytearray()
        posiAndtypes = {}
        fieldNameList = []
        firstIndex = -1
        index = 0
        for firstCell in sheet.row_values(0):
            if firstCell != '':
                if(firstIndex == -1):
                    firstIndex = index
                posiAndtypes[index] = firstCell
            index+=1
        fieldNameCells = sheet.row_values(1)
        for fieldIndex in posiAndtypes.keys():
            cellStr = fieldNameCells[fieldIndex]
            fieldNameList.append(cellStr)
        CSharpCodeGenerator.CodeGenerate(sheet.name,posiAndtypes.values(), fieldNameList )
        rowCount = len(sheet.col_values(0))
        for i in range(3, rowCount):
            rowlist = sheet.row_values(i)
            if rowlist[firstIndex] == '':
                continue
            for key, value in posiAndtypes.iteritems():
                s = sheet.row_values(i, key)
                ParseCell(ba, value, rowlist[key])
        f = open("../../WarClash/Assets/RequiredResources/TextConfigs/Config/"+sheet.name.lower()+'.bytes', 'wb')
        b = buffer(ba)
        f.write(b)
        f.close()
            # colCount = len(sheet.row_values(0))
            #print colCount

   

