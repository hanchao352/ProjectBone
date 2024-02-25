
using System;
using System.Collections.Generic;
using System.Linq;
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
        //显示中的界面
        private List<UIBase> _showViewsList = new List<UIBase>();
        //隐藏中的界面
        private List<UIBase> _hideViewsList = new List<UIBase>();
        public Transform UIRoot { get; set; }
        public Camera UICamera { get; set; }
        public Transform WindowRoot { get; set; }
        public Transform BotWindowRoot { get; set; }
        public Transform WebWindowRoot { get; set; }

        public Camera ModelCamera { get; set; }
        
        public override void Initialize()
        {
            Debug.Log("UIManager Initialize");
            GameObject eventSystem = GameObject.Find("EventSystem");
            UIRoot = GameObject.Find("ui_root").transform;
            WindowRoot = UIRoot.Find("canvas/window");
            BotWindowRoot = UIRoot.Find("canvas/bot_window");
            WebWindowRoot = UIRoot.Find("canvas/web_window");
            UICamera = new GameObject("UICamera").AddComponent<Camera>();
            UICamera.cullingMask = 1 << LayerMask.NameToLayer("UI");
            UICamera.clearFlags = CameraClearFlags.Depth;
            UICamera.depth = 1;
            UICamera.GetUniversalAdditionalCameraData().renderType = CameraRenderType.Overlay;
            Canvas canvas = UIRoot.Find("canvas").GetComponent<Canvas>();
            canvas.worldCamera = UICamera;
             // UIRoot.gameObject.AddComponent<EventSystem>();
             // UIRoot.gameObject.AddComponent<StandaloneInputModule>();
             //UIRoot.gameObject.AddComponent<Canvas>();
            GameObject.DontDestroyOnLoad(UIRoot);
            GameObject.DontDestroyOnLoad(eventSystem);
            //创建一个model相机
            ModelCamera = new GameObject("modelCamera").AddComponent<Camera>();
            Camera.main.GetUniversalAdditionalCameraData().cameraStack.Add(UICamera);
            ModelCamera.cullingMask = 1 << LayerMask.NameToLayer("Body");
            ModelCamera.clearFlags = CameraClearFlags.Depth;
            ModelCamera.depth = 2;
            ModelCamera.GetUniversalAdditionalCameraData().renderType = CameraRenderType.Overlay;
            Camera.main.GetUniversalAdditionalCameraData().cameraStack.Add(ModelCamera);
            
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
                _showViewsList = _showViews.Values.ToList();
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
                _showViewsList = _showViews.Values.ToList();
                _hideViewsList = _hideViews.Values.ToList();
               
            }
           return view;
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
            _showViewsList = _showViews.Values.ToList();
            _hideViewsList = _hideViews.Values.ToList();
           
        }

      

        public void ShowTips(string Tips)
        {
          
        }

    

        public override void Update(float time)
        {
            
            
            for (int i = 0; i < _showViewsList.Count; i++)
            {
                _showViewsList[i].Update(time);
            }
        }
        
        override public void Dispose()
        {
            for (int i = 0; i < _showViewsList.Count; i++)
            {
                _showViewsList[i].Dispose();
            }
           
        }
        
        override public void OnApplicationQuit()
        {
            for (int i = 0; i < _showViewsList.Count; i++)
            {
                _showViewsList[i].OnApplicationQuit();
            }
        }
      public  override  void OnApplicationFocus(bool focus)
        {
            for (int i = 0; i < _showViewsList.Count; i++)
            {
                _showViewsList[i].OnApplicationFocus(focus);
            }
        }
        
        override public void OnApplicationPause(bool pause)
        {
            for (int i = 0; i < _showViewsList.Count; i++)
            {
                _showViewsList[i].OnApplicationPause(pause);
            }
        }
        
    }
