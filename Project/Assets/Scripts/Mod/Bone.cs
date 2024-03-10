using System;
using UnityEngine;

public class Bone
{
    private int _id;
    private string _type;
    private string _name;
    private string _content;
    private Note _note;
    private EnumBone _boneenum;
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

    public EnumBone Boneenum
    {
        get => _boneenum;
        set => _boneenum = value;
    }

    public void UpdateBoneInfo(BoneInfo boneInfo)
    {
        Id = boneInfo.BoneId;
        Boneype = boneInfo.Type;
        Debug.Log("类型："+boneInfo.Type);
        if (Boneype == "骨骼")
        {
            Boneenum = EnumBone.Bone;
        }
        else if (Boneype == "肌肉")
        {
            Boneenum = EnumBone.Muscle;
        }
       
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