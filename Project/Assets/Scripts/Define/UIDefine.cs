
using System.Collections.Generic;



public class ViewID
{
    public const int LoginView = 1;
    public const int TipsView =2;
    public const int SelectOrCreatRoleView = 3;
}

public class UIDefine
{
    public static Dictionary<int, UIBase> _uiViews = new Dictionary<int, UIBase>
    {
        { ViewID.LoginView, new LoginView{packageName = "UILoginWindow",resName = "Main",useAsyncLoad = true} },
        { ViewID.TipsView, new TipsView(){packageName = "UITipsWindow",resName = "Main",useAsyncLoad = true} },
        { ViewID.SelectOrCreatRoleView, new SelectOrCreatRoleView(){packageName = "SelectOrCreateRole",resName = "Main",useAsyncLoad = true} }
    };
}
