using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class SVNLog 
{
    // Start is called before the first frame update
    private string versionstring;
    private string commitmsg;
    //r130015 | zhanpengfei | 20230318 01:18:50 +0800 (周六, 18 3月 2023) | 1 line
    private string version;
    private string user;
    private string time;
    private int line = 1;
    private List<int> order = new List<int>();
    public Dictionary<int, int> verDic = new Dictionary<int, int>();
    //------------------------------------------------------------
    public string revision = "revision";
    public string author = "author";
    public string date = "date";
    public string msg = "msg";
    public void SetVer(string versionstring)
    {
        this.versionstring = versionstring;
        this.commitmsg = commitmsg;
        string[] list = this.versionstring.Split('|');
       
        this.version = list[0].Substring(1);
        
        this.user = list[1];
        
      
        this.time = list[2];
        string[] lines = list[3].Split(new string[] { string.Empty },StringSplitOptions.RemoveEmptyEntries);
        int.TryParse(lines[0], out line);
    }

    public void SetCommitMsg(string msg)
    {
        this.commitmsg = msg;
        this.commitmsg = this.commitmsg.Replace('【','[');
        this.commitmsg = this.commitmsg.Replace('】',']');
        Regex r = new Regex(@"\[(.*?)\]");
        MatchCollection ms = r.Matches(this.commitmsg);
        foreach (Match match in ms)
        {
            GroupCollection groups = match.Groups;
            Debug.Log(groups[1].Value);
            int num ;
            if (int.TryParse(groups[1].Value,out num))
            {
                order.Add(num);
                verDic[num] = int.Parse(version);
            }
        }
    }

    public string GetVersion(int ordernum)
    {
        for (int i = 0; i < order.Count; i++)
        {
            if (ordernum == order[i])
            {
                return version;
            }
        }

        return string.Empty;
    }

    //public void SetC
}
