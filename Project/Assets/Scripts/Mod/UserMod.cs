using UnityEngine;

public class UserMod : SingletonMod<UserMod>, IMod
{
    private int _uid = 0;
    private int _muscleid = 0;

    public int Uid
    {
        get
        {
            if (_uid==0)
            {
                _uid = PlayerPrefs.GetInt("uid",0);
            }
            return _uid;
        }

        set
        {
            _uid = value;
            if (_uid!=0)
            {
                PlayerPrefs.SetInt("uid",_uid);
                string str = string.Format(WebUrl.UserInfo, _uid);
                WebInfo. webDic[WebState.UserInfo]= string.Format(WebUrl.UserInfo, _uid);
            }
            
        }
    }

    public int Muscleid
    {
        get { return _muscleid;}
        set
        {
            _muscleid = value;
            if (_muscleid!=0 && Uid!=0)
            {
                WebInfo. webDic[WebState.Note] = string.Format(WebUrl.Note, Uid,_muscleid);
            }
        }
    }

    public override void RegisterMessageHandler()
    {

    }

    public override void UnregisterMessageHandler()
    {

    }

    public override void Initialize()
    {
        base.Initialize();
        Uid = PlayerPrefs.GetInt("uid",0);
    }

    public override void AllModInitialize()
    {
        base.AllModInitialize();
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

    public override void Dispose()
    {
        base.Dispose();
    }
}