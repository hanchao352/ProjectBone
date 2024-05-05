using System.Collections.Generic;
using Com.ForbiddenByte.OSA.Core;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SearchView : UIBase
{

    private GameObject jilu;
    private GameObject sousuo;
    private TMP_InputField searchInput;
    private Button cancelBtn;
    private Button delectBtn;
    public CommonList sousuoList;
    public CommonList jiluLIst;
    private List<Bone> bones = new List<Bone>();
    public override void Initialize()
    {
        base.Initialize();
        jilu = Root.transform.Find("top/root").gameObject;
        sousuo = Root.transform.Find("top/sousuo").gameObject;
        searchInput = Root.transform.Find("top/search_input").GetComponent<TMP_InputField>();
        cancelBtn = Root.transform.Find("top/cancel_btn").GetComponent<Button>();
        delectBtn = Root.transform.Find("top/root/delect_btn").GetComponent<Button>();
        cancelBtn.onClick.AddListener(OnCancelButtonClick);
        delectBtn.onClick.AddListener(OnDelectButtonClick);
        searchInput.onSubmit.AddListener(OnSearchInputSubmit);
        searchInput.onSubmit.AddListener(OnSubmit);
       sousuoList = Root.transform.Find("top/sousuo").gameObject.GetComponent<CommonList>();
       sousuoList.Init();
       sousuoList.ItemInitEvent += OnItemInitHandler;
       sousuoList.ItemUpdateEvent += OnItemUpdateHandler;
       sousuoList.ItemRecycleEvent += OnItemRecycleHandler;
       
       jiluLIst = Root.transform.Find("top/root/jilu").gameObject.GetComponent<CommonList>();
       jiluLIst.Init();
       jiluLIst.ItemInitEvent += OnJiLuItemInitHandler;
       jiluLIst.ItemUpdateEvent += OnJiLuItemUpdateHandler;
       jiluLIst.ItemRecycleEvent += OnJiLuItemRecycleHandler;
       
       EventManager.Instance.RegisterEvent(EventDefine.OnSearchItemClick, OnSearchItemClick);
    }

    private void OnSearchItemClick(object[] args)
    {
        string boneName = args[0] as string;
        searchInput.text = boneName;
        OnSearchInputSubmit(boneName);
    }
    
    private void OnJiLuItemRecycleHandler(BaseItemViewsHolder vh, int index)
    {
        
    }
    
    private void OnJiLuItemUpdateHandler(BaseItemViewsHolder vh)
    {
        JiLuItemComponent itemcom = vh.componentBase as JiLuItemComponent;
        itemcom.Visible = true;
        itemcom.SetData(UserMod.Instance.SearchHistory[vh.ItemIndex]);
        itemcom.UpdateUI();
    }
    
    private void OnJiLuItemInitHandler(int index, BaseItemViewsHolder vh)
    {
        vh.componentBase = MakeComponent<JiLuItemComponent>(vh.root);
    }
    private void OnItemRecycleHandler(BaseItemViewsHolder vh, int index)
    {
        
    }

    private void OnItemUpdateHandler(BaseItemViewsHolder vh)
    {
       SearchItemComponent itemcom = vh.componentBase as SearchItemComponent;
       itemcom.Visible = true;
         itemcom.SetData(bones[vh.ItemIndex]);
         itemcom.UpdateUI();
       
    }

    private void OnItemInitHandler(int index, BaseItemViewsHolder vh)
    {
        vh.componentBase = MakeComponent<SearchItemComponent>(vh.root);
    }

    private void OnSubmit(string arg0)
    {
        Debug.Log("OnSubmit“："+arg0);
    }

    private void Init()
    {
        searchInput.text = "";
        bool showJilu = UserMod.Instance.SearchHistory.Count > 0;
        ChangeRoot(showJilu);
    }
    
    private void ChangeRoot(bool showJilu)
    {
        jilu.SetActive(showJilu);
        sousuo.SetActive(false);
        if (showJilu)
        {
            jiluLIst.ResetItems(UserMod.Instance.SearchHistory.Count);
        }
    }
    
    private void OnSearchInputSubmit(string arg0)
    {
        // todo 搜索输入框内容变化  改变显示root
        Debug.Log("OnSearchInputChange");
        ChangeRoot(false);
        bones = BoneMod.Instance.SearchBone(arg0);
        sousuoList.ResetItems(bones.Count);
        if (bones.Count > 0)
        {
            sousuo.SetActive(true);
            jilu.SetActive(false);
            UserMod.Instance.AddSearchHistory(arg0);
            jiluLIst.ResetItems(UserMod.Instance.SearchHistory.Count);
        }
    }
  
    private void OnCancelButtonClick()
    {
        UIManager.Instance.HideView(ViewID.SearchView);
        GameObjectManager.Instance.BodyVisible = true;
    }
    private void OnDelectButtonClick()
    {
        // todo 清除搜索历史
        Debug.Log("OnDelectButtonClick");
        UserMod.Instance.ClearSearchHistory();
        jiluLIst.ResetItems(UserMod.Instance.SearchHistory.Count);
        
    }
    public override void OnShow(params object[] args)
    {
        base.OnShow(args);
        Init();
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
        searchInput.text = "";
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
        EventManager.Instance.UnregisterEvent(EventDefine.OnSearchItemClick, OnSearchItemClick);
    }
}