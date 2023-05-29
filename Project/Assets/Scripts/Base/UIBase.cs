using System;
using System.Collections.Generic;
using FairyGUI;
using UnityEngine;

public abstract class UIBase
{
    public string packageName;
    public string resName;
    public bool useAsyncLoad=false;
   // private List<ComponentBase> child = new List<ComponentBase>();
    public GComponent MainComponent { get; private set; }
    
    public  void InitRes(params object[] args)
    {
        SetPackageItemExtension();
        UIPackage.AddPackage("Assets/Res/UI/" + packageName);
        if (useAsyncLoad)
        {
            
            UIPackage.CreateObjectAsync(packageName,resName,OnCreatDone);
        }
        else
        {
            MainComponent=  UIPackage.CreateObject(packageName,resName).asCom;
            GRoot.inst.AddChild(MainComponent);
            Init(args);
            OnShow(args);
        }
    }

    protected virtual void SetPackageItemExtension()
    {
       
    }

    protected abstract void OnInit(params object[] args);
    protected abstract void OnShow(params object[] args);

    protected virtual void OnUpdate()
    {
        
    }
    protected abstract void OnHide() ;
    protected abstract void OnDestroy() ;

    public void Init(params object[] args)
    {
        OnInit(args);
    }

    public void Show(params object[] args)
    {
        OnShow(args);
    }

    public void Hide()
    {
        OnHide();
    }

    public void Dispose()
    {
        OnDestroy();
        // MainComponent.Dispose();
        // for (int i = 0; i < child.Count; i++)
        // {
        //     child[i].Dispose();
        // }
        
    }


    protected void OnCreatDone(GObject result,params object[] args)
    {
        MainComponent = result.asCom;
        GRoot.inst.AddChild(MainComponent);
        Init(args);
        OnShow(args);
       
    }

    // public T MakeComponent<T>(GComponent component) where T : ComponentBase, new() 
    // {
    //     T t = new T();
    //     t.MainComponent = component;
    //     t.Init();
    //     t.SetVisible(false); 
    //     child.Add(t);
    //     return t;
    // }

}