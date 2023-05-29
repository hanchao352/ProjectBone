using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class TreeDateManager:Singleton<TreeDateManager>
{
     
     TreeDate TreeDate = null;
     string JsonPath = AppDomain.CurrentDomain.BaseDirectory;
     string JsonFileName = "RootNodeList.json";
    string filepath=string.Empty;
    public override void Initialize()
    {
        base.Initialize();    
         filepath = $"{JsonPath}/{JsonFileName}";
        if (File.Exists(filepath))
        {
            string jsonstr = File.ReadAllText(filepath);
            TreeDate = JsonConvert.DeserializeObject<TreeDate>(jsonstr);
        }
        if (TreeDate == null)
        {
            TreeDate = new TreeDate();
        }
      
        
    }
    
    public TreeDate GetTreeDate() 
    {
        return TreeDate;
    }


    public TreeDate AddRootNodeInfo(RootNodeInfo rootNodeInfo)
    {
        TreeDate.AddRootInfo(rootNodeInfo);
        SaveJson();
        return TreeDate;
    }
    public TreeDate RemoveRootNodeInfo(RootNodeInfo rootNodeInfo)
    {
        TreeDate.RemoveRootInfo(rootNodeInfo);
        SaveJson();
        return TreeDate;
    }

    public TreeDate UpdateRootNodeInfo(RootNodeInfo rootNodeInfo)
    {
        TreeDate.UpdateRootNodeInfo(rootNodeInfo);
        SaveJson();

        return TreeDate;
    }

    private void SaveJson()
    {
        string jsonstr= JsonConvert.SerializeObject(TreeDate, Formatting.Indented);
        File.WriteAllText(filepath, jsonstr);
    }

}

