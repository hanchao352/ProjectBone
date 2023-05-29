//Des:
//Author:韩超
//Time:2023年04月06日 星期四 15:01

using System.Threading.Tasks;
using Google.Protobuf;
using UnityEngine;



public class CreateRoleMod : SingletonMod<CreateRoleMod>
{
 

    public override void Initialize()
    {
        base.Initialize();
       
    }

    public override void RegisterMessageHandler()
    {
        RegisterCallback<CreateRoleResponse>(OnCreateRoleResponse);
    }
    private Task OnCreateRoleResponse(CreateRoleResponse arg)
    {
        CreateRoleResponse response = arg as CreateRoleResponse;
        if (response != null)
        {
            UIManager.Instance.ShowTips($" {response.Result.ToString()} : {response.Msg}");
        }

        return Task.CompletedTask;
    }

    public override void Update()
    {
        base.Update();
    }

    public override void Dispose()
    {
        base.Dispose();
       
    }
    
    public async void CreareRoleRquest(string rolename,int gender )
    {
        CreateRoleRequest createRoleRequest = new CreateRoleRequest();
        createRoleRequest.Rolename = rolename;
        createRoleRequest.Gender = gender;
        await SendMessage(createRoleRequest);
    }

   

    public override void UnregisterMessageHandler()
    {
        UnregisterCallback<CreateRoleResponse>();
    }
}