
using System;public enum EnumGender
{
    Male = 0,
    Female = 1,
}



[Flags]
public enum EnumBone
{
    // 其它（modeldate.type_id 为空或 0，bonedata.type 为 0）
    Other = 0,
    //骨骼
    Bone = 1<<1,
    //肌肉
    Muscle = 1<<2,
    //筋膜
    Fascia = 1<<3,
    //所有
    All = Bone|Muscle|Fascia,
}

[Flags]
public enum EnumPos
{

    //开始定义枚举
    //无
    None = 0,
    //上肢
    UpperLimbs = 1 << 1,
    //肩背
    ShoulderBack = 1<<2,
    //下肢
    LowerLimbs = 1 << 3,
    //盆骨
    Pelvis = 1 << 4,
    //头颈
    HeadAndNeck = 1 << 5,
    //胸腹
    ChestAndAbdomen = 1 << 6,
    //脊柱
    Spine = 1 << 7,
    //肩胛带
    Scapula = 1 << 8,
    //ALL
    All = None| UpperLimbs | ShoulderBack | LowerLimbs | Pelvis | HeadAndNeck | ChestAndAbdomen | Spine | Scapula,
    
    
}
[Flags]
public enum BoneShowType
{ 
    //不显示
    None = 0,
    //显示骨骼
    Bone = 1<< 0,
    //显示肌肉
    Muscle = 1<< 1,     
    //筋膜
    Fascia = 1<< 2,
   //显示所有
    All =Bone|Muscle|Fascia,
   
  
}

/// <summary>
/// 模型数据类型与界面筛选位之间的映射。
/// EnumBone 使用 bonedata.type 的值；BoneShowType 使用 App code=5 的筛选协议值。
/// </summary>
public static class BoneTypeMapper
{
    /// <summary>
    /// 将 modeldate.type_id（0=其它、1=骨骼、2=肌肉、3=筋膜）转换为模型数据类型。
    /// </summary>
    public static EnumBone FromTypeId(int typeId)
    {
        return typeId switch
        {
            1 => EnumBone.Bone,
            2 => EnumBone.Muscle,
            3 => EnumBone.Fascia,
            _ => EnumBone.Other
        };
    }

    /// <summary>
    /// 判断模型数据类型是否匹配 App/UI 下发的 BoneShowType 筛选位。
    /// Other 只在显示全部类型时可见。
    /// </summary>
    public static bool MatchesShowType(EnumBone boneType, int showType)
    {
        if ((showType & (int)BoneShowType.All) == (int)BoneShowType.All)
        {
            return true;
        }

        return boneType switch
        {
            EnumBone.Bone => (showType & (int)BoneShowType.Bone) != 0,
            EnumBone.Muscle => (showType & (int)BoneShowType.Muscle) != 0,
            EnumBone.Fascia => (showType & (int)BoneShowType.Fascia) != 0,
            _ => false
        };
    }
}

/// <summary>
/// 骨骼方向枚举
/// </summary>
[Flags]
public enum EnumDirection
{
    //无
    None = 0,
    //左侧
    Left = 1 << 0,
    //右侧
    Right = 1 << 1,
    //其他
    Other = 1 << 2,
}


