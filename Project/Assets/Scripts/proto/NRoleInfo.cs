// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: NRoleInfo.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021, 8981
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
/// <summary>Holder for reflection information generated from NRoleInfo.proto</summary>
public static partial class NRoleInfoReflection {

  #region Descriptor
  /// <summary>File descriptor for NRoleInfo.proto</summary>
  public static pbr::FileDescriptor Descriptor {
    get { return descriptor; }
  }
  private static pbr::FileDescriptor descriptor;

  static NRoleInfoReflection() {
    byte[] descriptorData = global::System.Convert.FromBase64String(
        string.Concat(
          "Cg9OUm9sZUluZm8ucHJvdG8aEU9wdGlvbk1zZ0lkLnByb3RvGg1OUmVzdWx0",
          "LnByb3RvGg5HYW1lUmFjZS5wcm90byJ3CglOUm9sZUluZm8SDAoEbmFtZRgB",
          "IAEoCRINCgVsZXZlbBgCIAEoBRIOCgZyb2xlSWQYAyABKAMSEwoLQmlnU2Vy",
          "dmVySWQYBCABKAUSFQoNU21hbGxTZXJ2ZXJJZBgFIAEoBRIRCglhY2NvdW50",
          "aWQYBiABKAliBnByb3RvMw=="));
    descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
        new pbr::FileDescriptor[] { global::OptionMsgIdReflection.Descriptor, global::NResultReflection.Descriptor, global::GameRaceReflection.Descriptor, },
        new pbr::GeneratedClrTypeInfo(null, null, new pbr::GeneratedClrTypeInfo[] {
          new pbr::GeneratedClrTypeInfo(typeof(global::NRoleInfo), global::NRoleInfo.Parser, new[]{ "Name", "Level", "RoleId", "BigServerId", "SmallServerId", "Accountid" }, null, null, null, null)
        }));
  }
  #endregion

}
#region Messages
public sealed partial class NRoleInfo : pb::IMessage<NRoleInfo>
#if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    , pb::IBufferMessage
#endif
{
  private static readonly pb::MessageParser<NRoleInfo> _parser = new pb::MessageParser<NRoleInfo>(() => new NRoleInfo());
  private pb::UnknownFieldSet _unknownFields;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public static pb::MessageParser<NRoleInfo> Parser { get { return _parser; } }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public static pbr::MessageDescriptor Descriptor {
    get { return global::NRoleInfoReflection.Descriptor.MessageTypes[0]; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  pbr::MessageDescriptor pb::IMessage.Descriptor {
    get { return Descriptor; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public NRoleInfo() {
    OnConstruction();
  }

  partial void OnConstruction();

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public NRoleInfo(NRoleInfo other) : this() {
    name_ = other.name_;
    level_ = other.level_;
    roleId_ = other.roleId_;
    bigServerId_ = other.bigServerId_;
    smallServerId_ = other.smallServerId_;
    accountid_ = other.accountid_;
    _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public NRoleInfo Clone() {
    return new NRoleInfo(this);
  }

  /// <summary>Field number for the "name" field.</summary>
  public const int NameFieldNumber = 1;
  private string name_ = "";
  /// <summary>
  ///名字
  /// </summary>
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public string Name {
    get { return name_; }
    set {
      name_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
    }
  }

  /// <summary>Field number for the "level" field.</summary>
  public const int LevelFieldNumber = 2;
  private int level_;
  /// <summary>
  ///等级
  /// </summary>
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public int Level {
    get { return level_; }
    set {
      level_ = value;
    }
  }

  /// <summary>Field number for the "roleId" field.</summary>
  public const int RoleIdFieldNumber = 3;
  private long roleId_;
  /// <summary>
  ///角色id
  /// </summary>
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public long RoleId {
    get { return roleId_; }
    set {
      roleId_ = value;
    }
  }

  /// <summary>Field number for the "BigServerId" field.</summary>
  public const int BigServerIdFieldNumber = 4;
  private int bigServerId_;
  /// <summary>
  ///大服id
  /// </summary>
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public int BigServerId {
    get { return bigServerId_; }
    set {
      bigServerId_ = value;
    }
  }

  /// <summary>Field number for the "SmallServerId" field.</summary>
  public const int SmallServerIdFieldNumber = 5;
  private int smallServerId_;
  /// <summary>
  ///小服id
  /// </summary>
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public int SmallServerId {
    get { return smallServerId_; }
    set {
      smallServerId_ = value;
    }
  }

  /// <summary>Field number for the "accountid" field.</summary>
  public const int AccountidFieldNumber = 6;
  private string accountid_ = "";
  /// <summary>
  ///账号id
  /// </summary>
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public string Accountid {
    get { return accountid_; }
    set {
      accountid_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
    }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public override bool Equals(object other) {
    return Equals(other as NRoleInfo);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public bool Equals(NRoleInfo other) {
    if (ReferenceEquals(other, null)) {
      return false;
    }
    if (ReferenceEquals(other, this)) {
      return true;
    }
    if (Name != other.Name) return false;
    if (Level != other.Level) return false;
    if (RoleId != other.RoleId) return false;
    if (BigServerId != other.BigServerId) return false;
    if (SmallServerId != other.SmallServerId) return false;
    if (Accountid != other.Accountid) return false;
    return Equals(_unknownFields, other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public override int GetHashCode() {
    int hash = 1;
    if (Name.Length != 0) hash ^= Name.GetHashCode();
    if (Level != 0) hash ^= Level.GetHashCode();
    if (RoleId != 0L) hash ^= RoleId.GetHashCode();
    if (BigServerId != 0) hash ^= BigServerId.GetHashCode();
    if (SmallServerId != 0) hash ^= SmallServerId.GetHashCode();
    if (Accountid.Length != 0) hash ^= Accountid.GetHashCode();
    if (_unknownFields != null) {
      hash ^= _unknownFields.GetHashCode();
    }
    return hash;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public override string ToString() {
    return pb::JsonFormatter.ToDiagnosticString(this);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public void WriteTo(pb::CodedOutputStream output) {
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    output.WriteRawMessage(this);
  #else
    if (Name.Length != 0) {
      output.WriteRawTag(10);
      output.WriteString(Name);
    }
    if (Level != 0) {
      output.WriteRawTag(16);
      output.WriteInt32(Level);
    }
    if (RoleId != 0L) {
      output.WriteRawTag(24);
      output.WriteInt64(RoleId);
    }
    if (BigServerId != 0) {
      output.WriteRawTag(32);
      output.WriteInt32(BigServerId);
    }
    if (SmallServerId != 0) {
      output.WriteRawTag(40);
      output.WriteInt32(SmallServerId);
    }
    if (Accountid.Length != 0) {
      output.WriteRawTag(50);
      output.WriteString(Accountid);
    }
    if (_unknownFields != null) {
      _unknownFields.WriteTo(output);
    }
  #endif
  }

  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output) {
    if (Name.Length != 0) {
      output.WriteRawTag(10);
      output.WriteString(Name);
    }
    if (Level != 0) {
      output.WriteRawTag(16);
      output.WriteInt32(Level);
    }
    if (RoleId != 0L) {
      output.WriteRawTag(24);
      output.WriteInt64(RoleId);
    }
    if (BigServerId != 0) {
      output.WriteRawTag(32);
      output.WriteInt32(BigServerId);
    }
    if (SmallServerId != 0) {
      output.WriteRawTag(40);
      output.WriteInt32(SmallServerId);
    }
    if (Accountid.Length != 0) {
      output.WriteRawTag(50);
      output.WriteString(Accountid);
    }
    if (_unknownFields != null) {
      _unknownFields.WriteTo(ref output);
    }
  }
  #endif

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public int CalculateSize() {
    int size = 0;
    if (Name.Length != 0) {
      size += 1 + pb::CodedOutputStream.ComputeStringSize(Name);
    }
    if (Level != 0) {
      size += 1 + pb::CodedOutputStream.ComputeInt32Size(Level);
    }
    if (RoleId != 0L) {
      size += 1 + pb::CodedOutputStream.ComputeInt64Size(RoleId);
    }
    if (BigServerId != 0) {
      size += 1 + pb::CodedOutputStream.ComputeInt32Size(BigServerId);
    }
    if (SmallServerId != 0) {
      size += 1 + pb::CodedOutputStream.ComputeInt32Size(SmallServerId);
    }
    if (Accountid.Length != 0) {
      size += 1 + pb::CodedOutputStream.ComputeStringSize(Accountid);
    }
    if (_unknownFields != null) {
      size += _unknownFields.CalculateSize();
    }
    return size;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public void MergeFrom(NRoleInfo other) {
    if (other == null) {
      return;
    }
    if (other.Name.Length != 0) {
      Name = other.Name;
    }
    if (other.Level != 0) {
      Level = other.Level;
    }
    if (other.RoleId != 0L) {
      RoleId = other.RoleId;
    }
    if (other.BigServerId != 0) {
      BigServerId = other.BigServerId;
    }
    if (other.SmallServerId != 0) {
      SmallServerId = other.SmallServerId;
    }
    if (other.Accountid.Length != 0) {
      Accountid = other.Accountid;
    }
    _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public void MergeFrom(pb::CodedInputStream input) {
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    input.ReadRawMessage(this);
  #else
    uint tag;
    while ((tag = input.ReadTag()) != 0) {
      switch(tag) {
        default:
          _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
          break;
        case 10: {
          Name = input.ReadString();
          break;
        }
        case 16: {
          Level = input.ReadInt32();
          break;
        }
        case 24: {
          RoleId = input.ReadInt64();
          break;
        }
        case 32: {
          BigServerId = input.ReadInt32();
          break;
        }
        case 40: {
          SmallServerId = input.ReadInt32();
          break;
        }
        case 50: {
          Accountid = input.ReadString();
          break;
        }
      }
    }
  #endif
  }

  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  void pb::IBufferMessage.InternalMergeFrom(ref pb::ParseContext input) {
    uint tag;
    while ((tag = input.ReadTag()) != 0) {
      switch(tag) {
        default:
          _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, ref input);
          break;
        case 10: {
          Name = input.ReadString();
          break;
        }
        case 16: {
          Level = input.ReadInt32();
          break;
        }
        case 24: {
          RoleId = input.ReadInt64();
          break;
        }
        case 32: {
          BigServerId = input.ReadInt32();
          break;
        }
        case 40: {
          SmallServerId = input.ReadInt32();
          break;
        }
        case 50: {
          Accountid = input.ReadString();
          break;
        }
      }
    }
  }
  #endif

}

#endregion


#endregion Designer generated code