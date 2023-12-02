using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf;
using UnityEngine;
using UnityEngine.Networking;

public class WebRequestManager : MonoSingleton<WebRequestManager>, IDisposable
{
    private string url = "https://certt.froglesson.com/muscle/list?openid=okqbz5N2B73Waqr7cMnuT75jxl2U&unionid=ofJJv6ETow2yvZTyKTXt4qy4qKfU&uid=16569";
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
       byte[] bs = message.Bytes();
       WWWForm form = new WWWForm();
       form.AddBinaryData("msgdata",bs);
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
               Debug.Log("Received: " + webRequest.downloadHandler.text);
                ReceiveMessages(webRequest.downloadHandler.data);
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
}
