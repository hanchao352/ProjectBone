using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public  class ProtoManager:SingletonManager<ProtoManager>,IGeneric
{
    private  readonly Dictionary<int, Type> _protoIdToType;
    private  readonly Dictionary<Type, int> _typeToProtoId;

    public ProtoManager()
    {
        _protoIdToType = new Dictionary<int, Type>();
        _typeToProtoId = new Dictionary<Type, int>();

        // 在此处注册所有消息类型和它们对应的protoId
        RegisterAllProtos();
        // 注册其他消息类型...
    }
    private void RegisterAllProtos()
    {
        var protoIds = typeof(ProtosMsgID).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(int));

        foreach (var protoIdField in protoIds)
        {
            int protoId = (int)protoIdField.GetValue(null);
            
            string messageTypeName = protoIdField.Name;
            Type messageType = Type.GetType(messageTypeName); 

            if (messageType != null)
            {
                RegisterProto(messageType, protoId);
            }
        }
    }
    private void RegisterProto(Type messageType, int protoId)
    {
        _protoIdToType[protoId] = messageType;
        _typeToProtoId[messageType] = protoId;
    }
     public override void Initialize()
     {
         base.Initialize();
     }

     public override void Update(float time)
    {
        base.Update( time);
    }

     public override void Dispose()
     {
         base.Dispose();
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
