python xls_deploy_tool.py ArmyConf xls\Army.xls
protoc ArmyConf.proto --descriptor_set_out=ArmyConf.protodesc
tools\ProtoGen\protogen -i:ArmyConf.protodesc -o:../../WarClash/Assets/Logic/Config/ArmyConf.cs
del ArmyConf.proto
del ArmyConf.protodesc
del ArmyConf.txt
del ArmyConf_pb2.py
del ArmyConf_pb2.pyc
pause