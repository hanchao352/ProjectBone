using System;

public class Singleton<T> where T : class, new()
{
    private static readonly Lazy<T> lazyInstance = new Lazy<T>(() => new T());

    public static T Instance
    {
        get { return lazyInstance.Value; }
    }

    protected Singleton()
    {
    }

    public virtual void Initialize()
    {
        // 在派生类中重写此方法以执行初始化操作
    }

    public virtual void Update(float time)
    {
        // 在派生类中重写此方法以执行更新操作
    }

    public virtual void Destroy()
    {
        // 在派生类中重写此方法以执行销毁操作
    }
}