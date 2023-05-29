using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MergeLogInfo
{
    public int version;
    public string msg;

    public string GetMegreInfo()
     {
    //     Merged revision(s) 134018, 134038, 134055 from trunk/code/client/goe/client/Lua:
    //         【26065】【日更230324】【奇遇】触发奇遇界面弹出错误
    //         ........
    //         【26065】【日更230324】【奇遇】触发奇遇界面弹出错误
    //         ........

    return $"{msg}";
     }
}
