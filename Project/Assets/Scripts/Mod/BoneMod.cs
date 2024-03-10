using System.Collections;
using System.Collections.Generic;
using Google.Protobuf;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class BoneMod : SingletonMod<BoneMod>,IMod
{
    private LayerMask interactableLayer;
    public Dictionary<int, Bone> boneDic;
    public bool boneLoaded = false;
    public int currentBoneId;
    public override void Initialize()
    {
        base.Initialize();
        boneLoaded = false;
        interactableLayer = 1<<UnityLayer.Layer_Body;
      //  InputManager.Instance.OnTap += OnTap;
        boneDic = new Dictionary<int, Bone>();
        currentBoneId = 0;
      
    }

    private void OnTap(Vector2 vec2)
    {
        Ray ray = Camera.main.ScreenPointToRay(vec2);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, interactableLayer))
        {
           Debug.Log(hit.transform.name);
        }
    }

    
    public override void RegisterMessageHandler()
    {
        RegisterWebRequestCallback<CSBoneRequest>(OnCSBoneRequest);
        RegisterWebRequestCallback<SCBoneResponse>(OnWebSCBoneResponse);
        RegisterWebRequestCallback<CSAllBoneRequest>(OnCSAllBoneRequest);
        RegisterWebRequestCallback<SCAllBoneResponse>(OnSCAllBoneResponse);
    }

    private void OnCSAllBoneRequest(CSAllBoneRequest obj)
    {
        Debug.Log("OnCSAllBoneRequest");
    }

    private void OnCSBoneRequest(CSBoneRequest obj)
    {
        Debug.Log("OnCSBoneRequest");
    }
 

    private void OnSCAllBoneResponse(SCAllBoneResponse obj)
    {
        //Debug.Log("OnSCAllBoneResponse");
        for (int i = 0; i < obj.Boneinfo.Count; i++)
        {
            BoneInfo boneInfo = obj.Boneinfo[i];
            if (boneDic.ContainsKey(boneInfo.BoneId))
            {
                boneDic[boneInfo.BoneId].UpdateBoneInfo(boneInfo);
            }
            else
            {
                Bone bone = new Bone();
                bone.UpdateBoneInfo(boneInfo);
                boneDic[boneInfo.BoneId] = bone;
            }
            // Debug.Log("BoneId:"+boneInfo.BoneId + " BoneName:"+boneInfo.Bonename + " BoneContent:"+boneInfo.Bonecontent+" BoneType:"+boneInfo.Type);
            // if (boneInfo.Note != null)
            // {
            //     Debug.Log("NoteID:"+boneInfo.Note.NoteId  + " NoteTitle:"+boneInfo.Note.NoteTitle + " NoteContent:"+boneInfo.Note.Notecontent);
            //     for (int j = 0; j < boneInfo.Note.Imageurl.Count; j++)
            //     {
            //         Debug.Log("URL:"+boneInfo.Note.Imageurl[j]);
            //     }
            // }
           
        }

        boneLoaded = true;
        Debug.Log("------------");
       
    }


    public override void UnregisterMessageHandler()
    {
        UnregisterWebRequestCallback<SCBoneResponse>();
        UnregisterWebRequestCallback<CSBoneRequest>();
        UnregisterWebRequestCallback<CSAllBoneRequest>();
        UnregisterWebRequestCallback<SCAllBoneResponse>();
    }
    private void OnWebSCBoneResponse(SCBoneResponse obj)
    {
        Debug.Log("OnWebSCBoneResponse");
    }
    int id = 55;
    public void Test()
    {
       // CSBoneRequest cSBoneRequest = new CSBoneRequest();
       //   cSBoneRequest.BoneId = 10;
       //   cSBoneRequest.ToSend();
       //
       //   CSAllBoneRequest cSAllBoneRequest = new CSAllBoneRequest();
       //   cSAllBoneRequest.ToSend();
         // SCBoneResponse sCBoneResponse = new SCBoneResponse();
         // sCBoneResponse.Result = 1;
         // BoneInfo boneInfo = new BoneInfo();
         // boneInfo.BoneId = 1;
         // boneInfo.Bonename = "BoneName";
         // boneInfo.Bonecontent = "BoneContent";
         // sCBoneResponse.Boneinfo = boneInfo;
         // sCBoneResponse.ToSend();
         //
         // ////////////
         // SCBoneResponse sCBoneResponse1 = new SCBoneResponse();
         // sCBoneResponse1.Result = 1;
         //
         // sCBoneResponse.Boneinfo.BoneId = 2;
         //    sCBoneResponse.Boneinfo.Bonename = "BoneName2";
         //    sCBoneResponse.Boneinfo.Bonecontent = "BoneContent2";
         // sCBoneResponse.ToSend();
         CSAllBoneRequest cSAllBoneRequest = new CSAllBoneRequest();
         cSAllBoneRequest.ToSend();
    }
    
    
}
