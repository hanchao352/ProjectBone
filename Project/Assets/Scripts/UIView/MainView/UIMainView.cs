using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMainView : UIBase
{
   public Button maleBtn;
   public Button femaleBtn;
   public Button armBtn;
   public Button headBtn;
   public Button legBtn;
   public Button bellyBtn;
    public override void Initialize()
    {
        base.Initialize();
        maleBtn = Root.transform.Find("top/male_body_btn").GetComponent<Button>();
        femaleBtn = Root.transform.Find("top/female_body_btn").GetComponent<Button>();
        armBtn = Root.transform.Find("top/arm_btn").GetComponent<Button>();
        headBtn = Root.transform.Find("top/head_btn").GetComponent<Button>();
        legBtn = Root.transform.Find("top/leg_btn").GetComponent<Button>();
        bellyBtn = Root.transform.Find("top/belly_btn").GetComponent<Button>();
        maleBtn.onClick.AddListener(OnMaleButtonClick);
        femaleBtn.onClick.AddListener(OnFemaleButtonClick);
        armBtn.onClick.AddListener(OnArmButtonClick);
        headBtn.onClick.AddListener(OnHeadButtonClick);
        legBtn.onClick.AddListener(OnLegButtonClick);
        bellyBtn.onClick.AddListener(OnBellyButtonClick);
    }

    private void OnFemaleButtonClick()
    {
        Debug.Log($"功能暂未开放");
    }

    private void OnMaleButtonClick()
    {
        UIManager.Instance.HideView(ViewID.MainView);
        UIManager.Instance.ShowView(ViewID.ModelView);
    }
    
    private void OnArmButtonClick()
    {
        Debug.Log($"功能暂未开放");
    }

    private void OnHeadButtonClick()
    {
        Debug.Log($"功能暂未开放");
    }
    private void OnLegButtonClick()
    {
        Debug.Log($"功能暂未开放");
    }
    private void OnBellyButtonClick()
    {
        Debug.Log($"功能暂未开放");
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
