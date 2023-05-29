

using System.Windows.Controls;

public class SecondLevelNode:TreeViewItem
{
    public NodeType NodeType = NodeType.Second;
    public string NodeName;
    public string FileName;
    public string FilePath;
    public int MsgID;
    public ProtoInfo ProtoInfo;
}