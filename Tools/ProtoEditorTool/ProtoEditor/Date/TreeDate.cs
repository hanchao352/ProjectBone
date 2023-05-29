using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class TreeDate
{
    public List<RootNodeInfo> RootNodeInfos=new List<RootNodeInfo>();
    public TreeDate()
    {
        List<ProtoFileInfo> ProtoInfos = ProtoManager.Instance.UpdateProtoInfos();
    }

    public TreeDate AddRootInfo(RootNodeInfo root)
    {
        RootNodeInfos.Add(root);
        RootNodeInfos.Sort(SortList);
        return this;
    }

    public TreeDate RemoveRootInfo(RootNodeInfo root)
    {
        for (int i = 0; i < RootNodeInfos.Count; i++)
        {
            RootNodeInfo info = RootNodeInfos[i];
            if (root.MinMsgID == info.MinMsgID && root.MaxMsgID == info.MaxMsgID)
            {
                RootNodeInfos.RemoveAt(i);
                break;
            }
        }
        RootNodeInfos.Sort(SortList);
        return this;
    }

    public TreeDate UpdateRootNodeInfo(RootNodeInfo root)
    {
        for (int i = 0; i < RootNodeInfos.Count; i++)
        {
            RootNodeInfo info = RootNodeInfos[i];
            if (root.RootID ==info.RootID)
            {
                RootNodeInfos[i] = root;
                break;
            }
        }
        RootNodeInfos.Sort(SortList);
        return this;
    }

    private int SortList(RootNodeInfo x, RootNodeInfo y)
    {
        
            return x.MinMsgID.CompareTo(y.MinMsgID);
      
    }
}



