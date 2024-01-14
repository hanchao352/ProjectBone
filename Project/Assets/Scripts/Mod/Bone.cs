using System;

public class Bone
{
    private int _id;
    private string _type;
    private string _name;
    private string _content;
    private Note _note;

    public int Id
    {
        get => _id;
        set => _id = value;
    }

    public string Boneype
    {
        get => _type;
        set => _type = value;
    }

    public string Name
    {
        get => _name;
        set => _name = value;
    }

    public string Content
    {
        get => _content;
        set => _content = value;
    }

    public Note Note
    {
        get => _note;
        set => _note = value;
    }

    public void UpdateBoneInfo(BoneInfo boneInfo)
    {
        Id = boneInfo.BoneId;
        Boneype = boneInfo.Type;
        Name = boneInfo.Bonename;
        Content = boneInfo.Bonecontent;
        if (Note == null)
        {
            Note = new Note();
        }

        if (boneInfo.Note != null)
        {
            Note.UpdateNote(boneInfo.Note);
        }
       
    }
}