python xls_deploy_tool.py BuildingConf xls/Building.xls
protoc BuildingConf.proto --descriptor_set_out=BuildingConf.protodesc
tools\ProtoGen\protogen -i:BuildingConf.protodesc -o:../../WarClash/Assets/Logic/Config/BuildingConf.cs
del BuildingConf.proto
del BuildingConf.protodesc
del BuildingConf.txt
del BuildingConf_pb2.py
del BuildingConf_pb2.pyc