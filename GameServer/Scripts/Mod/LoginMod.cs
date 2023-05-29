using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.Scripts.Define;
using MongoDB.Bson;
using MongoDB.Driver;


public class LoginMod:ModBase
{
    private static LoginMod instance;
    
    public static LoginMod Instance
    { 
        get
        {
            if (instance==null)
            {
                instance = new();
            }
            return instance;
        }
    }

    public override void Init()
    {
        base.Init();
        RegisterCallback<LoginRequest>(OnLoginRequest);
        RegisterCallback<RegisterRequest>(OnLRegisterRequest);
        RegisterCallback<CreateRoleRequest>(OnCreateRoleRequest);
    }

  

    private async Task OnLRegisterRequest(RegisterRequest arg1, ClientInfo arg2)
    {
        RegisterResponse registerResponse = new RegisterResponse();
        var filter = Builders<AccountInfo>.Filter.Eq(info => info.Username, arg1.Username);
        Task<AccountInfo> accountInfo = MongoDBManager.Instance.FindOneByUsernameAsync<AccountInfo>(CollectionNameDefine.AccountCollection,filter);
        if (accountInfo.Result != null)
        {
            registerResponse.Result = NResult.Fail;
            registerResponse.Msg = "ÕËºÅÒÑ´æÔÚ";
        }
        else
        {
            AccountInfo info = new AccountInfo();
            info.Username = arg1.Username;
            info.Password = arg1.Password;
            await MongoDBManager.Instance.InsertAsync<AccountInfo>(CollectionNameDefine.AccountCollection,info);
           
            registerResponse.Result = NResult.Success;
            registerResponse.Msg = "×¢²á³É¹¦";
        }
        await SendMessageAsync(arg2,registerResponse);
    }

    private async Task OnLoginRequest(LoginRequest arg1, ClientInfo arg2)
    {
        LoginResponse loginResponse = new LoginResponse();
        
        var filter = Builders<AccountInfo>.Filter.Eq(info => info.Username, arg1.Username);
        Task<AccountInfo> accountInfo = MongoDBManager.Instance.FindOneByUsernameAsync<AccountInfo>(CollectionNameDefine.AccountCollection,filter);
       if (accountInfo.Result == null)
       {
           loginResponse.Result = NResult.Fail;
           loginResponse.Msg = "ÕËºÅ²»´æÔÚ";
       }
       else
       {
          
           if (accountInfo.Result.Password != arg1.Password)
           {
               loginResponse.Result = NResult.Fail;
               loginResponse.Msg = "ÃÜÂë´íÎó";
              
               
           }
           else
           {
               loginResponse.Result = NResult.Success;
               loginResponse.Msg = "µÇÂ¼³É¹¦";

               var rolefilter = Builders<RoleInfo>.Filter.Eq(info => info.AccountId , accountInfo.Result.Id.ToJson());
               
               var result = await  MongoDBManager.Instance.FindManyAsync<RoleInfo>(CollectionNameDefine.PlayerCollection,rolefilter);
               if (result.Count<=0)
               {
                   
               }
               else
               {
                   for (int i = 0; i < result.Count; i++)
                   {
                       NRoleInfo roleInfo = new NRoleInfo();
                       roleInfo.RoleId = result[i].RoleId;
                       roleInfo.Accountid = result[i].AccountId;
                       roleInfo.Name = result[i].RoleName;
                       roleInfo.Level = result[i].Level;
                       roleInfo.BigServerId = result[i].BigServerId;
                       roleInfo.SmallServerId = result[i].SmallServerId;
                       loginResponse.Rolelist.Add(roleInfo);
                   }
                     
               }
               
           }
       }
      
        
        
        
       await SendMessageAsync(arg2,loginResponse);
    }
    
    
    private async Task OnCreateRoleRequest(CreateRoleRequest arg1, ClientInfo arg2)
    {
       await Task.CompletedTask;
    }
    
    public override void Update()
    {
        base.Update();
    }

    public override void Dispose()
    {
        base.Dispose();
        UnregisterCallback<LoginRequest>();
    }
}

