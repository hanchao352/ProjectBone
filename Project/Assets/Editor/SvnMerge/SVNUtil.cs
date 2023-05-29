using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using UnityEngine;
using Debug = UnityEngine.Debug;

public static class SVNUtil 
{
   public static void RunMergeCmd(string command,string sorcepath,string targetpath)
   {
      Process p = new Process();
      p.StartInfo.FileName = "cmd.exe";         //确定程序名
      p.StartInfo.Arguments = "/c" +" "+ command +" "+ sorcepath+" "+targetpath; //Application.dataPath + path;
      p.StartInfo.UseShellExecute = false;      //Shell的使用
      p.StartInfo.RedirectStandardInput = true;  //重定向输入
      p.StartInfo.RedirectStandardOutput = true; //重定向输出
      p.StartInfo.StandardOutputEncoding = Encoding.Default;
      p.StartInfo.RedirectStandardError = true;  //重定向输出错误
      p.StartInfo.CreateNoWindow = true;        //设置置不显示示窗口
      p.StartInfo.StandardOutputEncoding = Encoding.UTF8; //Encoding.GetEncoding("GBK");
      p.Start();
      p.WaitForExit();
      p.Close();
     
   }

   public static void RunCommitCmd(string localPath,string commitmsg)
   {
      
      
     
   }
   static string[] GetChangedFiles(string command)
   {
      var startInfo = new ProcessStartInfo();
      startInfo.FileName = "cmd.exe";
      startInfo.Arguments = "/c " + command;
      startInfo.UseShellExecute = false;
      startInfo.RedirectStandardOutput = true;

      var process = new Process();
      process.StartInfo = startInfo;
      process.Start();

      var output = process.StandardOutput.ReadToEnd();

      process.WaitForExit();

      // 过滤出所有有变动的文件
      var files = output
         .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
         .Where(line => line.Length > 7 && line[0] != ' ')
         .Select(line => line.Substring(8))
         .ToArray();

      return files;
   }
   // 获取指定文件夹下被删除的文件
   static string[] GetDeletedFiles(string folder)
   {
      var startInfo = new ProcessStartInfo();
      startInfo.FileName = "svn.exe";
      startInfo.Arguments = "status --no-ignore " + folder;
      startInfo.UseShellExecute = false;
      startInfo.RedirectStandardOutput = true;

      var process = new Process();
      process.StartInfo = startInfo;
      process.Start();

      var output = process.StandardOutput.ReadToEnd();

      process.WaitForExit();

      // 过滤出被删除的文件
      var files = output
         .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
         .Where(line => line.Length > 7 && line[0] == '!')
         .Select(line => line.Substring(8))
         .ToArray();

      return files;
   }
   public static void RunUpdateCmd(string targetpath)
   {
      Process p = new Process();
      p.StartInfo.FileName = "cmd.exe";         //确定程序名
      p.StartInfo.Arguments = "/c" +" "+"svn update" +" "+targetpath; //Application.dataPath + path;
      p.StartInfo.UseShellExecute = false;      //Shell的使用
      p.StartInfo.RedirectStandardInput = true;  //重定向输入
      p.StartInfo.RedirectStandardOutput = true; //重定向输出
      p.StartInfo.StandardOutputEncoding = Encoding.Default;
      p.StartInfo.RedirectStandardError = true;  //重定向输出错误
      p.StartInfo.CreateNoWindow = false;        //设置置不显示示窗口
      p.StartInfo.StandardOutputEncoding = Encoding.UTF8; //Encoding.GetEncoding("GBK");
      p.Start();
      p.WaitForExit();
      p.Close();
     
   }

   public static XmlNodeList RunCmdToXML(string command,string path)
   {

            
      Process p = new Process();
            
      p.StartInfo.FileName = "cmd.exe";         //确定程序名
      p.StartInfo.Arguments = "/c" + command + path; //Application.dataPath + path;
      //"F:/shenmo2project/trunk/public/res/editor/Assets/Res/Effect/Character_Prefab/Monster/b_maerwaduo/E_maerwaduo_rongyandiyu_jian.prefab"; //command+" "+Application.dataPath+"/"+path ;   //确定程式命令行
      p.StartInfo.UseShellExecute = false;      //Shell的使用
      
      p.StartInfo.RedirectStandardInput = true;  //重定向输入
      p.StartInfo.RedirectStandardOutput = true; //重定向输出
      p.StartInfo.StandardOutputEncoding = Encoding.Default;
      p.StartInfo.RedirectStandardError = true;  //重定向输出错误
      p.StartInfo.CreateNoWindow = true;        //设置置不显示示窗口
      p.StartInfo.StandardOutputEncoding = Encoding.UTF8; //Encoding.GetEncoding("GBK");
      p.Start();
      //curencod= p.StandardOutput.CurrentEncoding;
     
      string s = p.StandardOutput.ReadToEnd();
      string s1 = p.StandardOutput.ReadLine();
      p.WaitForExit();
      p.Close();
      XmlDocument xmlDocument = new XmlDocument();
      xmlDocument.LoadXml(s);
      XmlNodeList list= xmlDocument.GetElementsByTagName("logentry");
      return list; //p.StandardOutput.ReadToEnd();      //输出出流取得命令行结果果
   }
   public static  void ShellExcute(string fileName, string arguments = "", string workpath = null, bool waitForEnd = false)
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

   public static void OneKeyMergeAndCommit(string[] revisionNumbers,string commitmsg)
   {
      
      var processStartInfo = new ProcessStartInfo
      {
         FileName = "cmd.exe",
         Arguments = $"/C svn update {SVNManager.Instance().Branchpath}",
         RedirectStandardOutput = true,
         UseShellExecute = false,
         CreateNoWindow = true
      };
      var process = Process.Start(processStartInfo);
      process.WaitForExit();

      // 从主干合并多个指定版本代码到分支
      string revisions = string.Join(",", revisionNumbers);
      processStartInfo.Arguments = $"/C svn merge -c {revisions} {SVNManager.Instance().Trunkpath} {SVNManager.Instance().Branchpath}";
      process = Process.Start(processStartInfo);
      process.WaitForExit();

     ////提交
     
     string svnPath = @"C:\Program Files\TortoiseSVN\bin\svn.exe"; // svn.exe 的路径
     string workingCopyPath = SVNManager.Instance().Branchpath; // 工作副本的路径
     string logMessage = commitmsg; // 提交日志信息

     // 执行 svn status 命令查找有变动的文件
     ProcessStartInfo processInfo = new ProcessStartInfo(svnPath, "status -q -u " + workingCopyPath);
     processInfo.CreateNoWindow = true;
     processInfo.UseShellExecute = false;
     processInfo.RedirectStandardOutput = true;

     //Process process = new Process();
     process.StartInfo = processInfo;
     process.Start();

     string output = process.StandardOutput.ReadToEnd();
     process.WaitForExit();

     // 将有变动的文件添加到提交列表
     string[] files = output.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
     foreach (string file in files) {
        if (file.StartsWith("M") || file.StartsWith("A") || file.StartsWith("D")) {
           string filePath = file.Substring(8).Trim();
           Process.Start(svnPath, "add \"" + filePath + "\""); // 添加新文件或修改的文件
           Process.Start(svnPath, "delete \"" + filePath + "\""); // 删除的文件
        }
     }

     // 执行 svn commit 命令提交所有变动的文件
     Process.Start(svnPath, "commit -m \"" + logMessage + "\" " + workingCopyPath);
      
     
   }

   public static void Commit(string commitmsg)
   {
      
      string svnPath = @"C:\Program Files\TortoiseSVN\bin\svn.exe"; // svn.exe 的路径
      string workingCopyPath = SVNManager.Instance().Branchpath; // 工作副本的路径
      string logMessage = commitmsg; // 提交日志信息

      // 执行 svn status 命令查找有变动的文件
      ProcessStartInfo processInfo = new ProcessStartInfo(svnPath, "status -q -u " + workingCopyPath);
      processInfo.CreateNoWindow = true;
      processInfo.UseShellExecute = false;
      processInfo.RedirectStandardOutput = true;

      Process process = new Process();
      process.StartInfo = processInfo;
      process.Start();

      string output = process.StandardOutput.ReadToEnd();
      process.WaitForExit();

      // 将有变动的文件添加到提交列表
      string[] files = output.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
      foreach (string file in files) {
         if (file.StartsWith("M") || file.StartsWith("A") || file.StartsWith("D")) {
            string filePath = file.Substring(8).Trim();
            Process.Start(svnPath, "add \"" + filePath + "\""); // 添加新文件或修改的文件
            Process.Start(svnPath, "delete \"" + filePath + "\""); // 删除的文件
         }
      }

      // 执行 svn commit 命令提交所有变动的文件
      Process.Start(svnPath, "commit -m \"" + logMessage + "\" " + workingCopyPath);
   }
}
