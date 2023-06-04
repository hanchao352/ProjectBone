using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
  

    private string _ip = "127.0.0.1"; 
    private int _port = 5678;
    // 加载脚本实例时调用 Awake
    public void Awake()
    {
        TableManager.Instance.Initialize();
        UIManager.Instance.Initialize();
        ProtoManager.Instance.Initialize();
        ModManager.Instance.Initialize();

        Client.Instance.ConnectAsync(_ip, _port);
        
        
    }


    // 当对象已启用并处于活动状态时调用此函数
    public void OnEnable()
    {

    }

    // 仅在首次调用 Update 方法之前调用 Start
    public void Start()
    {
        UIManager.Instance.ShowView(ViewID.LoginView);
    }


    // 如果 MonoBehaviour 已启用，则在每一帧都调用 Update
    public void Update()
    {

    }

    // 当行为被禁用或处于非活动状态时调用此函数 
    public void OnDisable()
    {

    }

    // 当 MonoBehaviour 将被销毁时调用此函数
    public void OnDestroy()
    {
       ProtoManager.Instance.Destroy();
       ModManager.Instance.Destroy();
       Client.Instance.Destroy();
       
    }

    






}
