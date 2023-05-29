
using System.Text;

public  class Prototemplate:Singleton<Prototemplate>
{
    // public static string MessageName;
    // public static string ProroVersion = "proto3";
    // public static 
    // public static readonly string Template = $"syntax = \" {ProroVersion} \" " +
    //                                          $"//required: 必须提供该字段的值，否则该消息将被视为“未初始化” " +
    //                                          $"//optional：可以设置也可以不设置该字段。如果未设置可选字段值，则使用默认值。" +
    //                                          $"//repeated：该字段可以重复任意次数（包括零次）。重复值的顺序将保留在 protocol buffer 中。可以将 repeated 字段视为动态大小的数组。" +
    //                                          $"import \"standmsgid.proto\";" +
    //                                          $"message {MessageName}" +
    //                                          $"{{" +
    //                                          $"    option (msgid) = 1000;" +
    //                                          $"}}";

    public string MessageName;
    private string ProroVersion = "proto3";
    private StringBuilder sb = new StringBuilder();

    public string GetTemplate()
    {
        
        sb.AppendLine($"syntax = \"{ProroVersion}\" ;");
        sb.AppendLine($"//required: 必须提供该字段的值，否则该消息将被视为“未初始化” ");
        sb.AppendLine($"//optional：可以设置也可以不设置该字段。如果未设置可选字段值，则使用默认值。");
        sb.AppendLine($"//repeated：该字段可以重复任意次数（包括零次）。重复值的顺序将保留在 protocol buffer 中。可以将 repeated 字段视为动态大小的数组。");
        sb.AppendLine($"import \"OptionMsgId.proto\";");
        //sb.AppendLine($"message {MessageName}");
        //sb.AppendLine($"{{");
        //sb.AppendLine($"    option (msgid) = 1000;");
        //sb.AppendLine($"}}");
        
        string str = sb.ToString();
        sb.Clear();
        return str;
    }


}