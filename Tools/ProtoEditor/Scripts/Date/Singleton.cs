public class Singleton<T> where T : class, new()
{
    private static readonly T instance;

    static Singleton()
    {
        instance = new T();
    }

    public static T Instance
    {
        get
        {
            return instance;
        }
    }
    
    protected Singleton()
    {
    }

    public virtual void Initialize()
    {
        // 在派生类中重写此方法以执行初始化操作
    }
    
    
    public virtual void Update()
    {
        // 在派生类中重写此方法以执行更新操作
    }

    public virtual void Destroy()
    {
        // 在派生类中重写此方法以执行销毁操作
    }
}