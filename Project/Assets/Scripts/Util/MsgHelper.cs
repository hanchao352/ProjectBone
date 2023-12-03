using System;
using DG.Tweening.Plugins.Options;
using Google.Protobuf;

public class MsgInfo
{
    public int ProtoId;
    public string msg;
}

public static class MsgHelper
{
        
    public static byte[]  Bytes<T>(this T message) where T : IMessage
    {
        int protoId = ProtoManager.Instance.GetProtoIdByType(typeof(T));
        var messageBuffer = message.ToByteArray();
        var headerBuffer = BitConverter.GetBytes(messageBuffer.Length + 4 + 4);
        var messageIdBuffer = BitConverter.GetBytes(protoId);
        byte[] packagebuffer = new byte[messageBuffer.Length + 4 + 4];
        Array.Copy(headerBuffer, 0, packagebuffer, 0, headerBuffer.Length);
        Array.Copy(messageIdBuffer, 0, packagebuffer, 4, messageIdBuffer.Length);
        Array.Copy(messageBuffer, 0, packagebuffer, 8, messageBuffer.Length);
        return packagebuffer;
    }
    //将上面方法改成返回 string数组
    public static MsgInfo  ToMsgInfo<T>(this T message) where T : IMessage
    {
        int protoId = ProtoManager.Instance.GetProtoIdByType(typeof(T));
        var jsonFormatter = new JsonFormatter(new JsonFormatter.Settings(true));
        string str = jsonFormatter.Format(message);
        MsgInfo msgInfo = new MsgInfo();
        msgInfo.ProtoId = protoId;
        msgInfo.msg = str;
        return msgInfo;
    }
    public static void  ToSend<T>(this T message) where T : IMessage
    {
       WebRequestManager.Instance.SendMessageAsync(message);
    }
    
  
}
