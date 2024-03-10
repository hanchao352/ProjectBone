using UnityEngine.UI;

public class BotMenuView : UIBase
{
    public Button modelBtn;
    public Button bankBtn;
    public Button mineBtn;
    
    public override void Initialize()
    {
        base.Initialize();
        modelBtn = Root.transform.Find("model_btn").GetComponent<Button>();
        bankBtn = Root.transform.Find("questionbank_btn").GetComponent<Button>();
        mineBtn = Root.transform.Find("mine_btn").GetComponent<Button>();
        modelBtn.onClick.AddListener(OnModelButtonClick);
        bankBtn.onClick.AddListener(OnBankButtonClick);
        mineBtn.onClick.AddListener(OnMineButtonClick);
    }

    //模型
    private void OnModelButtonClick()
    {
        
    }
    //题库
    private void OnBankButtonClick()
    {
    }    
    //我的
    private void OnMineButtonClick()
    {
        int uid = UserMod.Instance.Uid;
        if (uid == 0)
        {
            UIManager.Instance.ShowView(ViewID.WebView,WebState.Login);
        }
        else
        {
            UIManager.Instance.ShowView(ViewID.WebView,WebState.UserInfo);
        }
        //UIManager.Instance.ShowView(ViewID.WebView,WebState.Login);
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