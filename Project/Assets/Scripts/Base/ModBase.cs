using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Protobuf;
using UnityEngine.Experimental.GlobalIllumination;

public abstract class ModBase 
{
    private readonly Dictionary<int, Func<IMessage, Task>> _callbacks = new Dictionary<int, Func<IMessage, Task>>();
    private  Dictionary<Type, Dictionary<int, Func<IMessage, Task>>> _messageIdToCallback;

    public virtual void Initialize()
    {
        RegisterMessageHandler();
    }

    public virtual void Update()
    {
    }

    public virtual void Dispose()
    {
        UnregisterMessageHandler();
    }

    public abstract void RegisterMessageHandler();
    public abstract void UnregisterMessageHandler();

    protected void RegisterCallback<T>(Func<T, Task> callback) where T : IMessage<T>
    {
        int protoId = ProtoManager.Instance.GetProtoIdByType(typeof(T));
        ModManager.Instance.RegisterCallback(protoId, callback);
    }

    protected void UnregisterCallback<T>() where T : IMessage<T>
    {
        int protoId = ProtoManager.Instance.GetProtoIdByType(typeof(T));
        ModManager.Instance.UnregisterCallback<T>(protoId);
    }

    internal bool TryGetCallback(int protoId, out Func<IMessage, Task> callback)
    {
        return _callbacks.TryGetValue(protoId, out callback);
    }
    
    public async Task SendMessage<T>(T message) where T : IMessage
    {
        int protoId = ProtoManager.Instance.GetProtoIdByType(typeof(T));
        await Client.Instance.SendMessageAsync(message, protoId);
    }
     
    
}
