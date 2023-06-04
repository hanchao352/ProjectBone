using Google.Protobuf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class LoginMod : SingletonMod<LoginMod>
{
    public List<RoleInfo> rolist = new List<RoleInfo>();
    public override void Initialize()
    {

       
    }

    public override void RegisterMessageHandler()
    {
        RegisterCallback<LoginResponse>(OnLoginrResponse);
        RegisterCallback<RegisterResponse>(OnRegisterResponse);
        RegisterWebSocketCallback<LoginResponse>(OnWebLoginrResponse);
       
    }

    private void OnWebLoginrResponse(LoginResponse obj)
    {
       
    }

    public override void UnregisterMessageHandler()
    {
        UnregisterCallback<LoginResponse>();
        UnregisterCallback<RegisterResponse>();
    }

    private async Task OnRegisterResponse(RegisterResponse arg)
    {
        RegisterResponse response = arg as RegisterResponse;
        if (response != null)
        {
            if (response.Result == NResult.Success)
            {
                UIManager.Instance.ShowTips($" {response.Msg} ");
            }
            else
            {
                UIManager.Instance.ShowTips($" 附带消息:{response.Msg}");
            }
            
        }
        await Task.CompletedTask;
    }


    private async Task OnLoginrResponse(LoginResponse arg)
    {
        LoginResponse response = arg;
        if (response != null)
        {
            UIManager.Instance.ShowTips($" {response.Msg}");
            if (response.Result == NResult.Success)
            {
                for (int i = 0; i < response.Rolelist.Count; i++)
                {
                  RoleInfo roleInfo = response.Rolelist[i].ToRoleInfo();
                    rolist.Add(roleInfo);
                }
               
                UIManager.Instance.ShowTips($" 角色数量： {response.Rolelist.Count} ");
                UIManager.Instance.ShowView(ViewID.SelectOrCreatRoleView);
                
            }
            else
            {
                
            }
           
            
        }
  
    }

    public override void Update()
    {
        base.Update();
    }

    public override void Dispose()
    {
        base.Dispose();
        
    }

    public async void SendLoginRequest(string username,string password)
    {
        LoginRequest loginRequest = new LoginRequest();
        loginRequest.Username = username;
        loginRequest.Password = password;
       await SendMessage(loginRequest);
    }
    
    public async void SendRegisterRequest(string username,string password)
    {
        RegisterRequest loginRequest = new RegisterRequest();
        loginRequest.Username = username;
        loginRequest.Password = password;
        await SendMessage(loginRequest);
    }

 
}
