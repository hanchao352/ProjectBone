using System;
using UnityEditor.Experimental.GraphView;
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
    
    public async override void Initialize()
    {
        Debug.Log("WebView initialized111");
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
        await canvasWebViewPrefab.WaitUntilInitialized();
        canvasWebViewPrefab.WebView.MessageEmitted += OnMessageEmitted;
        // After the prefab has initialized, you can use the IWebView APIs via its WebView property.
        // https://developer.vuplex.com/webview/IWebView
        canvasWebViewPrefab.WebView.UrlChanged += OnUrlChanged; 
        canvasWebViewPrefab.WebView.LoadProgressChanged += OnLoadProgress;
        
    }

    private void OnMessageEmitted(object sender, EventArgs<string> e)
    {
        Debug.Log("JSON received: " + e.Value);
    }

    private void OnInitialized(object sender, EventArgs e)
    {
        string url = "";
        switch (webState)
        {
            case WebState.Note:
                url = WebUrl.Note;
                break;
            case WebState.Collect:
                url = WebUrl.Collect;
                break;
            case WebState.Login:
                url = WebUrl.Login;
                break;
            default:
                url = WebUrl.Default;
                break;
        }
        canvasWebViewPrefab.WebView.LoadUrl(url);
        Debug.Log("WebView initialized");
    }

    
    private void OnCloseButtonClick()
    {
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

    private void OnUrlChanged(object sender, UrlChangedEventArgs e)
    {
        
            Debug.Log("[CanvasWebViewDemo] URL changed: " + e.Url);
        
    }

    private void OnLoadProgress(object sender, ProgressChangedEventArgs e)
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
            
        }
        else if (e.Type == ProgressChangeType.Failed)
        {
            
        }
    }

  

    public override void OnShow(params object[] args)
    {
        base.OnShow(args);
        Debug.Log("WebView OnShow");
        webState = (int)args[0];
    }

    public override void UpdateView(params object[] args)
    {
        base.UpdateView(args);
    }

    public override void Update(float time)
    {
        base.Update(time);
        if (Input.GetKeyUp(KeyCode.A))
        {
            canvasWebViewPrefab.WebView.LoadUrl("www.qq.com");
        }
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