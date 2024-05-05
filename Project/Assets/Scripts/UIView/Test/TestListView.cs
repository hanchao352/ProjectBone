using System.Collections.Generic;
using Com.ForbiddenByte.OSA.Core;
using UnityEngine;
using UnityEngine.UI;
public class TestListView : UIBase
{
    public CommonList commonList;
    
    private List<Student> students = new List<Student>();
    public Button closeButton;
    public override void Initialize()
    {
        base.Initialize();
        commonList = Root.transform.Find("OSA").GetComponent<CommonList>();
        closeButton = Root.transform.Find("Close").GetComponent<Button>();
        closeButton.onClick.AddListener(OnClose);
        //随机生成1000个学生
        for (int i = 0; i < 100; i++)
        {
            Student student = new Student();
            student.name = "学生" + i;
            student.age = UnityEngine.Random.Range(10, 20);
            students.Add(student);
        }
        commonList.Init();
        commonList.ItemInitEvent += OnItemInitHandler;
        commonList.ItemUpdateEvent += OnItemUpdateHandler;
        commonList.ItemRecycleEvent += OnItemRecycleHandler;
        commonList.ResetItems(students.Count);
        closeButton.onClick.AddListener(OnClose);
    }
    
    private void OnClose()
    {
        UIManager.Instance.HideView(ViewID.TestListView);
    }
    private void OnItemRecycleHandler(BaseItemViewsHolder vh, int index)
    {
        Debug.Log("OnItemRecycleHandler");
      
    }

    private void OnItemUpdateHandler(BaseItemViewsHolder vh)
    {
        Debug.Log("OnItemUpdateHandler");
     //   ItemComponent itemcom = vh.componentBase as ItemComponent;
        //vh.componentBase.Visible = true;
        // itemcom.UpdateUI();
    }

    private void OnItemInitHandler(int index, BaseItemViewsHolder vh)
    {
        Debug.Log("OnItemInitHandler");
      //  vh.componentBase = MakeComponent<ItemComponent>(vh.root);



    }
    public override void OnShow(params object[] args)
    {
        base.OnShow(args);
    }

    public override void UpdateView(params object[] args)
    {
        base.UpdateView(args);
    }

    public override void Update(float time)
    {
        base.Update(time);
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

    public override void OnHide()
    {
        base.OnHide();
    }

    public override void OnEnterMutex()
    {
        base.OnEnterMutex();
    }

    public override void OnExitMutex()
    {
        base.OnExitMutex();
    }

    public override void Dispose()
    {
        base.Dispose();
    }
}