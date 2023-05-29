// using System;
// using System.Collections.Generic;
// using FairyGUI;
//
// public abstract class ComponentBase : IDisposable
// {
//     public GComponent MainComponent { get;   set; }
//
//    
//    
//
//     #region 虚方法
//
//    
//
//     #endregion
//     
//     #region 抽象方法,声明周期
//
//     public abstract void Init(params object[] args);
//
//     protected abstract void OnShow(params object[] args);
//     protected abstract void OnUpdate() ;
//     protected abstract void OnHide() ;
//     protected abstract void OnDestroy() ;
//
//     #endregion
//     
//
//     #region Component 通用属性
//     
//     public bool Enabled
//     {
//         get {  return MainComponent.enabled; }
//         set { MainComponent.enabled = value; }
//     }
//     
//     public bool Grayed
//     {
//         get {  return MainComponent.grayed; }
//         set { MainComponent.grayed = value; }
//     }
//     
//     public bool Touchable
//     {
//         get {  return MainComponent.touchable; }
//         set { MainComponent.touchable = value; }
//     }
//
//     #endregion
//     
//     public void Dispose()
//     {
//         OnDestroy();
//     }
//
//     #region 外部调用方法
//
//     public void SetVisible(bool vis,params object[] args)
//     {
//         MainComponent.visible = vis;
//         if (vis)
//         {
//             OnShow(args);
//         }
//         else
//         {
//             OnHide();
//         }
//         
//        
//     }
//
//     #endregion
// }
