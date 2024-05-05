using System.Collections.Generic;
using UnityEngine;

public interface IViewGeneric
{
    public GameObject Root { get; set; }
    public List<ComponentBase> childComponents { get; set; }
    void Initialize();
    void OnShow(params object[] args);
    void UpdateView(params object[] args);
    void Update(float time);
    void OnApplicationFocus(bool hasFocus);
    void OnApplicationPause(bool pauseStatus);
    void OnApplicationQuit();
    void OnHide();
    void OnEnterMutex();
    void OnExitMutex();
    void Dispose();
        
    T MakeComponent<T>(GameObject comroot) where T:ComponentBase;
        
}