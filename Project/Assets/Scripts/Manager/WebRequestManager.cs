using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Google.Protobuf;
using UnityEngine;
using UnityEngine.Networking;

public class WebRequestManager : MonoSingleton<WebRequestManager>, IDisposable
{
    private string url = "https://certt.froglesson.com/muscle/index";
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
       
       Type messageType = ProtoManager.Instance.GetTypeByProtoId(msgInfo.ProtoId);
       IMessage message1 = (IMessage)Activator.CreateInstance(messageType);
                
       JsonParser jsonParser = new JsonParser(JsonParser.Settings.Default);
       message1 = jsonParser.Parse(msgInfo.msg, message.Descriptor);
       
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
               string responseData = webRequest.downloadHandler.text;
               
                Debug.Log("Received:" + webRequest.downloadHandler.text);
               // Debug.Log("Cookie:"+webRequest.GetRequestHeader("Cookie"));
               // ReceiveMessages(webRequest.downloadHandler.data);
               string[] parts = responseData.Split(new string[] { "<br>" }, StringSplitOptions.None);

                // 处理 POST 数据
               string postPart = parts[0];
               Dictionary<string, string> postValues = ParseResponse(responseData);
               int protoId = int.Parse(postValues["msgid"]);
                string msgstr = postValues["msgstr"];
                Type messageType = ProtoManager.Instance.GetTypeByProtoId(protoId);
                IMessage message = (IMessage)ObjectCreator.CreateInstance(messageType);
                
                JsonParser jsonParser = new JsonParser(JsonParser.Settings.Default);
                 message = jsonParser.Parse(responseData, message.Descriptor);
                 ModManager.Instance.InvokeWebSocketCallback(protoId, message);
                // 处理 GET 数据
               string getPart = parts[1];
              // Dictionary<string, string> getValues = ParseKeyValuePairs(getPart);

               Debug.Log("99");
               
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
