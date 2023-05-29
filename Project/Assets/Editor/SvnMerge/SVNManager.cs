using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

public class SVNManager
{
   private static SVNManager instanc;
   private  string trunkpath ;
   private  string branchpath;

   public string Trunkpath
   {
      get {  
         
            if (string.IsNullOrEmpty(trunkpath))
            {
                string path =new DirectoryInfo(Environment.CurrentDirectory + "/../../../../").FullName+"client\\goe\\client\\Lua";
               string path2 =path.Replace("\\", "/");;
               trunkpath = "F:/Project/trunk/Project"; //path2;
            }

            return trunkpath;
         
            }
      
      
   }

   public string Branchpath
   {
      get
      {
         if (string.IsNullOrEmpty(branchpath))
         {
            branchpath = "F:/Project/branches/Project"; //"G:/online/trunk/code/client/goe/client/Lua";
         }

         return branchpath;
      }

   }

   private List<SVNLogInfo> allog = new List<SVNLogInfo>();
   private List<SVNLogInfo> needmergelist = new List<SVNLogInfo>();
   public void Init()
   {
      string path =new DirectoryInfo(Environment.CurrentDirectory + "/../../../../").FullName+"client\\goe\\client\\Lua";
      string path2 =path.Replace("\\", "/");;
      trunkpath = "F:/Project/trunk/Project";//path2;
      branchpath ="F:/Project/branches/Project";//"G:/online/trunk/code/client/goe/client/Lua";
   }

   public SVNManager()
   {
     
   }
   public static SVNManager Instance()
   {
      if (instanc ==null)
      {
         instanc = new SVNManager();
      }

      return instanc;
   }

   public void UpdateBranchSVN()
   {
      SVNUtil.RunUpdateCmd(Branchpath);
   }
   public void UpdateTrunkSVN()
   {
      SVNUtil.RunUpdateCmd(Trunkpath);
   }

   public void MergeBranch(string cmd)
   {
      SVNUtil.RunMergeCmd(cmd,Trunkpath,Branchpath);
   }

   public void CommitBranchSvn(string commitmsg)
   {
      SVNUtil.RunCommitCmd(Branchpath,commitmsg);
     
     
   }
   public void CommitTrunkSvn(string commitmsg)
   {
      SVNUtil.RunCommitCmd(Trunkpath,commitmsg);
   }

   public List<SVNLogInfo> GetSvnLogInfos()
   {
      List<SVNLogInfo> infos = new List<SVNLogInfo>();
      XmlNodeList list = GetSVNLog(Trunkpath);
      for (int i = 0; i < list.Count; i++)
      {
         SVNLogInfo svnLogInfo = new SVNLogInfo(list[i]);
         infos.Add(svnLogInfo);
      }

      return infos;
   }
   
   /// <summary>
   /// 根据单号查找svn 提交 version
   /// </summary>
   /// <param name="order"></param>
   public void QueryOrder(string Ordernumber)
   {
      
      if (string.IsNullOrEmpty(Ordernumber))
      {
         return;
      }
      allog.Clear();
      needmergelist.Clear();
      Ordernumber =  Ordernumber.Replace("，",",");
      string[] querlist = Ordernumber.Split(new Char[] { ',' },StringSplitOptions.RemoveEmptyEntries);
       allog = GetSvnLogInfos();
      for (int i = 0; i < querlist.Length; i++)
      {
         int order;
         if (int.TryParse(querlist[i],out order))
         {
            for (int j = 0; j < allog.Count; j++)
            {
               if ( allog[j].verDic.ContainsKey(order))
               {
                  //verlist.Add( allog[j].verDic[order]);
                  needmergelist.Add( allog[j]);
               }
              
            }
         }
      }
      
   }

   public string GetMergeVersionStr()
   {
      string str = string.Empty;
      for (int i = 0; i < needmergelist.Count; i++)
      {
         string s = i == needmergelist.Count - 1 ? "" : ",";
         str += needmergelist[i].GetVersion().ToString() + s;
      }
      return str;
   }
   public string GetMergeVersionStrForCmd()
   {
      string str = string.Empty;
      for (int i = 0; i < needmergelist.Count; i++)
      {
         string s = i == needmergelist.Count - 1 ? "" : ",";
         str += "r"+needmergelist[i].GetVersion().ToString() + s;
      }
      return str;
   }
   public string GetMergeMsgStr()
   {
      string str = string.Empty;
      string[] revisions = new string[needmergelist.Count];
      if (needmergelist.Count == 0)
      {
         return "";
      }
      for (int i = 0; i < revisions.Length; i++)
      {
         revisions[i] = needmergelist[i].GetVersion().ToString();
      }
      str="Merge trunk r" + string.Join(",", revisions) + " changes into"+Branchpath+"\r\n"; 
      for (int i = 0; i < needmergelist.Count; i++)
      {
         string s = i == needmergelist.Count - 1 ? "" : ",";
         str += needmergelist[i].GetMsg().ToString() + "\r\n";
      }
      return str;
   }
   private int SortLogList(SVNLogInfo x, SVNLogInfo y)
   {
      return x.GetVersion() - y.GetVersion();
   }

   public List<SVNLogInfo> GetNeedLogList()
   {
      return needmergelist;
   }

   public XmlNodeList GetSVNLog(string path)
   {
      return SVNUtil.RunCmdToXML("svn log -r {20230318}:HEAD --xml --with-all-revprops ",path);
   }

   public void OneKeyUpdateAndMergeAndCommit(string commitmsg)
   {
      string[] revisions = new string[needmergelist.Count];
      if (needmergelist.Count == 0)
      {
         return;
      }
      for (int i = 0; i < revisions.Length; i++)
      {
         revisions[i] = needmergelist[i].GetVersion().ToString();
      }
      SVNUtil.OneKeyMergeAndCommit(revisions,commitmsg);
   }
}
