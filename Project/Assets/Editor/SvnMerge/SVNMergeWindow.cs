using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Policy;
using System.Text;
using System.Xml;
using Sirenix.OdinInspector.Editor;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using Sirenix.Utilities;
using UnityEditor;
using Debug = UnityEngine.Debug;
//[Serializable,Toggle("Enabled")]
public class SVNMergeWindow
{
   private List<SVNLogInfo> infolist = new List<SVNLogInfo>();
   private string mergeversion;
   
   [TextArea(5,5)]
   [PropertyTooltip("提交时的单号")]
   [LabelText("查询单号")]
   [PropertyOrder(0)]
   public string Ordernumber;

   
   [TextArea(5, 5)] 
   [LabelText("满足单号的查询结果")]
   public string svnversion;
   [TextArea(5, 10)] 
   [LabelText("合并日志")]
   public string mergelog;
   [Button("查询")]
   public void QuerySvn()
   {  
      updatedone = false;
      mergedone = false;
      commitdone = false;
      if (string.IsNullOrEmpty(Ordernumber))
      {
         EditorUtility.DisplayDialog("警告","查询单号不能为空","确定");
         return;
      }
      SVNManager.Instance().QueryOrder(Ordernumber);
      List<SVNLogInfo> needlist = SVNManager.Instance().GetNeedLogList();
      if (needlist.Count<=0)
      {
         EditorUtility.DisplayDialog("警告","没有满足单号的提交记录","确定");
         return;
      }
      
      svnversion = SVNManager.Instance().GetMergeVersionStr();
      mergelog = SVNManager.Instance().GetMergeMsgStr();
    
   }

   
 
  [HorizontalGroup("svn状态", 100,0,100)]
  [LabelText("分支更新完毕")] 
   [LabelWidth(100)] 
   [ReadOnly]
   public bool updatedone; 
   [HorizontalGroup("svn状态", 100,0,100)]
   [LabelText("分支合并完毕")]
   [LabelWidth(100)]
   [ReadOnly]
   public bool mergedone = false;
   [HorizontalGroup("svn状态", 100,0,100)]
   [LabelText("分支提交完毕")]
   [LabelWidth(100)]
   [ReadOnly]
   public bool commitdone = false;
  
   [Button("更新分支")]
   public void UpdateBranch()
   {
      string branchpath = "G:/online/trunk/code/client/goe/client/Lua";
      //ShellExcute("TortoiseProc.exe", "/command:update /path:" +branchpath+" /closeonend:2",null,true);
      SVNManager.Instance().UpdateBranchSVN();
      updatedone = true;
   }
   
   [ButtonGroup()]
   [Button(("开始合并分支(显示合并界面)"))]
   public void StartMerge()
   {
      if (string.IsNullOrEmpty(Ordernumber))
      {
         EditorUtility.DisplayDialog("警告","查询单号不能为空","确定");
         return;
      }
      UpdateBranch();
      
      string url = "http://10.12.20.20/svn/MMOFW2/trunk/code/client/goe/client/Lua";
      string branchpath = "G:/online/trunk/code/client/goe/client/Lua";
      ShellExcute("TortoiseProc.exe", "/command:merge /path:" +branchpath+" /fromurl:"+url+" /revrange:"+svnversion,null,true );
      mergedone = true;
   }
  
   [ButtonGroup]
   [Button(("开始合并分支(不显示界面)"))]
   public void StartMergeNoUI()
   {
      if (string.IsNullOrEmpty(Ordernumber))
      {
         EditorUtility.DisplayDialog("警告","查询单号不能为空","确定");
         return;
      }
      UpdateBranch();
      string mergeversion = SVNManager.Instance().GetMergeVersionStrForCmd();
      SVNManager.Instance().MergeBranch($"svn merge -c {mergeversion}");
      mergedone = true;


   }
  
  
 
   [Button("提交分支")]
   public void CommitBranch()
   {
      if (string.IsNullOrEmpty(Ordernumber))
      {
         EditorUtility.DisplayDialog("警告","查询单号不能为空","确定");
         return;
      }
      string branchpath = "G:/online/trunk/code/client/goe/client/Lua";
     // SVNUtil.RunCommitCmd(branchpath,mergeversion);
     SVNManager.Instance().CommitBranchSvn(mergelog);
     commitdone = true;
   }
 
   [Button("一键合并分支")]
   public void OnKeyCommitBranch()
   {
      if (string.IsNullOrEmpty(Ordernumber))
      {
         EditorUtility.DisplayDialog("警告","查询单号不能为空","确定");
         return;
      }
      SVNManager.Instance().OneKeyUpdateAndMergeAndCommit(mergelog);
   }
   public  void ShellExcute(string fileName, string arguments = "", string workpath = null, bool waitForEnd = false)
   {
      
      ProcessStartInfo processStartInfo = new ProcessStartInfo(fileName, arguments);
      processStartInfo.UseShellExecute = true;
      if (workpath != null)
      {
         processStartInfo.WorkingDirectory = workpath;
      }
      Process process = Process.Start(processStartInfo);
      if (waitForEnd)
      {
         process.WaitForExit();
      }
   }

  




}
