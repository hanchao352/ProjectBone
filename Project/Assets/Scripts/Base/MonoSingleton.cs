using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<T>();
                if (instance == null)
                {
                    GameObject singleton = new GameObject();
                    instance = singleton.AddComponent<T>();
                    singleton.name = $"{typeof(T).ToString()} (Singleton)";
                }
            }
            return instance;
        }
    }

    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = (T)this;
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
    }

    public virtual void Initialize()
    {
        // Override in subclass to perform initialization
    }

    public virtual void Update()
    {
        // Override in subclass to perform update operations
    }

    public virtual void Destroy()
    {
        // Override in subclass to perform destruction operations
    }
}