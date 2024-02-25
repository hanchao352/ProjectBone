using System;
using System.Collections.Generic;

public struct ViewInfo 
{
    public int ViewID;
    public string ResName;
    public Type ViewType;
}

public class ViewID
{

    //主界面
    public const int MainView = 1;
    //模型展示界面
    public const int ModelView = 2;
    //webview界面
    public const int WebView = 3;
    //底部菜单界面
    public const int BotMenuView = 4;

}

public class WebUrl
{
    public const string Default= "https://certt.froglesson.com/anatomy/getUserMuscleNote?uid=1";
    public const string Note = "https://certt.froglesson.com/anatomy/createUserMuscleNote?uid=1&muscle_id=1004";
    public const string Collect = "https://certt.froglesson.com/anatomy/createUserMuscleCollect?uid=1&muscle_id=1004";
    public const string Login = "https://certt.froglesson.com/anatomy/login";
}

public class WebState
{
    public const int Default = 1;
    public const int Note = 2;
    public const int Collect = 3;
    public const int Login = 4;
}

public static  class UIDefine
{
    public static Dictionary<int, ViewInfo> _uiViews = new Dictionary<int, ViewInfo>();

    static UIDefine()
    {
        // _uiViews[ViewID.LoginView] = new ViewInfo(){ViewID = ViewID.LoginView,ResName = "UILogin",ViewType = typeof(LoginView)};
       
         _uiViews[ViewID.MainView] = new ViewInfo(){ViewID = ViewID.MainView,ResName = "main_window",ViewType = typeof(UIMainView)};
         _uiViews[ViewID.ModelView] = new ViewInfo(){ViewID = ViewID.MainView,ResName = "show_window",ViewType = typeof(ModView)};
         _uiViews[ViewID.WebView] = new ViewInfo(){ViewID = ViewID.WebView,ResName = "CanvasWebViewPrefab",ViewType = typeof(BoneWebView)};
         _uiViews[ViewID.BotMenuView] = new ViewInfo(){ViewID = ViewID.BotMenuView,ResName = "bot_menu_window",ViewType = typeof(BotMenuView)};
    }
}