using System;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using Vuplex.WebView;

public class BoneWebView : UIBase
{
    CanvasWebViewPrefab canvasWebViewPrefab;
    public Button BackButton;
    public Button ForwardButton;
    public Button CloseButton;
    
    private int webState;
    private string weburl = "";
    public async override void Initialize()
    {
        Debug.Log("WebView initialized111");
        Web.ClearAllData();
        base.Initialize();
        canvasWebViewPrefab = Root.GetComponent<CanvasWebViewPrefab>();
        BackButton = Root.transform.Find("back").GetComponent<Button>();
        ForwardButton = Root.transform.Find("forward").GetComponent<Button>();
        CloseButton = Root.transform.Find("close").GetComponent<Button>();
        BackButton.onClick.AddListener(OnBackButtonClick);
        ForwardButton.onClick.AddListener(OnForwardButtonClick);
        CloseButton.onClick.AddListener(OnCloseButtonClick);
        canvasWebViewPrefab.InitialUrl = "";
        canvasWebViewPrefab.Initialized += OnInitialized;
        canvasWebViewPrefab.LogConsoleMessages = true;
        
        await canvasWebViewPrefab.WaitUntilInitialized();
        canvasWebViewPrefab.WebView.ConsoleMessageLogged += OnConsoleMessageLogged;
        canvasWebViewPrefab.WebView.MessageEmitted += OnMessageEmitted;
        // After the prefab has initialized, you can use the IWebView APIs via its WebView property.
        // https://developer.vuplex.com/webview/IWebView
        canvasWebViewPrefab.WebView.UrlChanged += OnUrlChanged; 
        canvasWebViewPrefab.WebView.LoadProgressChanged += OnLoadProgress;
        await canvasWebViewPrefab.WebView.WaitForNextPageLoadToFinish();
       
    }
    
    private void OnConsoleMessageLogged(object sender, ConsoleMessageEventArgs e)
    {
        Debug.Log($"Console message logged: [{e.Level}] {e.Message}");
    }

    public void PostMessage(EventState eventState)
    {
        if (canvasWebViewPrefab.WebView?.IsInitialized == true)
        {
            string json = JsonConvert.SerializeObject(eventState);
            canvasWebViewPrefab.WebView.PostMessage(json);
        }
        else
        {
            Debug.Log("WebView not initialized");
        }
    }
   
    
    private  void OnMessageEmitted(object sender, EventArgs<string> e)
    {
        Debug.Log("JSON received: " + e.Value);
        var data = JsonConvert.DeserializeObject<EventState>(e.Value);
        EventManager.Instance.TriggerEvent(data.type,data.message);
    }

    private void H5CallBack(string obj)
    {
      Debug.Log("H5CallBack:"+obj);
    }

    private  void OnInitialized(object sender, EventArgs e)
    {
        string url = "";
        if (webState == 0)
        {
            url = WebInfo.webDic[WebState.Default];
        }
        else
        {
            url = WebInfo.webDic[webState];
        }
        url = WebInfo.webDic[webState];
        weburl = url;
    
        canvasWebViewPrefab.WebView.LoadUrl(url);
        Debug.Log("WebView initialized");
    }

    
    private void OnCloseButtonClick()
    {
        EventManager.Instance.TriggerEvent(EventDefine.HideModel,1);
        UIManager.Instance.HideView(ViewID.WebView);
    }

    private async void OnForwardButtonClick()
    {
        if (canvasWebViewPrefab.WebView.IsInitialized)
        {
            bool result = await canvasWebViewPrefab.WebView.CanGoForward();
            
            if (result)
            {
                canvasWebViewPrefab.WebView.GoForward();
            }
        }
    }

    private async void OnBackButtonClick()
    {
        if (canvasWebViewPrefab.WebView.IsInitialized)
        {
            bool result = await canvasWebViewPrefab.WebView.CanGoBack();
            
            if (result)
            {
                canvasWebViewPrefab.WebView.GoBack();
            }
        }
    }

    private async void OnUrlChanged(object sender, UrlChangedEventArgs e)
    {
        
            Debug.Log("[CanvasWebViewDemo] URL changed: " + e.Url);
    }

    private async void OnLoadProgress(object sender, ProgressChangedEventArgs e)
    {
        Debug.Log("[CanvasWebViewDemo] Load progress: " + e.Progress);
        
        if (e.Type == ProgressChangeType.Started)
        {
            
        }
        else if (e.Type == ProgressChangeType.Updated)
        {
            
        }
        else if (e.Type == ProgressChangeType.Finished)
        {
            // WindowsWebView webView = sender as WindowsWebView;
            // var cookies = await canvasWebViewPrefab.WebView.ExecuteJavaScript("document.cookie");
            // canvasWebViewPrefab.
            // Debug.Log($"---{cookies}-------------");
        }
        else if (e.Type == ProgressChangeType.Failed)
        {
            
        }
    }

  

    public override void OnShow(params object[] args)
    {
        base.OnShow(args);
        EventManager.Instance.TriggerEvent(EventDefine.HideModel,0);
        Debug.Log("WebView OnShow");
        webState = (int)args[0];
        weburl =WebInfo.webDic[webState];
        if (canvasWebViewPrefab.WebView?.IsInitialized == true)
        {
            canvasWebViewPrefab.WebView.LoadUrl(weburl);
        }
    }
    public override void UpdateView(params object[] args)
    {
        base.UpdateView(args);
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
       // canvasWebViewPrefab.Visible = false;
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
    }
}