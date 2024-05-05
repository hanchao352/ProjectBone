using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class JiLuItemComponent : ComponentBase
{
    public TextMeshProUGUI TitleText;
    public string boneName = "";
    public Button GotoBtn;
    private List<Bone> bones = new List<Bone>();
    public override void Initialize()
    {
        base.Initialize();
        TitleText = Root.transform.Find("BackgroundImage/Text (TMP)").GetComponent<TextMeshProUGUI>();
        GotoBtn = Root.transform.Find("BackgroundImage").GetComponent<Button>();
        GotoBtn.onClick.AddListener(OnGotoBtnClick);
    }

    private void OnGotoBtnClick()
    {
        EventManager.Instance.TriggerEvent(EventDefine.OnSearchItemClick, boneName);
    }
    
    public override void OnShow(params object[] args)
    {
        base.OnShow(args);
        UpdateUI();
    }

    public override void UpdateView(params object[] args)
    {
        base.UpdateView(args);
        UpdateUI();
    }
    
    public void SetData(string boneName)
    {
        this.boneName = boneName;
    }
    
    public void UpdateUI()
    {
        if (string.IsNullOrEmpty(boneName))
            return;
        TitleText.text = boneName;
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