using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

public class TipsView : UIBase
{
    
    public Transform prefabText;
    //缓存中未使用的Text
    private List<TextMeshProUGUI> _textList = new List<TextMeshProUGUI>();
    //正在使用的Text
    private List<TextMeshProUGUI> _usingTextList = new List<TextMeshProUGUI>();
    private Vector3 startPos;
    private Vector3 endPos;
    private float flytime = 1;
    public override void Initialize()
    {
        base.Initialize();
        prefabText = Root.transform.Find("tips_label");
        prefabText.gameObject.SetActive(false);
        startPos = prefabText.localPosition;
        endPos = new Vector3(startPos.x, startPos.y+300, startPos.z);
        Debug.Log("StarPos:"+prefabText.localPosition);
    }

    public override void OnShow(params object[] args)
    {
        base.OnShow(args);
        string tips = args[0] as string;
        ShowTips(tips);
    }

     //从缓存中的Text中获取一个Text,有则直接使用，没有则创建一个新的Text
     TextMeshProUGUI GetText()
     {
            TextMeshProUGUI text;
            if (_textList.Count > 0)
            {
                text = _textList[0];
                _textList.RemoveAt(0);
            }
            else
            {
                text = GameObject.Instantiate(prefabText,Root.transform,false).GetComponent<TextMeshProUGUI>();
            }
            _usingTextList.Add(text);
            return text;
     }

     public void ShowTips(string tips)
     {
         TextMeshProUGUI text = GetText();
         text.gameObject.SetActive(true);
         text.text = tips;
         TweenerCore<Vector3, Vector3, VectorOptions> tween = text.transform.DOLocalMove(endPos, flytime).SetEase(Ease.Linear);
         tween.OnStart(() =>
         {
             Debug.Log("tween.OnStart");
         });
         
         tween.OnUpdate(() =>
         {
             Debug.Log("tween.OnUpdate");
             Debug.Log(text.rectTransform.localPosition.y);
         });
         
            tween.OnComplete(() =>
            {
                text.gameObject.SetActive(false);
                _usingTextList.Remove(text);
                _textList.Add(text);
                text.transform.localPosition = startPos;
                Debug.Log("tween.OnComplete");
            });
       
     }

   

     public override void UpdateView(params object[] args)
    {
        base.UpdateView(args);
        string tips = args[0] as string;
        ShowTips(tips);
    }

    public override void Update(float time)
    {
        base.Update(time);
    }

    public override void OnApplicationFocus(bool hasFocus)
    {
        base.OnApplicationFocus(hasFocus);
    }

    public override void OnApplicationPause(bool pauseStatus)
    {
        base.OnApplicationPause(pauseStatus);
    }

    public override void OnApplicationQuit()
    {
        base.OnApplicationQuit();
    }

    public override void OnHide()
    {
        base.OnHide();
    }

    public override void OnEnterMutex()
    {
        base.OnEnterMutex();
    }

    public override void OnExitMutex()
    {
        base.OnExitMutex();
    }

    public override void Dispose()
    {
        base.Dispose();
        for (int i = 0; i < _usingTextList.Count; i++)
        {
            _usingTextList[i].DOKill();
            GameObject.Destroy(  _usingTextList[i].gameObject);
        }
        for (int i = 0; i < _textList.Count; i++)
        {
            GameObject.Destroy(  _textList[i].gameObject);
        }
        
    }
}