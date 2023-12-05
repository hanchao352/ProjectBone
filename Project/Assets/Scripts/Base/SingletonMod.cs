using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Protobuf;
using UnityEngine;


public abstract class SingletonMod<T>  where T : class, IMod, new()
    {
        private static T instance;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new T();
                }
                return instance;
            }
        }
        private readonly Dictionary<int, Func<IMessage, Task>> _callbacks = new Dictionary<int, Func<IMessage, Task>>();
        private  Dictionary<Type, Dictionary<int, Func<IMessage, Task>>> _messageIdToCallback;
        bool initialized = false;
        public abstract void RegisterMessageHandler();
        public abstract void UnregisterMessageHandler();
        public bool Initialized => initialized;

        public SingletonMod()
        {
            if (instance != null)
            {
                throw new InvalidOperationException("Cannot create instances of a singleton class.");
            }
        }
      

       
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

        
        protected void RegisterWebRequestCallback<T>(Action<T> callback) where T : IMessage<T>
        {
            int protoId = ProtoManager.Instance.GetProtoIdByType(typeof(T));
            ModManager.Instance.RegisterWebSocketCallback(protoId, callback );
        }

        protected void UnregisterWebRequestCallback<T>() where T : IMessage<T>
        {
            int protoId = ProtoManager.Instance.GetProtoIdByType(typeof(T));
            ModManager.Instance.UnregisterWebSocketCallback(protoId);
        }
        
        
        public void SendWebRequestMessageAsync<T>(T message) where T : IMessage
        {
            int protoId = ProtoManager.Instance.GetProtoIdByType(typeof(T));
            WebRequestManager.Instance.SendMessageAsync(message);
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
        public virtual void Initialize()
        {
            // Default implementation, can be overridden in derived classes
            ModManager.Instance.RegisterMod(Instance);
            RegisterMessageHandler();
            initialized = true;
            Debug.Log(Instance.ToString()+" Initialize");
        }

        public virtual void Update(float time)
        {
            // Default implementation, can be overridden in derived classes
            //Debug.Log(Instance.ToString()+" Update"+time);
        }

        public virtual void OnApplicationFocus(bool hasFocus)
        {
            // Default implementation, can be overridden in derived classes
            Debug.Log(Instance.ToString()+" OnApplicationFocus"+hasFocus);
        }

        public virtual void OnApplicationPause(bool pauseStatus)
        {
            // Default implementation, can be overridden in derived classes
            Debug.Log(Instance.ToString()+"OnApplicationPause"+pauseStatus);
        }

        public virtual void OnApplicationQuit()
        {
            // Default implementation, can be overridden in derived classes
            Debug.Log(Instance.ToString()+" OnApplicationQuit");
        }

        public virtual void Dispose()
        {
            // Default implementation, can be overridden in derived classes
            Debug.Log(Instance.ToString()+" Destroy");
            UnregisterMessageHandler();
        }
    }
