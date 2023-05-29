using System;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;

public static class SugDef
{
    public static List<string> sugList = new List<string>()
    {
        "m","o","p","i","r","d","f","s","e"
    };

    public static bool IsMatch(string str)
    {
        for (int i = 0; i < sugList.Count; i++)
        {
            if (sugList[i]==str)
            {
                return  true;
            }
        }

        return false;
    }
   
}

public class CodeCompletionProvider
{
    private readonly List<string> keywordList = new List<string>
    {
        "message",
        "option",
        "package",
        "import",
        "void",
        "required",
        "optional",
        "repeated",
        "float",
        "int32",
        "int64",
        "bool",
        "string",
        "bytes",
        "enum",
        // Add more keywords here
    };
    public IList<ICompletionData> GenerateCompletionData(TextEditor editor)
    {
        // Create a list to hold the completion data.
        // List<ICompletionData> completionData = new List<ICompletionData>();
        //
        // // Add your own completion data here.
        // // For example, you can create a new instance of CustomCompletionData or your own ICompletionData implementation.
        // completionData.Add(new CustomCompletionData("message", String.Empty));
        // completionData.Add(new CustomCompletionData("option", String.Empty));
        // completionData.Add(new CustomCompletionData("package", String.Empty));
        // completionData.Add(new CustomCompletionData("import", String.Empty));
        // completionData.Add(new CustomCompletionData("required", String.Empty));
        // completionData.Add(new CustomCompletionData("optional", String.Empty));
        // completionData.Add(new CustomCompletionData("repeated", String.Empty));
        // completionData.Add(new CustomCompletionData("double", String.Empty));
        // completionData.Add(new CustomCompletionData("float", String.Empty));
        // completionData.Add(new CustomCompletionData("int32", String.Empty));
        // // completionData.Add(new CustomCompletionData("int64", String.Empty));
        // // completionData.Add(new CustomCompletionData("uint32", String.Empty));
        // // completionData.Add(new CustomCompletionData("uint64", String.Empty));
        // // completionData.Add(new CustomCompletionData("sint32", String.Empty));
        // // completionData.Add(new CustomCompletionData("sint64", String.Empty));
        // // completionData.Add(new CustomCompletionData("fixed32", String.Empty));
        // // completionData.Add(new CustomCompletionData("fixed64", String.Empty));
        // // completionData.Add(new CustomCompletionData("sfixed32", String.Empty));
        // // completionData.Add(new CustomCompletionData("sfixed64", String.Empty));
        // completionData.Add(new CustomCompletionData("bool", String.Empty));
        // completionData.Add(new CustomCompletionData("string", String.Empty));
        // completionData.Add(new CustomCompletionData("bytes", String.Empty));
        // completionData.Add(new CustomCompletionData("enum", String.Empty));
        //
        // // Return the generated completion data.
        // return completionData;
        
        List<ICompletionData> completionData = new List<ICompletionData>();

        // Get the current text before the caret position
        int currentLine = editor.Document.GetLineByOffset(editor.CaretOffset).LineNumber;
        string currentText = editor.Document.GetText(0, editor.CaretOffset);
        string[] lines = currentText.Split('\n');
        string lineText = lines.Length > 0 ? lines[currentLine - 1] : string.Empty;

        // Find the last word in the line
        Match match = Regex.Match(lineText, @"\w+$");
        string lastWord = match.Value;

        // Filter the keyword list based on the last word
        IEnumerable<string> filteredKeywords = keywordList.Where(keyword => keyword.StartsWith(lastWord, StringComparison.OrdinalIgnoreCase));

        // Add your own completion data here.
        // For example, you can create a new instance of DefaultCompletionData or your own ICompletionData implementation.
        foreach (var keyword in filteredKeywords)
        {
            completionData.Add(new CustomTextCompletionData(keyword));
        }

        // Return the generated completion data.
        return completionData;
    }
}



public class TextCompletionData : ICompletionData
{
    public TextCompletionData(string text)
    {
        Text = text;
    }

    public System.Windows.Media.ImageSource Image { get; set; }

    public string Text { get; private set; }

    // Use this property if you want to show a fancy UIElement in the list.
    public object Content
    {
        get { return Text; }
    }

    public object Description { get; set; }

    public double Priority { get; set; }

    public void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
    {
        textArea.Document.Replace(completionSegment, Text);
    }
}



public class CustomTextCompletionData : ICompletionData
{
    public CustomTextCompletionData(string text)
    {
        Text = text;
    }

    public System.Windows.Media.ImageSource Image { get; set; }

    public string Text { get; private set; }

    public object Content
    {
        get { return Text; }
    }

    public object Description { get; set; }

    public double Priority { get; set; }

    public void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
    {
        int wordStartOffset = textArea.Caret.Offset;

        while (wordStartOffset > 0 && char.IsLetterOrDigit(textArea.Document.GetCharAt(wordStartOffset - 1)))
        {
            wordStartOffset--;
        }

        int wordEndOffset = textArea.Caret.Offset;

        while (wordEndOffset < textArea.Document.TextLength && char.IsLetterOrDigit(textArea.Document.GetCharAt(wordEndOffset)))
        {
            wordEndOffset++;
        }

        textArea.Document.Replace(wordStartOffset, wordEndOffset - wordStartOffset, Text);
    }
}
