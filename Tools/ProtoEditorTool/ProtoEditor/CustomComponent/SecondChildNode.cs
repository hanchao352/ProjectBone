using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


 public class SecondChildNode : TreeNode
{
    public ProtoFileInfo ProtoInfo;
    public string path = string.Empty;
    public SecondChildNode()
    {
        
    }
    public SecondChildNode(ProtoFileInfo protoInfo)
    {
        this.ProtoInfo = protoInfo;
        this.Text = this.ProtoInfo.ProtoField.Name;
        this.path = this.ProtoInfo.ProtoPath;
    }
}

