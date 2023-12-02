using System;
using System.Collections.Generic;

public  class ProtoManager:Singleton<ProtoManager>
{
    private  readonly Dictionary<int, Type> _protoIdToType;
    private  readonly Dictionary<Type, int> _typeToProtoId;

    public ProtoManager()
    {
        _protoIdToType = new Dictionary<int, Type>();
        _typeToProtoId = new Dictionary<Type, int>();

        // 在此处注册所有消息类型和它们对应的protoId
        RegisterProto<CSBoneRequest>(ProtosMsgID.CSBoneRequest);
        // 注册其他消息类型...
    }

     public override void Initialize()
     {
         base.Initialize();
     }

     public override void Update(float time)
    {
        base.Update( time);
    }

     public override void Destroy()
     {
         base.Destroy();
     }

     public  void RegisterProto<T>(int protoId)
    {
        _protoIdToType[protoId] = typeof(T);
        _typeToProtoId[typeof(T)] = protoId;
    }

    public  Type GetTypeByProtoId(int protoId)
    {
        _protoIdToType.TryGetValue(protoId, out Type type);
        return type;
    }

    public  int GetProtoIdByType(Type type)
    {
        _typeToProtoId.TryGetValue(type, out int protoId);
        return protoId;
    }
}
