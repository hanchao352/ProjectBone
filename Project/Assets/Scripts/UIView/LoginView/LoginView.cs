using System;
using FairyGUI;
using UnityEngine;

public class LoginView: UIBase
{
    public GTextInput username;
    public GTextInput password;
    public GButton loginBtn;
    public GButton registerBtn;
    protected override void OnInit(params object[] args) 
    {
        username = MainComponent.GetChild("userinput").asTextInput;
        password = MainComponent.GetChild("passinput").asTextInput;
        loginBtn = MainComponent.GetChild("f_login").asButton;
        loginBtn.onClick.Add(OnLoginButtonClick);
      
        registerBtn = MainComponent.GetChild("register").asButton;
        registerBtn.onClick.Add(OnRegiseterButtonClick);
    }

   

    private void OnRegiseterButtonClick(EventContext context)
    {
        if (Client.Instance.IsConnected()==false)
        {
            UIManager.Instance.ShowTips("服务器未连接");
            return;
        }
        //LoginMod.Instance.SendRegisterRequest(username.text, password.text);
    }

    private void OnLoginButtonClick(EventContext context)
    {
        // LoginMod.Instance.SendLoginRequest("","");
        // if (Client.Instance.IsConnected()==false)
        // {
        //     UIManager.Instance.ShowTips("服务器未连接");
        //     return;
        // }
        // LoginMod.Instance.SendLoginRequest(username.text, password.text);
    }


    protected override void OnShow(params object[] args)
    {
        TimerManager.Instance.SetInterval(BoneMod.Instance.Test, 1000);

    }

    protected override void OnHide()
    {
       
    }

    protected override void OnDestroy()
    {
       
    }
}
