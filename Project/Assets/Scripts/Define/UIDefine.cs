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
    //测试界面
    public const int TestView = 5;
    //tips界面
    public const int TipsView = 6;
    //测试list
    public const int TestListView = 7;

    public const int SearchView = 8;

}

public class WebUrl
{
    
    
    public  static string Default= "https://certt.froglesson.com/anatomy/getUserMuscleNote?uid={0}";
    public  static string Note = "https://certt.froglesson.com/anatomy/createUserMuscleNote?uid={0}&muscle_id={1}";
    public  static string Collect = "https://certt.froglesson.com/anatomy/muscleNoteDetail?id=2";//"https://certt.froglesson.com/anatomy/createUserMuscleCollect?uid=1&muscle_id=1004";
    public  static string Login = "certt.froglesson.com/anatomy/login";
    public  static string UserInfo = "https://certt.froglesson.com/anatomy/my?uid={0}";
    
}

public static class WebInfo
{
    public static Dictionary<int,string> webDic= new Dictionary<int, string>();
     static WebInfo()
    {
        webDic[WebState.Default] = WebUrl.Default;
        webDic[WebState.Note] = WebUrl.Note;
        webDic[WebState.Collect] = WebUrl.Collect;
        webDic[WebState.Login] = WebUrl.Login;
        webDic[WebState.UserInfo] = WebUrl.UserInfo;
    }
}

public class WebState
{
    public const int Default = 1;
    public const int Note = 2;
    public const int Collect = 3;
    public const int Login = 4;
    public const int UserInfo = 5;
}

public class EventState
{
    public int type { get; set; }
    public string message { get; set; }
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
         _uiViews[ViewID.TestView] = new ViewInfo(){ViewID = ViewID.TestView,ResName = "Test_window",ViewType = typeof(TestView)};
         _uiViews[ViewID.TipsView] = new ViewInfo(){ViewID = ViewID.TipsView,ResName = "tips_window",ViewType = typeof(TipsView)};
         _uiViews[ViewID.TestListView] = new ViewInfo(){ViewID = ViewID.TestListView,ResName = "TestList",ViewType = typeof(TestListView)};
         _uiViews[ViewID.SearchView] = new ViewInfo(){ViewID = ViewID.SearchView,ResName = "search_window",ViewType = typeof(SearchView)};
         
    }
}