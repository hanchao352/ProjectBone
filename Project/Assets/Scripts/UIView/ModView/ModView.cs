using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModView : UIBase
{
    public Button backbutton;
    public Button refreshbutton;
    public Button switchbutton;
    public Button searchbutton;
    public Button infoButton;
    public Button bone_btn;
    public Button last_btn;

    private GameObject botGo;
    private GameObject infoSvGo;
    private TextMeshProUGUI titleText;
    private GameObject upGo;
    private GameObject downGo;
    private TextMeshProUGUI msgText;
    private Button detailBtn;
    private Button hideBtn;
    private Button lucencyBtn;
    private Button lucencyOtherBtn;
    private Button showBtn;
    private Button otherBtn;
    private bool showInfoSv;
    
    public override void Initialize()
    {
        base.Initialize();
        searchbutton = Root.transform.Find("left/Image/search_btn").GetComponent<Button>();
        backbutton = Root.transform.Find("left/back_btn").GetComponent<Button>();
        refreshbutton = Root.transform.Find("left/Image/refresh_btn").GetComponent<Button>();
        switchbutton = Root.transform.Find("left/Image/switch_btn").GetComponent<Button>();
        infoButton = Root.transform.Find("left/Image/info_btn").GetComponent<Button>();
        last_btn = Root.transform.Find("left/Image/backup_btn").GetComponent<Button>();
        bone_btn = Root.transform.Find("left/Image/bone_btn").GetComponent<Button>();
        searchbutton.onClick.AddListener(OnSearchButtonClick);
        backbutton.onClick.AddListener(OnBackButtonClick);
        refreshbutton.onClick.AddListener(OnRefreshButtonClick);
        switchbutton.onClick.AddListener(OnSwitchButtonClick);
        infoButton.onClick.AddListener(OnInfoButtonClick);
        last_btn.onClick.AddListener(OnLastButtonClick);
        bone_btn.onClick.AddListener(OnBoneButtonClick);
        
        botGo = Root.transform.Find("bot").gameObject;
        infoSvGo = Root.transform.Find("bot/bg/infoSv").gameObject;
        titleText = Root.transform.Find("bot/bg/title/info_text").GetComponent<TextMeshProUGUI>();
        upGo = Root.transform.Find("bot/bg/title/detail_btn/up").gameObject;
        downGo = Root.transform.Find("bot/bg/title/detail_btn/down").gameObject;
        msgText = Root.transform.Find("bot/bg/infoSv/Viewport/Content/msg_text").GetComponent<TextMeshProUGUI>();
        
        detailBtn = Root.transform.Find("bot/bg/title/detail_btn").GetComponent<Button>();
        hideBtn = Root.transform.Find("bot/bg/bottom/hide_btn").GetComponent<Button>();
        lucencyBtn = Root.transform.Find("bot/bg/bottom/lucency_btn").GetComponent<Button>();
        lucencyOtherBtn = Root.transform.Find("bot/bg/bottom/lucency_other_btn").GetComponent<Button>();
        showBtn = Root.transform.Find("bot/bg/bottom/show_btn").GetComponent<Button>();
        otherBtn = Root.transform.Find("bot/bg/bottom/other_btn").GetComponent<Button>();
        detailBtn.onClick.AddListener(OnDetailButtonClick);
        hideBtn.onClick.AddListener(OnHideButtonClick);
        lucencyBtn.onClick.AddListener(OnLucencyButtonClick);
        lucencyOtherBtn.onClick.AddListener(OnLucencyOtherButtonClick);
        showBtn.onClick.AddListener(OnShowButtonClick);
        otherBtn.onClick.AddListener(OnOtherButtonClick);
        showInfoSv = false;
        infoSvGo.SetActive(showInfoSv);
    }

    private void OnSelectBone()
    {
        //todo 当点击骨骼时，显示骨骼信息
        titleText.text = "骨骼名称";
        msgText.text = "骨骼具体信息";
    }
    
    private void OnDetailButtonClick()
    {
        showInfoSv = !showInfoSv;
        infoSvGo.SetActive(showInfoSv);
        upGo.SetActive(showInfoSv);
        downGo.SetActive(!showInfoSv);
    }
    //隐藏按钮
    private void OnHideButtonClick()
    {
        
    }
    //透明按钮
    private void OnLucencyButtonClick()
    {
        
    }
    //透明其他按钮
    private void OnLucencyOtherButtonClick()
    {
        
    }
    //显示按钮
    private void OnShowButtonClick()
    {
        
    }
    //其他按钮
    private void OnOtherButtonClick()
    {
        
    }

    private void OnSearchButtonClick()
    {
        
    }
    
    private void OnBackButtonClick()
    {
        ShowInfo(false);
        UIManager.Instance.ShowView(ViewID.MainView);
        UIManager.Instance.HideView(ViewID.ModelView);
    }

    //底部详细信息
    private void ShowInfo(bool show)
    {
        botGo.SetActive(show);
        if (show)
            UIManager.Instance.HideView(ViewID.BotMenuView);
        else
            UIManager.Instance.ShowView(ViewID.BotMenuView);
    }
    
    private void OnRefreshButtonClick()
    {
        
    }
    
    private void OnSwitchButtonClick()
    {
        
    }
    
    private void OnInfoButtonClick()
    {
        UIManager.Instance.ShowView(ViewID.WebView);
    }
    
    private void OnLastButtonClick()
    {
        
    }
    
    private void OnBoneButtonClick()
    {
        
    }
    
    
    
    public override void OnShow(params object[] args)
    {
        base.OnShow(args);
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