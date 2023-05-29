using System.IO;
using System.Text.RegularExpressions;



public class ProtoParser
{
    public static ProtoInfo ParseProtoFile(string filePath)
    {
        string content = File.ReadAllText(filePath);
        ProtoInfo protoInfo = ParseProtoContent(content);
        protoInfo.FilePath = filePath;
        return protoInfo;
    }

    public static ProtoInfo ParseProtoContent(string content)
    {
        var protoInfo = new ProtoInfo();
        var messagePattern = @"message\s+(\w+)\s+\{([\s\S]*?)\}";
        var fieldPattern = @"\s*(\w+)\s+(\w+)\s+=\s+(\d+)\s*(\[.*?\])?;";
        var optionPattern = @"option\s+(?:\((.*?)\)|(\w+))\s*=\s*(.*?);";

        var messageMatches = Regex.Matches(content, messagePattern);

        foreach (Match messageMatch in messageMatches)
        {
            var messageInfo = new ProtoInfo.MessageInfo
            {
                Name = messageMatch.Groups[1].Value
            };

            string messageBody = messageMatch.Groups[2].Value;

            // 解析消息中的选项（自定义选项和标准选项）
            var optionMatches = Regex.Matches(messageBody, optionPattern);
            foreach (Match optionMatch in optionMatches)
            {
                string customOptionKey = optionMatch.Groups[1].Value.Trim();
                string standardOptionKey = optionMatch.Groups[2].Value.Trim();
                string optionKey = string.IsNullOrEmpty(customOptionKey) ? standardOptionKey : customOptionKey;
                string optionValue = optionMatch.Groups[3].Value.Trim();

                if (string.IsNullOrEmpty(customOptionKey))
                {
                    messageInfo.StandardOptions[optionKey] = optionValue;
                }
                else
                {
                    messageInfo.CustomOptions[optionKey] = optionValue;
                    messageInfo.MsgID = int.Parse(optionValue);
                }
            }

            // 解析字段
            var fieldMatches = Regex.Matches(messageBody, fieldPattern);
            foreach (Match fieldMatch in fieldMatches)
            {
                var fieldInfo = new ProtoInfo.FieldInfo
                {
                    Type = fieldMatch.Groups[1].Value,
                    Name = fieldMatch.Groups[2].Value,
                    Tag = int.Parse(fieldMatch.Groups[3].Value)
                };

                messageInfo.Fields.Add(fieldInfo);
            }

            protoInfo.Messages.Add(messageInfo);
        }

        return protoInfo;
    }
}