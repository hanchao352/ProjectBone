﻿using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModView : UIBase
{
    public Button backbutton;
    public Button refreshbutton;
    public Button switchbutton;
    public Button searchbutton;
    public Button infoButton;
    public Button last_btn;
    public Button bone_btn;
    public Button bone_muscle_btn;
    public Button allshow_btn;
    private TextMeshProUGUI boneText;
    private TextMeshProUGUI bone_muscle_text;
    
    private GameObject switchRoot;
    private GameObject muscleRoot;
    
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
    private bool isBoneRoot;
    
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
        bone_muscle_btn =  Root.transform.Find("left/Image/bone_btn/switch_root/bone_muscle_btn").GetComponent<Button>();
        allshow_btn =  Root.transform.Find("left/Image/bone_btn/switch_root/allshow_btn").GetComponent<Button>();
        boneText =  Root.transform.Find("left/Image/bone_btn/bone_text").GetComponent<TextMeshProUGUI>();
        bone_muscle_text = Root.transform.Find("left/Image/bone_btn/switch_root/bone_muscle_btn/bone_muscle_text").GetComponent<TextMeshProUGUI>();
        switchRoot = Root.transform.Find("left/Image/bone_btn/switch_root").gameObject;
        muscleRoot = Root.transform.Find("left/Image/bone_btn/muscle_root").gameObject;
        
        searchbutton.onClick.AddListener(OnSearchButtonClick);
        backbutton.onClick.AddListener(OnBackButtonClick);
        refreshbutton.onClick.AddListener(OnRefreshButtonClick);
        switchbutton.onClick.AddListener(OnSwitchButtonClick);
        infoButton.onClick.AddListener(OnInfoButtonClick);
        last_btn.onClick.AddListener(OnLastButtonClick);
        bone_btn.onClick.AddListener(OnBoneButtonClick);
        bone_muscle_btn.onClick.AddListener(OnBoneMuscleButtonClick);
        allshow_btn.onClick.AddListener(OnAllShowButtonClick);
        
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
        Init();
    }

    
    // 界面初始化状态
    private void Init()
    {
        showInfoSv = false;
        isBoneRoot = true;
        switchRoot.SetActive(false);
        muscleRoot.SetActive(false);
        infoSvGo.SetActive(showInfoSv); 
        boneText.text = "Bone";
        bone_muscle_text.text = "Muscle";
        // 点击复位按钮时 恢复到原始状态  相机模型需要复位
    }

    public override void RegisterEvent()
    {
        base.RegisterEvent();
        EventManager.Instance.RegisterEvent(EventDefine.BoneClickEvent,OnBoneClick);
    }

    private void OnBoneClick(object[] args)
    {
       int boneid = (int) args[0];
       OnReceiveClickEvent(boneid);
       Debug.Log("骨骼点击事件" + boneid);
    }

    public override void UnRegisterEvent()
    {
        base.UnRegisterEvent();
        EventManager.Instance.UnregisterEvent(EventDefine.BoneClickEvent,OnBoneClick);
    }

    private void OnBoneMuscleButtonClick()
    {
        isBoneRoot = !isBoneRoot;
        switchRoot.SetActive(false);
        muscleRoot.SetActive(!isBoneRoot);
        boneText.text = isBoneRoot ? "Bone" : "Muscle";
        bone_muscle_text.text = isBoneRoot ? "Muscle" : "Bone"; 
        
        //todo 显示骨骼或者肌肉
    }
    
    private void OnAllShowButtonClick()
    {
        //todo 显示所有骨骼肌肉
        Init();
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
        GameObjectManager.Instance.HideBone();
    }
    //透明按钮
    private void OnLucencyButtonClick()
    {
        GameObjectManager.Instance.HideBone();
    }
    //透明其他按钮
    private void OnLucencyOtherButtonClick()
    {
        GameObjectManager.Instance.HideOtherBone();
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
        // todo 打开搜索界面 
        // UIManager.Instance.ShowView(ViewID.WebView);
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
        //todo 恢复到原始状态
    }
    
    private void OnSwitchButtonClick()
    {
        //切换男女模型
        Debug.Log("功能暂未开放");
    }
    
    private void OnInfoButtonClick()
    {
        // todo 打开笔记界面
        UIManager.Instance.ShowView(ViewID.WebView);
    }
    
    private void OnLastButtonClick()
    {
        // 复位？ 还是备份 暂时不太懂 不开放
    }
    
    private void OnBoneButtonClick()
    {
        switchRoot.SetActive(true);
        muscleRoot.SetActive(false);
    }
    
    
    // 收到骨骼或肌肉点击事件
    private void OnReceiveClickEvent(int keyId)
    {
        if (BoneMod.Instance.boneLoaded == false)
        {
            Debug.Log("骨骼数据未加载");
            return;
        }
        if ( BoneMod.Instance.boneDic.ContainsKey(keyId))
        {
            var info = BoneMod.Instance.boneDic[keyId];
            // 隐藏底部菜单 显示骨骼信息
            ShowInfo(true);
            titleText.text = info.Name;
            msgText.text = info.Content;
        }
          
    }
    
    public override void OnShow(params object[] args)
    {
        base.OnShow(args);
    }

    public override void UpdateView(params object[] args)
    {
        base.UpdateView(args);
    }

    private int key = 1001;
    public override void Update(float time)
    {
        base.Update(time);
        // 测试 按键显示骨骼信息
        if (Input.GetKeyDown(KeyCode.L))
        {
            OnReceiveClickEvent(key);
            key++;
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