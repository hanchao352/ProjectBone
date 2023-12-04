using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoneMod : SingletonMod<BoneMod>,IMod
{
    private LayerMask interactableLayer;
    public override void Initialize()
    {
        base.Initialize();
        interactableLayer = 1<<UnityLayer.Layer_Body;
        InputManager.Instance.OnTap += OnTap;
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
        Debug.Log("OnSCAllBoneResponse");
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
       //   cSBoneRequest.BoneId = id++;
       //   cSBoneRequest.ToSend();
       //
       //   CSAllBoneRequest cSAllBoneRequest = new CSAllBoneRequest();
       //   cSAllBoneRequest.ToSend();
         SCBoneResponse sCBoneResponse = new SCBoneResponse();
         sCBoneResponse.Result = 1;
         BoneInfo boneInfo = new BoneInfo();
         boneInfo.BoneId = 1;
         boneInfo.Bonename = "BoneName";
         boneInfo.Bonecontent = "BoneContent";
         sCBoneResponse.Boneinfo = boneInfo;
         sCBoneResponse.ToSend();
         
         ////////////
         SCBoneResponse sCBoneResponse1 = new SCBoneResponse();
         sCBoneResponse1.Result = 1;
        
         sCBoneResponse.Boneinfo.BoneId = 2;
            sCBoneResponse.Boneinfo.Bonename = "BoneName2";
            sCBoneResponse.Boneinfo.Bonecontent = "BoneContent2";
         sCBoneResponse.ToSend();
    }
}
