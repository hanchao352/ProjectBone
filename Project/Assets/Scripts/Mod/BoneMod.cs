using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Google.Protobuf;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class BoneMod : SingletonMod<BoneMod>,IMod
{
    private LayerMask interactableLayer;
    public Dictionary<int, Bone> boneDic;
    public bool boneLoaded = false;
    private int currentBoneId = 0;

    public int CurrentBoneId
    {
        get
        {
            return  currentBoneId;
        }

        set
        {
            currentBoneId = value;
            if (GameObjectManager.Instance.Body != null)
            {
               GameObjectManager.Instance.SelectBone(currentBoneId);
            }
           
        }
        
    }

    public override void Initialize()
    {
        base.Initialize();
        boneLoaded = false;
        interactableLayer = 1<<UnityLayer.Layer_Body;
      //  InputManager.Instance.OnTap += OnTap;
        boneDic = new Dictionary<int, Bone>();
        CurrentBoneId = 0;
      
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
    
    public List<Bone> SearchBone(string name)
    {
        
        List<Bone> bones = new List<Bone>();
        foreach (var bone in boneDic)
        {
            if (IsMathch(name,bone.Value.Name ))
            {
                bones.Add(bone.Value);
            }
        }

        return bones;
    }
    
    public bool  IsMathch(string input, string target)
    {
        if (string.IsNullOrEmpty(target))
        {
            return false;
        }

        if (string.IsNullOrEmpty(input))
        {
            return false;
        }
        // 预处理字符串：替换中文括号为英文括号，并对特殊字符进行转义
        input = Regex.Escape(input.Replace('（', '(').Replace('）', ')'));
        target = target.Replace('（', '(').Replace('）', ')');

        // 构建正则表达式，该表达式按顺序包含input中的每个字符，字符之间可以有其他字符
        var patternBuilder = new System.Text.StringBuilder();
        foreach (var ch in input) {
            patternBuilder.Append(Regex.Escape(ch.ToString()) + ".*?");
        }

        Regex regex = new Regex(patternBuilder.ToString(), RegexOptions.Singleline);
        bool isMatch = regex.IsMatch(target);
        return isMatch;
       
    }
    private string BuildPattern(string input) {
        // 使用字典统计input中每个字符的出现次数
        var charCount = new Dictionary<char, int>();
        foreach (var ch in input) {
            if (charCount.ContainsKey(ch)) {
                charCount[ch]++;
            } else {
                charCount[ch] = 1;
            }
        }

        // 构建正则表达式
        var patternBuilder = new System.Text.StringBuilder();
        foreach (var kvp in charCount) {
            // 对每个字符，构建一个模式，确保它在字符串中至少出现了指定次数
            // 考虑到正则表达式的特殊字符需要转义
            var escapedChar = Regex.Escape(kvp.Key.ToString());
            patternBuilder.Append($"(?=.*{escapedChar}{{{kvp.Value},}})");
        }

        return patternBuilder.ToString();
    }

}
