using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;
using UnityEngine;

public class SVNLogInfo 
{
    private string revision = "revision";
    private string author = "author";
    private string date = "date";
    private string msg = "msg";

    private int svnrevision;
    private string svnauthor;
    private string svndate;
    private string svnmsg;
    private List<int> order = new List<int>();
    public Dictionary<int, int> verDic = new Dictionary<int, int>();
    public SVNLogInfo(XmlNode node)
    {
       
        svnrevision = int.Parse(node.Attributes.GetNamedItem(revision).Value);
        svnauthor = node.ChildNodes[0].InnerText;
         svndate = node.ChildNodes[1].InnerText;
         svnmsg = node.ChildNodes[2].InnerText;
         Init();
    }

    public void Init()
    {
        this.svnmsg = this.svnmsg.Replace('【','[');
        this.svnmsg = this.svnmsg.Replace('】',']');
        Regex r = new Regex(@"\[(.*?)\]");
        MatchCollection ms = r.Matches(this.svnmsg);
        foreach (Match match in ms)
        {
            GroupCollection groups = match.Groups;
            //Debug.Log(groups[1].Value);
            int num ;
            if (int.TryParse(groups[1].Value,out num))
            {
                order.Add(num);
                verDic[num] = svnrevision;
            }
        }
    }
    public int GetVersion(int ordernum)
    {
        for (int i = 0; i < order.Count; i++)
        {
            if (ordernum == order[i])
            {
                return svnrevision;
            }
        }

        return 0;
    }

    public int GetVersion()
    {
        return svnrevision;
    }

    public string GetMsg()
    {
        return svnmsg;
    }
}
