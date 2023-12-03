using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Protobuf;
using UnityEngine;

public  class ModManager :SingletonManager<ModManager>, IGeneric
{
    private  Dictionary<Type, Dictionary<int, Func<IMessage, Task>>> _messageIdToCallback;
    private Dictionary<int, Action<IMessage>> _webSocketCallbacks = new Dictionary<int, Action<IMessage>>();
    private  readonly Dictionary<Type, ModBase> _mods = new Dictionary<Type, ModBase>();
    public ModManager()
    {
        _messageIdToCallback = new Dictionary<Type, Dictionary<int, Func<IMessage, Task>>>();
    }
    public override void Initialize()
    {
       //需要提前实例化的模块
       
       
    }


    public  void RegisterMod(ModBase mod)
    {
        var modType = mod.GetType();
        if (_mods.ContainsKey(modType))
        {
            throw new InvalidOperationException($"Mod {modType} has already been registered.");
            
        }
        else
        {
            Debug.Log($"Registering mod {modType}.");
        }
        
        if (mod.IsInitialized == false)
        {
            mod.Initialize();
            _mods.Add(modType, mod);
        }
       
    }

    public override void Update(float time)
    {
        base.Update(time);
        foreach (var mod in _mods.Values)
        {
            mod.Update();
        }
    }

    public override void Dispose ()
    {
        base.Dispose();
        foreach (var mod in _mods.Values)
        {
            mod.Dispose();
        }

        _mods.Clear();
    }
    public  void RegisterCallback<T>(int protoId, Func<T, Task> callback) where T : IMessage<T>
    {
        Type messageType = typeof(T);
        if (!_messageIdToCallback.ContainsKey(messageType))
        {
            _messageIdToCallback[messageType] = new Dictionary<int, Func<IMessage, Task>>();
        }
        _messageIdToCallback[messageType][protoId] = (message) => callback((T)message);
    }
    public  void UnregisterCallback<T>(int protoId) where T : IMessage<T>
    {
        Type messageType = typeof(T);
        if (_messageIdToCallback.ContainsKey(messageType))
        {
            _messageIdToCallback[messageType].Remove(protoId);
        }
    }
    public  async Task<bool> InvokeCallback(int protoId, IMessage message)
    {

        Type messageType = message.GetType();
        if (_messageIdToCallback.TryGetValue(messageType, out Dictionary<int, Func<IMessage, Task>> callbackDict))
        {
            if (callbackDict.TryGetValue(protoId, out Func<IMessage, Task> callback))
            {
                await callback(message);
                return true;
            }
        }
        return false;
    }
    public  void UnregisterMod<T>() where T : ModBase
    {
        var modType = typeof(T);
        if (_mods.TryGetValue(modType, out var mod))
        {
            mod.Dispose();
            _mods.Remove(modType);
        }
    }

    public void RegisterWebSocketCallback(int protoId, Action<IMessage> callback)
    {
        if (!_webSocketCallbacks.ContainsKey(protoId))
        {
            _webSocketCallbacks[protoId] = callback;
        }
        else
        {
            Debug.LogWarning($"WebSocket callback for protoId {protoId} is already registered. Overwriting.");
            _webSocketCallbacks[protoId] = callback;
        }
    }

    public void UnregisterWebSocketCallback(int protoId)
    {
        if (_webSocketCallbacks.ContainsKey(protoId))
        {
            _webSocketCallbacks.Remove(protoId);
        }
        else
        {
            Debug.LogWarning($"No WebSocket callback to unregister for protoId {protoId}.");
        }
    }

    public void InvokeWebSocketCallback(int protoId, IMessage message)
    {
        if (_webSocketCallbacks.TryGetValue(protoId, out var callback))
        {
            callback(message);
        }
        else
        {
            Debug.LogWarning($"No WebSocket callback registered for protoId {protoId}.");
        }
    }




}
