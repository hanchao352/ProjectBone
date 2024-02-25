using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

using Object = UnityEngine.Object;

public abstract class UIBase:IViewGeneric
{
    public object[] Args;
    public GameObject Root { get; set; }
    public RectTransform rectTransform { get; set; }
    public List<ComponentBase> childComponents { get; set; }
    public ViewInfo ViewInfo { get; set; }
    public bool Isvisible
    {
        get => isvisible;
    }

    private bool isvisible = false;
    public void CreateRoot()
    {
        Root = ResManager.Instance.LoadRes<GameObject>("UI/prefab/"+ViewInfo.ResName);
        var parent = UIManager.Instance.WindowRoot;
        if (ViewInfo.ViewID == ViewID.BotMenuView)
        {
            parent = UIManager.Instance.BotWindowRoot;
        }
        else if (ViewInfo.ViewID == ViewID.WebView)
        {
            parent = UIManager.Instance.WebWindowRoot;
        }
        Root.transform.SetParent(parent,false); 
        rectTransform = Root.GetComponent<RectTransform>();
        Root.transform.localPosition = Vector3.zero;
        Root.transform.localScale= Vector3.one;
        Root.transform.rotation= Quaternion.identity;

        Initialize();
        OnShow(Args);
    }

    private void OnLoadDone(GameObject uiroot)
    {
        Root=   GameObject.Instantiate(uiroot,UIManager.Instance.UIRoot.transform);
        Initialize();
        OnShow(Args);
    }


    public virtual void Initialize()
    {
        childComponents = new List<ComponentBase>();
    }

    public virtual void OnShow(params object[] args)
    {
        RegisterEvent();
        isvisible = true;
        Root.SetActive(true);
       
    }

    public virtual void RegisterEvent()
    {
        
    }
    
    public virtual void UnRegisterEvent()
    {
        
    }
    
    public virtual void UpdateView(params object[] args)
    {
        for (int i = 0; i < childComponents.Count; i++)
        {
            childComponents[i].UpdateView(); 
        }
    }

    public virtual void Update(float time)
    {
        for (int i = 0; i < childComponents.Count; i++)
        {
            childComponents[i].Update(time);
        }
    }

    public virtual void OnApplicationFocus(bool hasFocus)
    {
        
    }

    public virtual void OnApplicationPause(bool pauseStatus)
    {
       
    }

    public virtual void OnApplicationQuit()
    {
        
    }

    public virtual void OnHide()
    {
        UnRegisterEvent();
        isvisible = false;
        Root.SetActive(false);
        for (int i = 0; i < childComponents.Count; i++)
        {
            childComponents[i].Visible = false;
        }
    }

    public virtual void OnEnterMutex()
    {
        for (int i = 0; i < childComponents.Count; i++)
        {
            childComponents[i].OnEnterMutex();
        }
    }

    public virtual void OnExitMutex()
    {
        for (int i = 0; i < childComponents.Count; i++)
        {
            childComponents[i].OnExitMutex();
        }
    }

    public virtual void Dispose()
    {
        for (int i = 0; i < childComponents.Count; i++)
        {
            childComponents[i].Dispose();
        }
        Object.Destroy(Root);
    }
    
    public virtual T MakeComponent<T>(GameObject comroot) where T:ComponentBase
    {
        T com = comroot.MakeComponent<T>();
        if (childComponents.Contains(com)==false)
        {
            childComponents.Add(com);
        }
        return com;
    }
}