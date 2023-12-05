using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Google.Protobuf;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class WebRequestManager : MonoSingleton<WebRequestManager>, IDisposable
{
    private string url = "http://ecmo.froglesson.com/protobuf/index";
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
    }

    
   public void Dispose()
   {
       
   }
   public void SendMessageAsync<T>(T message) where T : IMessage
   {
       MsgInfo msgInfo = message.ToMsgInfo();
       var jsonFormatter = new JsonFormatter(new JsonFormatter.Settings(true));
       string str = jsonFormatter.Format(message);
       message.ToByteArray();
       WWWForm form = new WWWForm();
       //byte数组转字符串
       form.AddField("msgid",msgInfo.ProtoId); 
       form.AddField("msgstr",msgInfo.msg);  
       //form.AddBinaryData("msgdata",bs);
       Debug.Log("str:"+msgInfo.msg);
       
      
       
       StartCoroutine(PostRequest(url, form));
   }
   IEnumerator PostRequest(string url, WWWForm form)
   {
       using (UnityWebRequest webRequest = UnityWebRequest.Post(url, form))
       {
           yield return webRequest.SendWebRequest();

           if (webRequest.isNetworkError || webRequest.isHttpError)
           {
               Debug.LogError(webRequest.error);
           }
           else
           {
               byte[] bs = new byte[] { 30, 0, 0, 0 };
               byte[] bs1 = new byte[] { 30,0,0,0,16,39,0,0,8,1,18,26,8,10,26,10,232,130,140,232,130,137,49,50,51,52,34,10,231,187,147,230,158,132,49,49,52,52};
               string responseData = webRequest.downloadHandler.text;
               int[] intArray = JsonConvert.DeserializeObject<int[]>(responseData);
               
               // 转换为字节数组
               byte[] byteArray = new byte[intArray.Length];
               for (int i = 0; i < intArray.Length; i++)
               {
                   byteArray[i] = (byte)intArray[i];
               }
               List<byte> catchbufferlist =byteArray.ToList();
              
                //Debug.Log("Received:" + webRequest.downloadHandler.text);
               // Debug.Log("Cookie:"+webRequest.GetRequestHeader("Cookie"));
               byte[] lengthBytes = new byte[4];
               Array.Copy(byteArray, 0, lengthBytes, 0, 4);
               byte[] idBytes = new byte[4];
                Array.Copy(byteArray, 4, idBytes, 0, 4);
               int packagelength = BitConverter.ToInt32(lengthBytes);
               int protoId = BitConverter.ToInt32(idBytes);
               byte[] messageBytes = new byte[packagelength];
                Array.Copy(byteArray, 8, messageBytes, 0, packagelength);
                Type messageType = ProtoManager.Instance.GetTypeByProtoId(protoId);
                IMessage message = (IMessage)ObjectCreator.CreateInstance(messageType);
                message.MergeFrom(messageBytes);
                ModManager.Instance.InvokeWebSocketCallback(protoId, message);
               
               
           }
       }
   }

   private void ReceiveMessages(byte[] bs)
   {



       List<byte> catchbufferlist = bs.ToList();

       int packagelength = BitConverter.ToInt32(catchbufferlist.GetRange(0, 4).ToArray(), 0);
       int protoId = BitConverter.ToInt32(catchbufferlist.GetRange(4, 4).ToArray(), 0);
       int messageLength = packagelength - sizeof(int) * 2;
       Type messageType = ProtoManager.Instance.GetTypeByProtoId(protoId);
       IMessage message = (IMessage)Activator.CreateInstance(messageType);
       byte[] messageBytes = catchbufferlist.GetRange(8, messageLength).ToArray();
       message.MergeFrom(messageBytes);
       ModManager.Instance.InvokeWebSocketCallback(protoId, message);




   }
   
   public Dictionary<string, string> ParseResponse(string responseData)
   {
       var data = new Dictionary<string, string>();
       var lines = responseData.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
       foreach (var line in lines)
       {
           var match = Regex.Match(line, @"[""']?(\w+)[""']?\s*=>\s*string\(\d+\)\s*[""'](.+)[""']");
           if (match.Success)
           {
               data[match.Groups[1].Value] = match.Groups[2].Value;
           }
       }
       return data;
   }
}
