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
    private Button addBtn;
    private Button subBtn;
    private Button add_light_btn;
    private Button sub_light_btn;
    private TMP_InputField light_text;

    private float m_lightValue;

    private Light mLight;
    
    
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
        addBtn = Root.transform.Find("left/Image/bone_btn/muscle_root/add_btn").GetComponent<Button>();
        subBtn = Root.transform.Find("left/Image/bone_btn/muscle_root/sub_btn").GetComponent<Button>();
        add_light_btn = Root.transform.Find("left/Image/add_btn").GetComponent<Button>();
        sub_light_btn = Root.transform.Find("left/Image/sub_btn").GetComponent<Button>();
        light_text =  Root.transform.Find("left/Image/light_text").GetComponent<TMP_InputField>();
        light_text.onValueChanged.AddListener(OnInputValueChanged);
        searchbutton.onClick.AddListener(OnSearchButtonClick);
        backbutton.onClick.AddListener(OnBackButtonClick);
        refreshbutton.onClick.AddListener(OnRefreshButtonClick);
        switchbutton.onClick.AddListener(OnSwitchButtonClick);
        infoButton.onClick.AddListener(OnInfoButtonClick);
        last_btn.onClick.AddListener(OnLastButtonClick);
        bone_btn.onClick.AddListener(OnBoneButtonClick);
        bone_muscle_btn.onClick.AddListener(OnBoneMuscleButtonClick);
        allshow_btn.onClick.AddListener(OnAllShowButtonClick);
        addBtn.onClick.AddListener(OnAddButtonClick);
        subBtn.onClick.AddListener(OnSubButtonClick);
        add_light_btn.onClick.AddListener(OnAddLightButtonClick);
        sub_light_btn.onClick.AddListener(OnSubLightButtonClick);
        
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

    private void OnInputValueChanged(string arg0)
    {
        if (float.TryParse(arg0, out float value))
        {
            mLight.intensity = value;
            m_lightValue = value;
        }
       
    }

    private void OnSubButtonClick()
    {
        int curid = BoneMod.Instance.CurrentBoneId;
        if (curid <= 0) //当前没有选中任何骨骼id
        {
            if (GameObjectManager.Instance.ShowType==(int)BoneShowType.None)
            {
                TipsMod.Instance.ShowTips("已隐藏全部层级");
            }
            else
            {
                GameObjectManager.Instance.ShowType = UtilHelper.ClearHighestBit((int)GameObjectManager.Instance.ShowType);
            }
           
        }
        else
        {
            if (GameObjectManager.Instance.ShowType==(int)BoneShowType.None)
            {
                TipsMod.Instance.ShowTips("已隐藏全部层级");
            }
            else
            {
                GameObjectManager.Instance.ShowType = UtilHelper.ClearHighestBit((int)GameObjectManager.Instance.ShowType);
            }
        }
        
    }

    private void OnAddLightButtonClick()
    {
        m_lightValue+=0.02f;
        mLight.intensity = m_lightValue;
        light_text.text = m_lightValue.ToString("F");
    }
    
    private void OnSubLightButtonClick()
    {
        m_lightValue-=0.02f;
        mLight.intensity = m_lightValue;
        light_text.text = m_lightValue.ToString("F");
    }
    

    private void OnAddButtonClick()
    {
        int curid = BoneMod.Instance.CurrentBoneId;
        if (curid <= 0) //当前没有选中任何骨骼id
        {
            if (GameObjectManager.Instance.ShowType > (int)BoneShowType.All)
            {
                GameObjectManager.Instance.ShowType = (int)BoneShowType.All;
            }
            else
            {
                if (UtilHelper.IsContains(GameObjectManager.Instance.ShowType,(int)BoneShowType.All)) 
                {
                    //GameObjectManager.Instance.ShowType = (int)BoneShowType.All;
                    TipsMod.Instance.ShowTips("已显示全部层级");
                }
                else
                {
                    GameObjectManager.Instance.ShowType =  UtilHelper.AddOneBeforeHighestBit((int)GameObjectManager.Instance.ShowType);
                }
            }
           
        }
        else
        {
            if (GameObjectManager.Instance.ShowType > (int)BoneShowType.All)
            {
                GameObjectManager.Instance.ShowType = (int)BoneShowType.All;
            }
            else
            {
                if (UtilHelper.IsContains(GameObjectManager.Instance.ShowType,(int)BoneShowType.All)) 
                {
                    //GameObjectManager.Instance.ShowType = (int)BoneShowType.All;
                    TipsMod.Instance.ShowTips("已显示全部层级");
                }
                else
                {
                    GameObjectManager.Instance.ShowType =  UtilHelper.AddOneBeforeHighestBit((int)GameObjectManager.Instance.ShowType);
                }
            }
        }
    }


    // 界面初始化状态
    private void Init()
    {
        showInfoSv = false;
        isBoneRoot = true;
        switchRoot.SetActive(false);
        muscleRoot.SetActive(false);
        infoSvGo.SetActive(showInfoSv); 
        boneText.text = "全部";
        bone_muscle_text.text = "肌肉";
        ShowInfo(false);
        // 点击复位按钮时 恢复到原始状态  相机模型需要复位
    }

    public override void RegisterEvent()
    {
        base.RegisterEvent();
        EventManager.Instance.RegisterEvent(EventDefine.BoneClickEvent,OnBoneClick);
        EventManager.Instance.RegisterEvent(EventDefine.HideModel,HideModelCallback);
    }

    private void HideModelCallback(object[] args)
    {
        bool show = (int)args[0] == 1;
        GameObjectManager.Instance.BodyVisible = show;
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
        EventManager.Instance.UnregisterEvent(EventDefine.HideModel,HideModelCallback);
    }

    private void OnBoneMuscleButtonClick()
    {
        isBoneRoot = !isBoneRoot;
        switchRoot.SetActive(false);
        muscleRoot.SetActive(!isBoneRoot);
        boneText.text = isBoneRoot ? "骨骼" : "肌肉";
        bone_muscle_text.text = isBoneRoot ? "肌肉" : "骨骼"; 
        BoneShowType type = isBoneRoot ? BoneShowType.Bone : BoneShowType.Muscle;
        //todo 显示骨骼或者肌肉
        GameObjectManager.Instance.ShowType = (int)type;
        //GameObjectManager.Instance.ShowBoneByType(type);
    }
    
    private void OnAllShowButtonClick()
    {
        //todo 显示所有骨骼肌肉
        Init();
        GameObjectManager.Instance.ShowType = (int)BoneShowType.All;
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
        upGo.SetActive(!showInfoSv);
        downGo.SetActive(showInfoSv);
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
        GameObjectManager.Instance.HideOtherBone();
    }
    //其他按钮
    private void OnOtherButtonClick()
    {
        
    }

    private void OnSearchButtonClick()
    {
        // todo 打开搜索界面 
        UIManager.Instance.ShowView(ViewID.SearchView);
        GameObjectManager.Instance.BodyVisible = false;
    }
    
    private void OnBackButtonClick()
    {
        ShowInfo(false);
        OnAllShowButtonClick();
        UIManager.Instance.ShowView(ViewID.MainView);
        UIManager.Instance.HideView(ViewID.ModelView);
        GameObjectManager.Instance.BodyVisible = false;
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
        BoneMod.Instance.CurrentBoneId = 0;
        GameObjectManager.Instance.HideBone();
        Init();
        GameObjectManager.Instance.ReSet();
    }
    
    private void OnSwitchButtonClick()
    {
        //切换男女模型
        TipsMod.Instance.ShowTips("功能暂未开放");
        Debug.Log("功能暂未开放");
    }
    
    private void OnInfoButtonClick()
    {
        // todo 打开笔记界面
        UIManager.Instance.ShowView(ViewID.WebView,WebState.Note);
    }
    
    private void OnLastButtonClick()
    {
        // 复位？ 还是备份 暂时不太懂 不开放
        GameObjectManager.Instance.SelectBoneByPos(GameObjectManager.Instance.SelectBoneType);
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
        mLight = GameObject.Find("Directional Light").transform.GetComponent<Light>();
        m_lightValue = mLight.intensity;
        light_text.text = m_lightValue.ToString("F");
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