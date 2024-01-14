
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
 using UnityEngine.EventSystems;
 using UnityEngine.Rendering.Universal;

 public class UIManager:SingletonManager<UIManager>, IGeneric
    {
        
  
        //显示中的界面
        private Dictionary<int, UIBase> _showViews = new Dictionary<int, UIBase>();
        //隐藏中的界面
        private Dictionary<int, UIBase> _hideViews = new Dictionary<int, UIBase>();
        public Transform UIRoot { get; set; }
        public Camera UICamera { get; set; }
        public Transform WindowRoot { get; set; }
        public Transform BotWindowRoot { get; set; }
        
        public override void Initialize()
        {
            UIRoot = GameObject.Find("ui_root").transform;
            WindowRoot = UIRoot.Find("canvas/window");
            BotWindowRoot = UIRoot.Find("canvas/bot_window");
            UICamera = new GameObject("UICamera").AddComponent<Camera>();
            UICamera.cullingMask = 1 << LayerMask.NameToLayer("UI");
            UICamera.clearFlags = CameraClearFlags.Depth;
            UICamera.depth = 1;
            UICamera.GetUniversalAdditionalCameraData().renderType = CameraRenderType.Overlay;
            Canvas canvas = UIRoot.Find("canvas").GetComponent<Canvas>();
            canvas.worldCamera = UICamera;
             UIRoot.gameObject.AddComponent<EventSystem>();
             UIRoot.gameObject.AddComponent<StandaloneInputModule>();
             //UIRoot.gameObject.AddComponent<Canvas>();
            GameObject.DontDestroyOnLoad(UIRoot);
            //创建一个model相机
            Camera modelCamera = new GameObject("modelCamera").AddComponent<Camera>();
            Camera.main.GetUniversalAdditionalCameraData().cameraStack.Add(UICamera);
            modelCamera.cullingMask = 1 << LayerMask.NameToLayer("Body");
            modelCamera.clearFlags = CameraClearFlags.Depth;
            modelCamera.depth = 2;
            modelCamera.GetUniversalAdditionalCameraData().renderType = CameraRenderType.Overlay;
            Camera.main.GetUniversalAdditionalCameraData().cameraStack.Add(modelCamera);
        }

        public UIBase GetView(int viewid)
        {
            if (_showViews.ContainsKey(viewid))
            {
                return _showViews[viewid];
            }
            else if (_hideViews.ContainsKey(viewid))
            {
                return _hideViews[viewid];
            }
            return null;
       
        }



        public UIBase ShowView(int viewid, params object[] args)
        {
            UIBase view = GetView(viewid);
            if (view == null)
            {
                view = ObjectCreator.CreateInstance(UIDefine._uiViews[viewid].ViewType) as UIBase;
                view.ViewInfo = UIDefine._uiViews[viewid];
                view.Args = args;
                view.CreateRoot();
                _showViews[viewid] = view;
            }
            else
            {
                if (view.Isvisible)
                {
                    view.UpdateView(args);
                }
                else
                {
                    view.OnShow(args);
                }
                
                _showViews[viewid] = view;
                if (_hideViews.ContainsKey(viewid))
                {
                    _hideViews.Remove(viewid);
                }
            }
           return null;
        }

        public void HideView(int viewid)
        {
            if (_showViews.ContainsKey(viewid))
            {
                UIBase view = _showViews[viewid];
                view.OnHide();
                _showViews.Remove(viewid);
                _hideViews[viewid] = view;
            }
        }

      

        public void ShowTips(string Tips)
        {
          
        }

    

        public override void Update(float time)
        {
            foreach (var view in _showViews)
            {
               view.Value.Update(time);
            }
        }
        
        override public void Dispose()
        {
            foreach (var view in _showViews)
            {
               
            }
           
        }
        
        override public void OnApplicationQuit()
        {
            foreach (var view in _showViews)
            {
              
            }
        }
      public  override  void OnApplicationFocus(bool focus)
        {
            foreach (var view in _showViews)
            {
              
            }
        }
        
        override public void OnApplicationPause(bool pause)
        {
            foreach (var view in _showViews)
            {
              
            }
        }
        
    }
