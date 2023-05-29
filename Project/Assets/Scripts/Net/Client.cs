using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;

public class Client :Singleton<Client>
{
    private Socket _socket;
    private NetworkStream _networkStream;

    public async Task ConnectAsync(string ip, int port)
    {
        try
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            await _socket.ConnectAsync(ip, port);
            //_networkStream = new NetworkStream(_socket, FileAccess.ReadWrite, true);
            Debug.Log("Connected to server.");
            ReceiveMessages();
        }
        catch (Exception e)
        {
            Debug.LogError($"Error connecting to server: {e.Message}");
        }
    }

    public override void Initialize()
    {
        base.Initialize();
    }

    public override void Update()
    {
        base.Update();
    }

    public override void Destroy()
    {
        base.Destroy();
        Disconnect();
    }
  

    private async void ReceiveMessages()
    {
        const int BufferSizeLimit = 512;
        byte[] buffer = new byte[BufferSizeLimit];
        List<byte> catchbufferlist = new List<byte>();
        MemoryStream ms = new MemoryStream();
        BinaryReader br = new BinaryReader(ms);

        while (_socket.Connected)
        {
            

            int bytesRead = await _socket.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None);
            
            
            
            if (bytesRead == 0)
            {
                catchbufferlist.Clear();
                Debug.Log("Disconnected from server.");
                break;
            }
            catchbufferlist.AddRange(buffer[0..bytesRead]);
            if (catchbufferlist.Count < sizeof(int) * 2)
            {
                continue;
            }
            int packagelength = BitConverter.ToInt32(catchbufferlist.GetRange(0, 4).ToArray(), 0);
            int protoId = BitConverter.ToInt32(catchbufferlist.GetRange(4, 4).ToArray(), 0);
            int messageLength = packagelength - sizeof(int) * 2;
            if (catchbufferlist.Count < packagelength)
            {
                continue;
            }
            ms.Write(catchbufferlist.GetRange(0, packagelength).ToArray(), 0, packagelength);
            ms.Position = 0;
            
           
            while (catchbufferlist.Count>=packagelength)
            {
               

               
                //改了解包策略后,就不存在长度不足的情况了
                //if (ms.Length - ms.Position < messageLength + sizeof(int))
                //{
                //    Debug.Log($"消息长度:{messageLength}--剩余长度:{ms.Length - ms.Position}");
                //    ms.Position -= sizeof(int);
                //    break;
                //}

                
                Type messageType = ProtoManager.Instance.GetTypeByProtoId(protoId);
               
                IMessage message = (IMessage)Activator.CreateInstance(messageType);

                byte[] messageBytes = catchbufferlist.GetRange(8, messageLength).ToArray();
                message.MergeFrom(messageBytes);
                ModManager.Instance.InvokeCallback(protoId, message);
                catchbufferlist.RemoveRange(0, packagelength);
                if (catchbufferlist.Count >= sizeof(int) * 2)
                {
                     packagelength = BitConverter.ToInt32(catchbufferlist.GetRange(0, 4).ToArray(), 0);
                     protoId = BitConverter.ToInt32(catchbufferlist.GetRange(4, 4).ToArray(), 0);
                     messageLength = packagelength - sizeof(int) * 2;
                }
               

            }

           
            ms.SetLength(0);
            ms.Position = 0;
        }
    }


    public async Task SendMessageAsync(IMessage message, int protoId)
    {
        var messageBuffer = message.ToByteArray();
        var headerBuffer = BitConverter.GetBytes(messageBuffer.Length + 4+4);
        var messageIdBuffer = BitConverter.GetBytes(protoId);
        byte[] packagebuffer=new byte[messageBuffer.Length + 4+4];
        Array.Copy(headerBuffer, 0, packagebuffer,0, headerBuffer.Length);
        Array.Copy(messageIdBuffer, 0, packagebuffer, 4, messageIdBuffer.Length);
        Array.Copy(messageBuffer, 0, packagebuffer, 8, messageBuffer.Length);
        await _socket.SendAsync(new ArraySegment<byte>(packagebuffer), SocketFlags.None);
       // await _networkStream.WriteAsync(packagebuffer, 0, packagebuffer.Length).ConfigureAwait(false);
    }

    public void Disconnect()
    {
        _networkStream?.Close();
        _socket?.Close();
    }
    
    public  bool IsConnected()
    {
        return _socket.Connected;
    }
}
