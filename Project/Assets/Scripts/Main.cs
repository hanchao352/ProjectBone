using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;

public class Main : MonoBehaviour
{
  

   
    private IGeneric[] managers;
    private IMod[] mods;
    public void Awake()
    {
        DontDestroyOnLoad(this);
        var managerTypes = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => typeof(IGeneric).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

        List<IGeneric> managersList = new List<IGeneric>();

        foreach (var type in managerTypes)
        {
            // 获取静态属性 "Instance"
            var instanceProperty = type.GetProperty("Instance", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
            if (instanceProperty != null)
            {
                // 通过 "Instance" 属性获取单例实例
                var managerInstance = instanceProperty.GetValue(null, null) as IGeneric;
                if (managerInstance != null)
                {
                    managersList.Add(managerInstance);
                }
            }
        }
        managers = managersList.ToArray();

        // 调用每个 manager 的 Initialize 方法
        foreach (var manager in managers)
        {
            manager.Initialize();
        }
        
        var modTypes = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => typeof(IMod).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

        List<IMod> modList = new List<IMod>();

        foreach (var mod in modTypes)
        {
            // 获取静态属性 "Instance"
            var instanceProperty = mod.GetProperty("Instance", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
            if (instanceProperty != null)
            {
                // 通过 "Instance" 属性获取单例实例
                var managerInstance = instanceProperty.GetValue(null, null) as IMod;
                if (managerInstance != null)
                {
                    modList.Add(managerInstance);
                }
            }
        }
        mods = modList.ToArray();
        //调用每个 manager 的 Initialize 方法
        foreach (var mod in modList)
        {
            mod.Initialize();
        }
    }


    // 当对象已启用并处于活动状态时调用此函数
    public void OnEnable()
    {

    }

    // 仅在首次调用 Update 方法之前调用 Start
    public void Start()
    {
       // UIManager.Instance.ShowView(ViewID.LoginView);
       LoadBody();
       BoneMod.Instance.Test();
    }

   

    // 如果 MonoBehaviour 已启用，则在每一帧都调用 Update
    public void Update()
    {
        
        float time = Time.deltaTime;
        foreach (var manager in managers)
        {
            manager.Update(time);
        }
    }

    // 当行为被禁用或处于非活动状态时调用此函数 
    public void OnDisable()
    {

    }

    // 当 MonoBehaviour 将被销毁时调用此函数
    public void OnDestroy()
    {
        foreach (var manager in managers)
        {
            manager.Dispose();
        }
       
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        foreach (var manager in managers)
        {
            manager.OnApplicationFocus(hasFocus);
        }
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        foreach (var manager in managers)
        {
            manager.OnApplicationPause(pauseStatus);
        }
    }

    private void OnApplicationQuit()
    {
        foreach (var manager in managers)
        {
            manager.OnApplicationQuit();
        }
    }

    void LoadBody()
    {
        GameObject obj = ResManager.Instance.LoadRes("jirou_nan");
        obj.transform.position = new Vector3(0, 0, 0);
        for (int i = 0; i < obj.transform.childCount; i++)
        {
            obj.transform.GetChild(i).gameObject.layer = UnityLayer.Layer_Body;
            if ( obj.transform.GetChild(i).gameObject.GetComponent<MeshCollider>() == null)
            {
                obj.transform.GetChild(i).gameObject.AddComponent<MeshCollider>();
            }
            
        }
        GameObjectManager.Instance.Body = obj;
    }




}
