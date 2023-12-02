using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoneMod : SingletonMod<BoneMod>
{
    private LayerMask interactableLayer;
    public override void Initialize()
    {
        base.Initialize();
        interactableLayer = UnityLayer.Layer_Body;
        InputManager.Instance.OnTap += OnTap;
        Debug.Log("BoneMod Initialize");
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
        
        RegisterWebRequestCallback<SCBoneResponse>(OnWebSCBoneResponse);
    }

   

    public override void UnregisterMessageHandler()
    {
        UnregisterWebRequestCallback<SCBoneResponse>();
    }
    private void OnWebSCBoneResponse(SCBoneResponse obj)
    {
        Debug.Log("OnWebSCBoneResponse");
    }
    int id = 0;
    public void Test()
    {
       CSBoneRequest cSBoneRequest = new CSBoneRequest();
         cSBoneRequest.BoneId = id++;
         SendWebRequestMessageAsync(cSBoneRequest);
    }
}
