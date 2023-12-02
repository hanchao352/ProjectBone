using Google.Protobuf;
using System.Net.WebSockets;
using System.Threading;
using System;
using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;

public class WebSocketClient : Singleton<WebSocketClient>
{
    private ClientWebSocket _webSocket = new ClientWebSocket();
    
    public async Task ConnectAsync(string uri)
    {
        await _webSocket.ConnectAsync(new Uri(uri), CancellationToken.None);
        Debug.Log("Connected to WebSocket server.");
        ReceiveMessages();
    }

    private async void ReceiveMessages()
    {
        const int BufferSizeLimit = 512;
        byte[] buffer = new byte[BufferSizeLimit];
        List<byte> catchbufferlist = new List<byte>();
        MemoryStream ms = new MemoryStream();
        BinaryReader br = new BinaryReader(ms);

        while (_webSocket.State == WebSocketState.Open)
        {
            var result = await _webSocket.ReceiveAsync(buffer, CancellationToken.None);

            if (result.MessageType == WebSocketMessageType.Close)
            {
                await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                Debug.Log("Disconnected from WebSocket server.");
                break;
            }
            else
            {
                int bytesRead = result.Count;
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

                while (catchbufferlist.Count >= packagelength)
                {
                    Type messageType = ProtoManager.Instance.GetTypeByProtoId(protoId);

                    IMessage message = (IMessage)Activator.CreateInstance(messageType);

                    byte[] messageBytes = catchbufferlist.GetRange(8, messageLength).ToArray();
                    message.MergeFrom(messageBytes);
                    ModManager.Instance.InvokeWebSocketCallback(protoId, message); // Assuming that you have InvokeWebSocketCallback method for WebSocket callbacks in ModManager
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
    }

    public async Task SendMessageAsync(IMessage message, int protoId)
    {
        var messageBuffer = message.ToByteArray();
        var headerBuffer = BitConverter.GetBytes(messageBuffer.Length + 4 + 4);
        var messageIdBuffer = BitConverter.GetBytes(protoId);
        byte[] packagebuffer = new byte[messageBuffer.Length + 4 + 4];
        Array.Copy(headerBuffer, 0, packagebuffer, 0, headerBuffer.Length);
        Array.Copy(messageIdBuffer, 0, packagebuffer, 4, messageIdBuffer.Length);
        Array.Copy(messageBuffer, 0, packagebuffer, 8, messageBuffer.Length);

        await _webSocket.SendAsync(new ArraySegment<byte>(packagebuffer), WebSocketMessageType.Binary, true, CancellationToken.None);
    }
}
