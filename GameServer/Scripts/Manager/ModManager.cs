using Google.Protobuf;
using System.Net.Sockets;
using System.Net;

public class ModManager:Singleton<ModManager>
{
    private  Dictionary<Type, Dictionary<int, Func<IMessage, ClientInfo, Task>>> _messageIdToCallback;
    private  readonly Dictionary<Type, ModBase> _mods = new Dictionary<Type, ModBase>();

    public ModManager()
    {
        _messageIdToCallback = new Dictionary<Type, Dictionary<int, Func<IMessage, ClientInfo, Task>>>();
    }
    
    public override void Initialize()
    {
        base.Initialize();
        RegisterMod(LoginMod.Instance);
    }
    public override void Update()
    {
        foreach (var mod in _mods.Values)
        {
            mod.Update();
        }
    }

    public override void Destroy()
    {
        base.Destroy();
        foreach (var mod in _mods.Values)
        {
            mod.Dispose();
        }

        _mods.Clear();
        _messageIdToCallback =null;
    }

    public  void RegisterMod(ModBase mod)
    {
        var modType = mod.GetType();
        if (_mods.ContainsKey(modType))
        {
            throw new InvalidOperationException($"Mod {modType} has already been registered.");
        }

        mod.Init();
        _mods.Add(modType, mod);
    }


    public  void RegisterCallback<T>(int protoId, Func<T, ClientInfo, Task> callback) where T : IMessage<T>
    {
        Type messageType = typeof(T);
        if (!_messageIdToCallback.ContainsKey(messageType))
        {
            _messageIdToCallback[messageType] = new Dictionary<int, Func<IMessage, ClientInfo, Task>>();
        }
        _messageIdToCallback[messageType][protoId] = (message, client) => callback((T)message, client);
    }

    public  void UnregisterCallback<T>(int protoId) where T : IMessage<T>
    {
        Type messageType = typeof(T);
        if (_messageIdToCallback.ContainsKey(messageType))
        {
            _messageIdToCallback[messageType].Remove(protoId);
        }
    }
    public  async Task<bool> InvokeCallback(int protoId, IMessage message, ClientInfo client)
    {

        Type messageType = message.GetType();
        if (_messageIdToCallback.TryGetValue(messageType, out Dictionary<int, Func<IMessage, ClientInfo, Task>> callbackDict))
        {
            if (callbackDict.TryGetValue(protoId, out Func<IMessage, ClientInfo, Task> callback))
            {
                await callback(message, client);
                return true;
            }
        }
        return false;
    }

    public  async Task SendMessageAsync<T>(ClientInfo client, T message) where T : IMessage<T>
    {
        using (MemoryStream ms = new MemoryStream())
        {
            int protoId = ProtoManager.Instance.GetProtoIdByType(typeof(T));
            BinaryWriter bw = new BinaryWriter(ms);

            // 写入包头和协议ID
            // 协议长度
            int messageLength= message.ToByteArray().Length;
            int packageLength= messageLength+sizeof(int)*2;

            ms.Seek(0, SeekOrigin.Begin);
            bw.Write(messageLength+sizeof(int)*2);
            bw.Write(protoId);
            bw.Write(message.ToByteArray());
            // 写入协议内容
            // message.WriteTo(ms);




            try
            {
                byte[] messageBytes = ms.ToArray();

                SocketAsyncEventArgs sendEventArgs = new SocketAsyncEventArgs();
                sendEventArgs.SetBuffer(messageBytes, 0, messageBytes.Length);
                 client.ClientSocket.SendAsync(sendEventArgs);
                if (sendEventArgs.BytesTransferred < messageBytes.Length)
                {
                    Console.WriteLine("缓冲区已满，发送未完成。");
                }


               
               
              
                
            }
            catch (Exception e)
            {

                Console.WriteLine(e.ToString());
            }
            

           
        }
    }

 
}
