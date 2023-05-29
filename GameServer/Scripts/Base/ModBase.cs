using Google.Protobuf;

public abstract class ModBase
{
    protected void RegisterCallback<T>(Func<T, ClientInfo, Task> callback) where T : IMessage<T>
    {
        int protoId = ProtoManager.Instance.GetProtoIdByType(typeof(T));
        ModManager.Instance.RegisterCallback(protoId, callback);
    }

    protected void UnregisterCallback<T>() where T : IMessage<T>
    {
        int protoId = ProtoManager.Instance.GetProtoIdByType(typeof(T));
        ModManager.Instance.UnregisterCallback<T>(protoId);
    }

    protected async Task SendMessageAsync<T>(ClientInfo client, T message) where T : IMessage<T>
    {
        await ModManager.Instance.SendMessageAsync(client, message);
    }

    public virtual void Init()
    {
    }

    public virtual void Update()
    {
    }

    public virtual void Dispose()
    {
    }
}
