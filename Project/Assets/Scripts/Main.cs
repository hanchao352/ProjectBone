using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using UnityEngine;
using Stopwatch = System.Diagnostics.Stopwatch;


public class Main : MonoBehaviour
{
    private IGeneric[] managers;
    private IMod[] mods;

    public void Awake()
    {
        StartupTimingLogger.Mark("main_awake_enter");
        DontDestroyOnLoad(this);
        SetScreen();
        
        // 添加文件日志记录器（日志写入 persistentDataPath/unity_log.txt）
        if (GetComponent<FileLogger>() == null)
        {
            gameObject.AddComponent<FileLogger>();
        }
        Debug.Log($"[StartupTiming] 启动性能日志: {StartupTimingLogger.LogFilePath}");

        // StandaloneWebView.SetCommandLineArguments("--disable-web-security");
        Stopwatch stageTimer = Stopwatch.StartNew();
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
        StartupTimingLogger.MarkDuration(
            "manager_discovery_complete", stageTimer, $"count={managers.Length}");

        // 调用每个 manager 的 Initialize 方法
        foreach (var manager in managers)
        {
            stageTimer.Restart();
            manager.Initialize();
            StartupTimingLogger.MarkDuration(
                "manager_initialize_complete", stageTimer, $"type={manager.GetType().FullName}");
        }

        stageTimer.Restart();
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
        StartupTimingLogger.MarkDuration(
            "mod_discovery_complete", stageTimer, $"count={mods.Length}");

        //调用每个 manager 的 Initialize 方法
        foreach (var mod in modList)
        {
            stageTimer.Restart();
            mod.Initialize();
            StartupTimingLogger.MarkDuration(
                "mod_initialize_complete", stageTimer, $"type={mod.GetType().FullName}");
        }
        foreach (var manager in managers)
        {
            stageTimer.Restart();
            manager.AllManagerInitialize();
            StartupTimingLogger.MarkDuration(
                "manager_all_initialize_complete", stageTimer, $"type={manager.GetType().FullName}");
        }
        foreach (var mod in mods)
        {
            stageTimer.Restart();
            mod.AllModInitialize();
            StartupTimingLogger.MarkDuration(
                "mod_all_initialize_complete", stageTimer, $"type={mod.GetType().FullName}");
        }
        StartupTimingLogger.Mark("main_awake_exit");
    }

    //设置为竖屏.不自动旋转
    private void SetScreen()
    {
        Screen.orientation = ScreenOrientation.Portrait;
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;
        Screen.autorotateToLandscapeLeft = false;
        Screen.autorotateToLandscapeRight = false;
       
    }

    // 当对象已启用并处于活动状态时调用此函数
    public void OnEnable()
    {

    }

    // 仅在首次调用 Update 方法之前调用 Start
    public void Start()
    {
        StartupTimingLogger.Mark("main_start_enter");
        StartCoroutine(LogFirstFrameEnd());
        GameObjectManager.Instance.SelectBoneType = (int)EnumPos.All;
#if UNITY_EDITOR
        // 编辑器下加载本地 bonedata.txt 模拟服务器数据，用于测试
        LoadLocalBoneData();
#endif
        StartupTimingLogger.Mark("main_start_exit");
    }

    private IEnumerator LogFirstFrameEnd()
    {
        yield return new WaitForEndOfFrame();
        StartupTimingLogger.Mark("first_frame_end");
    }

#if UNITY_EDITOR
    private void LoadLocalBoneData()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("bonedata");
        if (textAsset == null)
        {
            Debug.LogWarning("[Main] bonedata.txt 未找到");
            return;
        }

        ButtonBehavior buttonBehavior = FindObjectOfType<ButtonBehavior>();
        if (buttonBehavior != null)
        {
            buttonBehavior.ReceiveMessage(textAsset.text);
        }
    }
#endif

    private void OnEvent2(object[] args)
    {
        Debug.Log("OnEvent2");
    }

    private void OnEvent1()
    {
        Debug.Log("OnEvent1");
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

  




}
