using Google.Protobuf.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class ProtoFileInfo
{
    public string ProtoPath=string.Empty;
    public FileDescriptorProto ProtoField;
    public List<int> MsgIDList = new List<int>();
    public int MinMsgId
    {
        get { return GetMinMsgId(); }
    }

    public int MaxMsgId
    {
        get { return GetMaxMsgId(); }
    }
    public ProtoFileInfo()
    {
        ProtoField = new FileDescriptorProto();
    }
    public void Init()
    {
        if (ProtoField != null && ProtoField.MessageTypes!=null)
        { 
            MsgIDList.Clear();
            for (int i = 0; i < ProtoField.MessageTypes.Count; i++)
            {
               
                if (ProtoField.MessageTypes[i].Options!=null && ProtoField.MessageTypes[i].Options.UninterpretedOptions!=null)
                {
                    for (int j = 0; j < ProtoField.MessageTypes[i].Options.UninterpretedOptions.Count; j++)
                    {
                        int msgid = 0;
                        bool canparse = int.TryParse(ProtoField.MessageTypes[i].Options.UninterpretedOptions[j].AggregateValue, out msgid);
                        if (canparse)
                        {
                            if (!MsgIDList.Contains(msgid))
                            {
                                MsgIDList.Add(msgid);
                            }
                            else
                            {
                                MessageBox.Show(StringDefine.ProtoWindow_MsgIDSameError, StringDefine.ProtoWindow_OKText);
                                return;
                            }
                            
                        }
                       
                    } 
                }
            }
         
        }

        MsgIDList.Sort(SortByValue);
    }

     int GetMinMsgId()
    {
        if (MsgIDList.Count<=0)
        {
            return -1;
        }
        return MsgIDList[0];
    }

     int GetMaxMsgId()
    {
        if (MsgIDList.Count <= 0)
        {
            return -1;
        }
        return MsgIDList[MsgIDList.Count-1];
    }

    private int SortByValue(int x, int y)
    {
        
        return x.CompareTo(y);
    }
}

