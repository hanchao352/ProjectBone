using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;

public class Main : MonoBehaviour
{
  

   
    // 加载脚本实例时调用 Awake
    public void Awake()
    {
        ProtoManager.Instance.Initialize();
        TimerManager.Instance.Initialize();
        // TableManager.Instance.Initialize();
         UIManager.Instance.Initialize();
         ModManager.Instance.Initialize();
         WebRequestManager.Instance.Initialize();
         InputManager.Instance.Initialize();
         
    }


    // 当对象已启用并处于活动状态时调用此函数
    public void OnEnable()
    {

    }

    // 仅在首次调用 Update 方法之前调用 Start
    public void Start()
    {
       // UIManager.Instance.ShowView(ViewID.LoginView);
        TimerManager.Instance.SetInterval(Test1, 1000);
        TimerManager.Instance.SetTimeout(Test2, 5000);
    }

    private void Test2()
    {
        Debug.Log("延时计时器");
    }

    private void Test1()
    {
        Debug.Log("循环计时器");
    }

    // 如果 MonoBehaviour 已启用，则在每一帧都调用 Update
    public void Update()
    {
        
        TimerManager.Instance.Update(Time.deltaTime);
        InputManager.Instance.Update(Time.deltaTime);
        // UIManager.Instance.Update();
        // ProtoManager.Instance.Update();
        // ModManager.Instance.Update();
        // WebSocketClient.Instance.Update();
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
