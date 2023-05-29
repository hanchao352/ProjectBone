

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

public class ProtoManager:Singleton<ProtoManager>
{
     List<ProtoInfo> ProtoInfos = new List<ProtoInfo>();
     string ProtoDirPath = @"F:\GameProject\PtotoFiles";
    List<string> excludedFiles = new List<string> { "standmsgid.proto" };
    string[] _filePaths;
    public string curseletProtoPath;
    public override void Initialize()
    {
        base.Initialize();
        InitProtoInfos();
    }
    private void InitProtoInfos()
    {
        ProtoInfos.Clear();
            
          
        _filePaths  = Directory.GetFiles(ProtoDirPath, "*.proto")
            .Where(filePath => !excludedFiles.Contains(Path.GetFileName(filePath))).ToArray();
        for (int i = 0; i < _filePaths.Length; i++)
        {
            string protopath = _filePaths[i];
            ProtoInfo protoInfo = ProtoParser.ParseProtoFile(protopath);
            ProtoInfos.Add(protoInfo);
        }
        ProtoInfos.Sort(SortByMsgId);
    }

    public List<ProtoInfo> UpdateProtoInfos()
    {
        ProtoInfos.Clear();
            
          
        _filePaths  = Directory.GetFiles(ProtoDirPath, "*.proto")
            .Where(filePath => !excludedFiles.Contains(Path.GetFileName(filePath))).ToArray();
        for (int i = 0; i < _filePaths.Length; i++)
        {
            string protopath = _filePaths[i];
            ProtoInfo protoInfo = ProtoParser.ParseProtoFile(protopath);
            ProtoInfos.Add(protoInfo);
        }
        ProtoInfos.Sort(SortByMsgId);
        return ProtoInfos;
    }

    
    public List<ProtoInfo> GetProtoInfos()
    {
        return ProtoInfos;
    }
    
    private int SortByMsgId(ProtoInfo x, ProtoInfo y)
    {
        int id1= x.Messages.Count<=0?0:x.Messages[0].MsgID;
        int id2= y.Messages.Count<=0?0:y.Messages[0].MsgID;
        return id1.CompareTo(id2);
    }

    public string GetProtoDirPath()
    {
        return ProtoDirPath;
    }

    public void ExportProtos()
    {
        RunBatFile(@"..\..\..\..\..\Tools\proto2csforClient.bat");
        RunBatFile(@"..\..\..\..\..\Tools\proto2csforServer.bat");
    }

    public  void RunBatFile(string relativeBatFilePath)
    {
        // 获取当前应用程序的exe所在目录
        string exePath = AppDomain.CurrentDomain.BaseDirectory;
        Console.WriteLine("----------------:"+exePath);
        string exeDirectory = Path.GetDirectoryName(exePath);
        Console.WriteLine("----------------:"+exeDirectory);
        // 将相对路径转换为绝对路径
        string batFilePath = Path.GetFullPath(Path.Combine(exeDirectory, relativeBatFilePath));
        string file = Path.GetFileName(batFilePath);
        string dir = Path.GetDirectoryName(batFilePath);
        Console.WriteLine(dir);
        batFilePath = batFilePath.Replace("/", "\\");
        Console.WriteLine("path:"+batFilePath);
        // 创建一个新的Process实例
        try
        {
            Process process = new Process();
            process.StartInfo.FileName = @"cmd.exe";
            process.StartInfo.Arguments = $"/c .\\{file}";
            process.StartInfo.WorkingDirectory = dir; // 批处理文件所在的目录
            process.StartInfo.UseShellExecute = true;
            // process.StartInfo.RedirectStandardOutput = true;
            // process.StartInfo.RedirectStandardError = true;
            process.StartInfo.CreateNoWindow = false;
            process.Start();
        
           
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
       
    }

    public  void GenerateMsgIDCSharpFileForClient( )
    {
        string exePath = AppDomain.CurrentDomain.BaseDirectory;
        string exeDirectory = Path.GetDirectoryName(exePath);
       
        // 定义相对路径
        string relativePath = @"..\..\..\..\..\Project\Assets\Scripts\Define";

        // 根据exe所在目录和相对路径组合得到完整路径
        string fullPath = Path.GetFullPath(Path.Combine(exeDirectory, relativePath));
        fullPath =fullPath+"\\"+"ProtosMsgID.cs";
        // 打印完整路径
        StringBuilder fileContent = new StringBuilder();

        // 添加命名空间和类定义
        fileContent.AppendLine("public static class ProtosMsgID");
        fileContent.AppendLine("{");

        // 获取所有msgid大于0的MessageInfo，并按照MsgID从小到大排序
        var sortedMessages = ProtoInfos.SelectMany(protoInfo => protoInfo.Messages)
            .Where(messageInfo => messageInfo.MsgID > 0)
            .OrderBy(messageInfo => messageInfo.MsgID)
            .ToList();

        // 遍历所有筛选后的MessageInfo，将每个MessageInfo的Name和MsgID添加到类中
        foreach (var messageInfo in sortedMessages)
        {
            fileContent.AppendLine($"    public const int {messageInfo.Name} = {messageInfo.MsgID};");
        }

        fileContent.AppendLine("}");

       
        File.WriteAllText(fullPath, fileContent.ToString());
    }
    
    public  void GenerateMsgIDCSharpFileForServer( )
    {
        string exePath = Assembly.GetExecutingAssembly().Location;
        string exeDirectory = Path.GetDirectoryName(exePath);

        // 定义相对路径
        string relativePath = @"..\..\..\..\..\GameServer\Scripts\Define";

        // 根据exe所在目录和相对路径组合得到完整路径
        string fullPath = Path.GetFullPath(Path.Combine(exeDirectory, relativePath));
        fullPath =fullPath+"\\"+"ProtosMsgID.cs";
        // 打印完整路径
        StringBuilder fileContent = new StringBuilder();

        // 添加命名空间和类定义
        fileContent.AppendLine("public static class ProtosMsgID");
        fileContent.AppendLine("{");

        // 获取所有msgid大于0的MessageInfo，并按照MsgID从小到大排序
        var sortedMessages = ProtoInfos.SelectMany(protoInfo => protoInfo.Messages)
            .Where(messageInfo => messageInfo.MsgID > 0)
            .OrderBy(messageInfo => messageInfo.MsgID)
            .ToList();

        // 遍历所有筛选后的MessageInfo，将每个MessageInfo的Name和MsgID添加到类中
        foreach (var messageInfo in sortedMessages)
        {
            fileContent.AppendLine($"    public const int {messageInfo.Name} = {messageInfo.MsgID};");
        }

        fileContent.AppendLine("}");

       
        File.WriteAllText(fullPath, fileContent.ToString());
    }
}