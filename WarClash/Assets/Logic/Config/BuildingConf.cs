//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: BuildingConf.proto
namespace Config
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"BuildingConf")]
  public partial class BuildingConf : global::ProtoBuf.IExtensible
  {
    public BuildingConf() {}
    
    private int _Id;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"Id", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int Id
    {
      get { return _Id; }
      set { _Id = value; }
    }
    private int _ArmyId = (int)0;
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"ArmyId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue((int)0)]
    public int ArmyId
    {
      get { return _ArmyId; }
      set { _ArmyId = value; }
    }
    private string _ResPath = "";
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"ResPath", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string ResPath
    {
      get { return _ResPath; }
      set { _ResPath = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"BuildingConf_ARRAY")]
  public partial class BuildingConf_ARRAY : global::ProtoBuf.IExtensible
  {
    public BuildingConf_ARRAY() {}
    
    private readonly global::System.Collections.Generic.List<Config.BuildingConf> _items = new global::System.Collections.Generic.List<Config.BuildingConf>();
    [global::ProtoBuf.ProtoMember(1, Name=@"items", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<Config.BuildingConf> items
    {
      get { return _items; }
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
}