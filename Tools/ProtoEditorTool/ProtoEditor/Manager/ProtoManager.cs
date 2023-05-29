

using Google.Protobuf.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.SymbolStore;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using static ProtoInfo;
using static System.Windows.Forms.Design.AxImporter;

public class Person
{
    public string name;
    public int age;
}

public class ProtoManager:Singleton<ProtoManager>
{
     List<ProtoInfo> ProtoInfos = new List<ProtoInfo>();
     string ProtoDirPath = string.Empty;
    List<string> excludedFiles = new List<string> { "OptionMsgId.proto" };
    string[] _filePaths;
    public string curseletProtoPath;
    List<ProtoFileInfo> fileDescriptorProtos = new List<ProtoFileInfo>();
    public List<int> MsgIdList = new List<int>();
    public List<string> MsgNameList= new List<string>();
    public override void Initialize()
    {
        base.Initialize();
        InitPath();
        //InitProtoInfos();
        ParseProtoFile();
    }
    void InitPath()
    {
        string relativeDirPath = @"..\..\..\..\..\..\PtotoFiles";
        string exePath = AppDomain.CurrentDomain.BaseDirectory;
        Console.WriteLine("----------------:" + exePath);
        string exeDirectory = Path.GetDirectoryName(exePath);
        Console.WriteLine("----------------:" + exeDirectory);
        // 将相对路径转换为绝对路径
         ProtoDirPath = Path.GetFullPath(Path.Combine(exeDirectory, relativeDirPath));
      
    }
    






    private void InitProtoInfos()
    {
        ParseProtoFile();
       
    }

    private void ParseProtoFile()
    {

        fileDescriptorProtos.Clear();
        _filePaths = Directory.GetFiles(ProtoDirPath, "*.proto")
            .Where(filePath => !excludedFiles.Contains(Path.GetFileName(filePath))).ToArray();
       

        for (int i = 0; i < _filePaths.Length; i++)
        {
            ProtoFileInfo protoFileInfo = ParseProtoFile(_filePaths[i]);
            fileDescriptorProtos.Add(protoFileInfo);
        }
     
    }

 

    private ProtoFileInfo ParseProtoFile(string protofilepath)
    {
        FileDescriptorSet schema = new FileDescriptorSet();

      
            string protopath = protofilepath;

            string fileName = Path.GetFileName(protopath);
            string protoContent = File.ReadAllText(protopath);
            schema.Add(fileName, true, new StringReader(protoContent));

        
        schema.Process();
        if (schema == null)
        {
            return null;
        }
        
            ProtoFileInfo protoFileInfo = new ProtoFileInfo();
            protoFileInfo.ProtoPath = protofilepath;
            protoFileInfo.ProtoField = schema.Files[0];
            protoFileInfo.Init();
          return protoFileInfo;
        
    }
    public ProtoFileInfo TryParseProtoFile(string protofilepath,string strvalue)
    {
        FileDescriptorSet schema = new FileDescriptorSet();


        string protopath = protofilepath;

        string fileName = Path.GetFileName(protopath);
        string protoContent = strvalue;
        schema.Add(fileName, true, new StringReader(protoContent));


        schema.Process();
        if (schema == null)
        {
            return null;
        }
        FileDescriptorProto protofile = schema.Files[0];
        var duplicateNames = protofile.MessageTypes.GroupBy(proto => proto.Name)
                                .Where(group => group.Count() > 1)
                                .Select(group => group.Key)
                                .ToList();

        if (duplicateNames.Count > 0)
        {

            MessageBox.Show(string.Format(StringDefine.ProtoWindow_HaveSameMsgName, duplicateNames[0]) , StringDefine.ProtoWindow_OKText);
            return null;
           
        }


        var duplicateMsgId= protofile.MessageTypes.GroupBy(proto => proto.Options?.UninterpretedOptions[0].AggregateValue)
                               .Where(group => group.Count() > 1)
                               .Select(group => group.Key)
                               .ToList();

        if (duplicateMsgId.Count > 0)
        {

            MessageBox.Show(string.Format(StringDefine.ProtoWindow_HaveSameMsgID, duplicateMsgId[0]), StringDefine.ProtoWindow_OKText);
            return null;
        }
        List<ProtoFileInfo> templist= new List<ProtoFileInfo>();
        for (int i = 0; i < fileDescriptorProtos.Count; i++)
        {
            if (fileDescriptorProtos[i].ProtoPath != protofilepath )
            {
                templist.Add(fileDescriptorProtos[i]);
            }
        }
         var messageinfo1 = templist.SelectMany(protofiles => protofiles.ProtoField.MessageTypes);
        var messageinfo2 = protofile.MessageTypes;

        var commonNames = messageinfo1.Where(message => messageinfo2.Any(messageinfo2 => message.Name == messageinfo2.Name))
                                      .Select(msginfo => msginfo.Name)
                                      .Distinct()
                                      .ToList();
       
        
        var commonIDS = messageinfo1.Where(message => messageinfo2.Any(messageinfo2 => message.Options!=null && message.Options.UninterpretedOptions!=null && message.Options.UninterpretedOptions[0] != null &&  message.Options.UninterpretedOptions?[0].AggregateValue == messageinfo2.Options?.UninterpretedOptions?[0].AggregateValue))
                                     .Select(msginfo => msginfo.Options?.UninterpretedOptions?[0].AggregateValue)
                                     .Distinct()
                                     .ToList();
        if (commonNames.Count > 0)
        {
            MessageBox.Show(string.Format(StringDefine.ProtoWindow_HaveSameMsgName, commonNames[0]), StringDefine.ProtoWindow_OKText);
            return null;
        }
       
        if (commonIDS.Count > 0)
        {
            MessageBox.Show(string.Format(StringDefine.ProtoWindow_HaveSameMsgID, commonIDS[0]), StringDefine.ProtoWindow_OKText);
            return null;
        }
       
        ProtoFileInfo protoFileInfo = new ProtoFileInfo();
        protoFileInfo.ProtoPath = protofilepath;
        protoFileInfo.ProtoField = protofile;
        protoFileInfo.Init();
        return protoFileInfo;

    }
    public List<ProtoFileInfo> UpdateProtoInfos()
    {
        ParseProtoFile();
        fileDescriptorProtos.Sort(SortByMinID);
        return fileDescriptorProtos;
    }

    
    public List<ProtoFileInfo> GetProtoInfos()
    {
        fileDescriptorProtos.Sort(SortByMinID);
        return fileDescriptorProtos;
    }
    


    public string GetProtoDirPath()
    {
        return ProtoDirPath;
    }

    public void ExportProtos()
    {
        RunBatFile(@"..\..\..\..\..\..\Tools\proto2csforClient.bat");
        RunBatFile(@"..\..\..\..\..\..\Tools\proto2csforServer.bat");
    }

    public void ExportToClient()
    {
        RunBatFile(@"..\..\..\..\..\..\Tools\proto2csforClient.bat");
    }

    public void ExportToServer()
    {
        RunBatFile(@"..\..\..\..\..\..\Tools\proto2csforServer.bat");
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
        string relativePath = @"..\..\..\..\..\..\Project\Assets\Scripts\Define";

        // 根据exe所在目录和相对路径组合得到完整路径
        string fullPath = Path.GetFullPath(Path.Combine(exeDirectory, relativePath));
        fullPath =fullPath+"\\"+"ProtosMsgID.cs";
        // 打印完整路径
        StringBuilder fileContent = new StringBuilder();

        // 添加命名空间和类定义
        fileContent.AppendLine("public static class ProtosMsgID");
        fileContent.AppendLine("{");

        // 获取所有msgid大于0的MessageInfo，并按照MsgID从小到大排序
        fileDescriptorProtos.Sort(SortByMinID);
        // 遍历所有筛选后的MessageInfo，将每个MessageInfo的Name和MsgID添加到类中
        for (int i = 0; i < fileDescriptorProtos.Count; i++)
        {
            for (int j = 0; j < fileDescriptorProtos[i].ProtoField.MessageTypes.Count; j++)
            {
                if (fileDescriptorProtos[i].ProtoField.MessageTypes[j].Options!=null && fileDescriptorProtos[i].ProtoField.MessageTypes[j].Options.UninterpretedOptions!=null)
                {
                    fileContent.AppendLine($"    public const int {fileDescriptorProtos[i].ProtoField.MessageTypes[j].Name} = {fileDescriptorProtos[i].ProtoField.MessageTypes[j].Options.UninterpretedOptions[0].AggregateValue};");
                }
            }
           
        }
       

        fileContent.AppendLine("}");

       
        File.WriteAllText(fullPath, fileContent.ToString());
    }

    private int SortByMinID(ProtoFileInfo x, ProtoFileInfo y)
    {
        return x.MinMsgId.CompareTo(y.MinMsgId);
    }

    public  void GenerateMsgIDCSharpFileForServer( )
    {
        string exePath = Assembly.GetExecutingAssembly().Location;
        string exeDirectory = Path.GetDirectoryName(exePath);

        // 定义相对路径
        string relativePath = @"..\..\..\..\..\..\GameServer\Scripts\Define";

        // 根据exe所在目录和相对路径组合得到完整路径
        string fullPath = Path.GetFullPath(Path.Combine(exeDirectory, relativePath));
        fullPath =fullPath+"\\"+"ProtosMsgID.cs";
        // 打印完整路径
        StringBuilder fileContent = new StringBuilder();

        // 添加命名空间和类定义
        fileContent.AppendLine("public static class ProtosMsgID");
        fileContent.AppendLine("{");

        fileDescriptorProtos.Sort(SortByMinID);
        // 遍历所有筛选后的MessageInfo，将每个MessageInfo的Name和MsgID添加到类中
        for (int i = 0; i < fileDescriptorProtos.Count; i++)
        {
            for (int j = 0; j < fileDescriptorProtos[i].ProtoField.MessageTypes.Count; j++)
            {
                if (fileDescriptorProtos[i].ProtoField.MessageTypes[j].Options != null && fileDescriptorProtos[i].ProtoField.MessageTypes[j].Options.UninterpretedOptions != null)
                {
                    fileContent.AppendLine($"    public const int {fileDescriptorProtos[i].ProtoField.MessageTypes[j].Name} = {fileDescriptorProtos[i].ProtoField.MessageTypes[j].Options.UninterpretedOptions[0].AggregateValue};");
                }
            }

        }

        fileContent.AppendLine("}");

       
        File.WriteAllText(fullPath, fileContent.ToString());
    }

   
    public List<ProtoFileInfo> GetProtoInfosByMinIDAndMaxID(int minid, int maxid)
    {
        List<ProtoFileInfo> list = new List<ProtoFileInfo>();
        for (int i = 0; i < fileDescriptorProtos.Count; i++)
        {
            ProtoFileInfo protoInfo = fileDescriptorProtos[i];
            if (protoInfo.MaxMsgId <= maxid && protoInfo.MinMsgId >= minid)
            {
                list.Add(protoInfo);
            }
        }
       
        return list;
    }
    public void CreatProtoFile()
    {
        
    }
    private void RenameFile(string sourceFilePath, string newFileName)
    {
        if (File.Exists(sourceFilePath))
        {
            // 获取文件所在的目录路径
            string folderPath = Path.GetDirectoryName(sourceFilePath);

            // 生成新的文件路径
            string newFilePath = Path.Combine(folderPath, newFileName);

            // 使用 File.Move 方法重命名文件
            File.Move(sourceFilePath, newFilePath);
        }
        else
        {
            Console.WriteLine("文件不存在: " + sourceFilePath);
        }
    }
    public void ReNameProto(string filepath,string newname)
    {
        string name = newname.Split('.')[0];
        RenameFile(filepath, name+".proto");
    }
}