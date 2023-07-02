
    using System.Collections.Generic;
    using FairyGUI;
    using UnityEngine;

    public class UIManager:Singleton<UIManager>
    {
        

        private Dictionary<int, UIBase> _uiViews = new Dictionary<int, UIBase>();
        private GComponent _uiRoot;
        private TipsView _tipsView;
        public override void Initialize()
        {
            GRoot.inst.SetContentScaleFactor(Screen.width, Screen.height, UIContentScaler.ScreenMatchMode.MatchWidthOrHeight);
            _uiRoot = GRoot._inst;
            _tipsView = ShowView(ViewID.TipsView) as TipsView;
        }

        public UIBase GetView(int viewid)
        {
            if (_uiViews.ContainsKey(viewid))
            {
                return _uiViews[viewid];
            }
            else
            {
                
                _uiViews[viewid]=UIDefine._uiViews[viewid];
                _uiViews[viewid].InitRes();
                return _uiViews[viewid];
            }
        }



        public UIBase ShowView(int viewid, params object[] args)
        {
            
            if (_uiViews.ContainsKey(viewid))
            {
                _uiViews[viewid].Show(args);
                return _uiViews[viewid];
            }
            else
            {
                
                _uiViews[viewid]=UIDefine._uiViews[viewid];
                _uiViews[viewid].InitRes();
                return _uiViews[viewid];
            }
        }

        public void HideView(int viewid)
        {
            if (_uiViews.ContainsKey(viewid))
            {
                _uiViews[viewid].Hide();
                _uiRoot.RemoveChild(_uiViews[viewid].MainComponent);
            }
        }

        public void DestroyView(int viewid)
        {
            if (_uiViews.ContainsKey(viewid))
            {
                _uiViews[viewid].Dispose();
                _uiViews.Remove(viewid);
            }
        }

        public void ShowTips(string Tips)
        {
            if (_tipsView != null)
            {
                _tipsView.ShowTips(Tips);
            }
        }
    }
