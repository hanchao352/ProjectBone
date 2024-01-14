using System.Collections.Generic;
using System.Linq;

public class Note
{
    private int _id;
    private string _title;
    private string _content;
    private List<string> _imageurl = new List<string>();

    public int Id
    {
        get => _id;
        set => _id = value;
    }

    public string Title
    {
        get => _title;
        set => _title = value;
    }

    public string Content
    {
        get => _content;
        set => _content = value;
    }

    public List<string> Imageurl
    {
        get => _imageurl;
        set => _imageurl = value;
    }

    public void UpdateNote(BoneNote boneNote)
    {
        Id = boneNote.NoteId;
        Title = boneNote.NoteTitle;
        Content = boneNote.Notecontent;
        Imageurl = boneNote.Imageurl.ToList();
    }
}