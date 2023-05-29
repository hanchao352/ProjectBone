

using System.Collections.Generic;

public class ProtoInfo
{
    public List<MessageInfo> Messages { get; set; } = new List<MessageInfo>();
    public string FilePath { get; set; }
    
    public class MessageInfo
    {
        public string Name { get; set; }
        public List<FieldInfo> Fields { get; set; } = new List<FieldInfo>();
        public Dictionary<string, string> CustomOptions { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> StandardOptions { get; set; } = new Dictionary<string, string>();
        public int MsgID { get; set; }
       
    }

    public class FieldInfo
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public int Tag { get; set; }
    }
}