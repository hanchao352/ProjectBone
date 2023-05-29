using System;
using ICSharpCode.AvalonEdit.CodeCompletion;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;

public class CustomCompletionData : ICompletionData
{
    public CustomCompletionData(string text, string description)
    {
        this.Text = text;
        this.Description = description;
    }

    public ImageSource Image => null;

    public string Text { get; }

    public object Content => Text;

    public object Description { get; }

    public double Priority => 0;

    public void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
    {
        textArea.Document.Replace(completionSegment, Text);
    }
}