using System.Collections.Generic;

public class TipsMod : SingletonMod<TipsMod>, IMod
{
    //消息队列
    private Queue<string> _tipsQueue = new Queue<string>();
    public override void RegisterMessageHandler()
    {

    }

    public override void UnregisterMessageHandler()
    {

    }

    public override void Initialize()
    {
        base.Initialize();
    }

    public override void AllModInitialize()
    {
        base.AllModInitialize();
    }

    public override void Update(float time)
    {
        base.Update(time);
        if (_tipsQueue.Count <=0) 
        {
            return;
        }
        UIManager.Instance.ShowView(ViewID.TipsView, _tipsQueue.Dequeue());
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
    
    public void ShowTips(string Tips)
    {
        _tipsQueue.Enqueue(Tips);
    }
}