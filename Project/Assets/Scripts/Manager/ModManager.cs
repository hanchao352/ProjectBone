using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Protobuf;

public  class ModManager :Singleton<ModManager>
{
    private  Dictionary<Type, Dictionary<int, Func<IMessage, Task>>> _messageIdToCallback;
    private  readonly Dictionary<Type, ModBase> _mods = new Dictionary<Type, ModBase>();
    public ModManager()
    {
        _messageIdToCallback = new Dictionary<Type, Dictionary<int, Func<IMessage, Task>>>();
    }
    public override void Initialize()
    {
        
        RegisterMod( LoginMod.Instance);
        RegisterMod(CreateRoleMod.Instance);
    }


    public  void RegisterMod(ModBase mod)
    {
        var modType = mod.GetType();
        if (_mods.ContainsKey(modType))
        {
            throw new InvalidOperationException($"Mod {modType} has already been registered.");
        }

        mod.Initialize();
        _mods.Add(modType, mod);
    }

    public override void Update()
    {
        base.Update();
        foreach (var mod in _mods.Values)
        {
            mod.Update();
        }
    }

    public override void Destroy()
    {
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

    // public   void InvokeCallback(int protoId, IMessage message)
    // {
    //     foreach (var mod in _mods)
    //     {
    //         if (mod.Value.TryGetCallback(protoId, out var callback))
    //         {
    //             _ = callback(message);
    //         }
    //     }
    //
    // }

    

 
}
