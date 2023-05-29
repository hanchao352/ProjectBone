Timer _timer;
InitManager();
AsyncServer.Instance.Initialize();
await AsyncServer.Instance.StartAsync();
_timer = new Timer(UpdateMods, null, 0, 100);

Console.ReadLine();

// 停止AsyncServer
await AsyncServer.Instance.StopAsync();

// 销毁ModManager
ModManager.Instance.Destroy();

 static void UpdateMods(object state)
{
    ModManager.Instance.Update();
}


async void InitManager()
{
    MongoDBManager.Instance.Initialize();
    await MongoDBManager.Instance.ConnectAsync();
    ProtoManager.Instance.Initialize();
    ModManager.Instance.Initialize();
    

}