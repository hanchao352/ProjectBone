using Google.Protobuf;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;

public class AsyncServer :Singleton<AsyncServer>
{
    private readonly TcpListener _listener;
    private readonly ConcurrentDictionary<Guid, ClientInfo> _clients;
    private string ip = "127.0.0.1";
    private int port = 5678;
    private IPAddress _address;

    public AsyncServer()
    {
        _address = IPAddress.Parse(ip);
        _listener = new TcpListener(_address, port);
        _clients = new ConcurrentDictionary<Guid, ClientInfo>();
    }

    public async Task StartAsync()
    {
        _listener.Start();
        Console.WriteLine("Server started.");

        while (true)
        {
            Socket clientSocket = await _listener.AcceptSocketAsync();
            Console.WriteLine("Client connected.");
            var clientInfo = new ClientInfo(clientSocket);
            _clients.TryAdd(clientInfo.Id, clientInfo);
            _ = ProcessClientAsync(clientInfo);
        }
    }

    //private async Task ProcessClientAsync(ClientInfo client)
    //{
    //    using (client.ClientSocket)
    //    {
    //        const int BufferSizeLimit = 16;
    //        byte[] buffer = new byte[BufferSizeLimit];

    //        MemoryStream ms = new MemoryStream();
    //        BinaryReader br = new BinaryReader(ms);

    //        while (client.ClientSocket.Connected)
    //        {
    //            int bytesRead = await client.ClientSocket.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None);

    //            if (bytesRead == 0)
    //            {
    //                Console.WriteLine("Client disconnected.");
    //                break;
    //            }
    //            if (ms.Length + bytesRead > BufferSizeLimit)
    //            {
    //                Console.WriteLine("Buffer size limit exceeded, disconnecting client.");
    //                break;
    //            }

    //            ms.Write(buffer, 0, bytesRead);
    //            ms.Position -= bytesRead;

    //            while (true)
    //            {
    //                if (ms.Length - ms.Position < sizeof(int))
    //                {
    //                    break;
    //                }
    //                var ss = ms.GetBuffer();
    //                int messageLength = br.ReadInt32() - sizeof(int) * 2;
    //                if (ms.Length - ms.Position < messageLength + sizeof(int)) //剩余的长度,不够解析
    //                {
    //                    ms.Position -= sizeof(int);
    //                    Console.WriteLine(" 剩余的长度,不够解析");
    //                    break;
    //                }

    //                int protoId = br.ReadInt32();
    //                Type messageType = ProtoManager.GetTypeByProtoId(protoId);
    //                IMessage message = (IMessage)Activator.CreateInstance(messageType);

    //                byte[] messageBytes = br.ReadBytes(messageLength);
    //                using (var codedInputStream = new CodedInputStream(messageBytes))
    //                {
    //                    message.MergeFrom(codedInputStream);
    //                }

    //                if (!await ModManager.InvokeCallback(protoId, message, client))
    //                {
    //                    Console.WriteLine($"No callback found for protoId {protoId}.");
    //                }

    //            }

    //            if (ms.Position < ms.Length)
    //            {
    //                int packagelength = br.ReadInt32();
    //                var bs = ms.GetBuffer();
    //                Array.Copy(bs, ms.Position, bs, 0, ms.Length - ms.Position);
    //            }
    //            var bs1 = ms.GetBuffer();
    //            ms.SetLength(ms.Length - ms.Position);
    //            ms.Position = 0;
    //        }
    //    }
    //    _clients.TryRemove(client.Id, out _);
    //}


    private async Task ProcessClientAsync(ClientInfo client)
    {
        using (client.ClientSocket)
        {
            const int BufferSizeLimit = 512;
            byte[] buffer = new byte[BufferSizeLimit];
            List<byte> catchbufferlist = new List<byte>();
            MemoryStream ms = new MemoryStream();
            BinaryReader br = new BinaryReader(ms);

            while (client.ClientSocket.Connected)
            {
                int bytesRead = await client.ClientSocket.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None);

                if (bytesRead == 0)
                {
                    Console.WriteLine("Client disconnected.");
                    break;
                }
                //if (ms.Length + bytesRead > BufferSizeLimit)
                //{
                //    Console.WriteLine("Buffer size limit exceeded, disconnecting client.");
                //    break;
                //}

                catchbufferlist.AddRange(buffer[0..bytesRead]);
                if (catchbufferlist.Count<sizeof(int)*2)
                {
                    continue;
                }
                int packagelength = BitConverter.ToInt32(catchbufferlist.GetRange(0, 4).ToArray(), 0) ;
                int protoId = BitConverter.ToInt32(catchbufferlist.GetRange(4, 4).ToArray(), 0);
                int messageLength = packagelength - sizeof(int) * 2;
                if (catchbufferlist.Count < packagelength)
                {
                    continue;
                }
                ms.Write(catchbufferlist.GetRange(0,packagelength).ToArray(), 0, packagelength);
                ms.Position =0;

                while (catchbufferlist.Count>=packagelength)
                {
                    if (ms.Length - ms.Position < sizeof(int))
                    {
                        break;
                    }
                    var ss = ms.GetBuffer();
                  
                    //if (ms.Length - ms.Position < messageLength + sizeof(int)) //剩余的长度,不够解析
                    //{
                    //    ms.Position -= sizeof(int);
                    //    Console.WriteLine(" 剩余的长度,不够解析");
                    //    break;
                    //}

                    //int protoId = br.ReadInt32();
                    Type messageType = ProtoManager.Instance.GetTypeByProtoId(protoId);
                    IMessage message = (IMessage)Activator.CreateInstance(messageType);

                    byte[] messageBytes = catchbufferlist.GetRange(8,messageLength).ToArray();
                    using (var codedInputStream = new CodedInputStream(messageBytes))
                    {
                        message.MergeFrom(codedInputStream);
                    }

                    if (!await ModManager.Instance.InvokeCallback(protoId, message, client))
                    {
                        Console.WriteLine($"No callback found for protoId {protoId}.");
                    }
                    else
                    {
                        catchbufferlist.RemoveRange(0, packagelength);
                    }
                    if (catchbufferlist.Count>=sizeof(int)*2)
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
        _clients.TryRemove(client.Id, out _);
    }

   

    public async Task StopAsync()
    {
        _clients.Clear();
        _listener.Stop();
        await Task.Delay(100); // Give the listener a little time to close properly
    }

}
