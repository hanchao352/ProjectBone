using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Protobuf;
using UnityEngine.Experimental.GlobalIllumination;

public abstract class ModBase 
{
    private readonly Dictionary<int, Func<IMessage, Task>> _callbacks = new Dictionary<int, Func<IMessage, Task>>();
    private  Dictionary<Type, Dictionary<int, Func<IMessage, Task>>> _messageIdToCallback;
    bool _isInitialized = false;

    public bool IsInitialized => _isInitialized;

    public void Use()
    {
    }

    public virtual void Initialize()
    {
        _isInitialized = true;
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
    // protected void RegisterWebSocketCallback<T>(Action<T> callback) where T : IMessage<T>
    // {
    //     int protoId = ProtoManager.Instance.GetProtoIdByType(typeof(T));
    //     ModManager.Instance.RegisterWebSocketCallback(protoId, callback as Action<IMessage>);
    // }

    protected void UnregisterWebSocketCallback<T>() where T : IMessage<T>
    {
        int protoId = ProtoManager.Instance.GetProtoIdByType(typeof(T));
        ModManager.Instance.UnregisterWebSocketCallback(protoId);
    }
    
    
    // protected void RegisterWebRequestCallback<T>(Action<T> callback) where T : IMessage<T>
    // {
    //     int protoId = ProtoManager.Instance.GetProtoIdByType(typeof(T));
    //     ModManager.Instance.RegisterWebSocketCallback(protoId, callback as Action<IMessage>);
    // }

    protected void UnregisterWebRequestCallback<T>() where T : IMessage<T>
    {
        int protoId = ProtoManager.Instance.GetProtoIdByType(typeof(T));
        ModManager.Instance.UnregisterWebSocketCallback(protoId);
    }
    
    public async Task SendWebMessageAsync<T>(T message) where T : IMessage
    {
        int protoId = ProtoManager.Instance.GetProtoIdByType(typeof(T));
        await WebSocketClient.Instance.SendMessageAsync(message, protoId);
    }
    public void SendWebRequestMessageAsync<T>(T message) where T : IMessage
    {
        int protoId = ProtoManager.Instance.GetProtoIdByType(typeof(T));
         WebRequestManager.Instance.SendMessageAsync(message);
    }
   
}
