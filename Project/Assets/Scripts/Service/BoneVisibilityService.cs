using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 骨骼可见性控制服务。负责骨骼显示/隐藏、按 BoneShowType 类型过滤、按 EnumPos 部位过滤。
/// 所有过滤操作单次遍历完成，使用位运算判定，零GC。
/// </summary>
public class BoneVisibilityService
{
    private readonly SkeletonRegistry _registry;

    public BoneVisibilityService(SkeletonRegistry registry)
    {
        _registry = registry;
    }

    /// <summary>
    /// 按骨骼类型过滤显示。单次遍历，使用位运算判定。
    /// 当 type 包含 BoneShowType.All 时，显示全部。
    /// </summary>
    public void ShowBoneByType(int type)
    {
        var items = _registry.GetItems(out int count);
        bool showAll = (type & (int)BoneShowType.All) == (int)BoneShowType.All;

        for (int i = 0; i < count; i++)
        {
            ref var info = ref items[i]; // ref 避免 struct 拷贝
            if (showAll)
            {
                info.boneGameObject.SetActive(true);
            }
            else
            {
                // 位运算：将 EnumBone 映射到 BoneShowType 的对应位
                int boneBit = (int)info.bone.Boneenum;
                bool visible = (type & boneBit) != 0;
                info.boneGameObject.SetActive(visible);
            }
        }
    }

    /// <summary>
    /// 同时按骨骼类型和身体部位过滤显示。
    /// 骨骼必须同时满足类型和部位条件才会显示。
    /// 当 pos 为 EnumPos.All 时不做部位过滤。
    /// </summary>
    public void ShowBoneByTypeAndPos(int type, int pos)
    {
        var items = _registry.GetItems(out int count);
        bool showAllType = (type & (int)BoneShowType.All) == (int)BoneShowType.All;
        bool showAllPos = (pos & (int)EnumPos.All) == (int)EnumPos.All;

        int visibleCount = 0;
        for (int i = 0; i < count; i++)
        {
            ref var info = ref items[i];

            // 类型过滤
            bool typeVisible = showAllType || ((type & (int)info.bone.Boneenum) != 0);

            // 部位过滤
            bool posVisible = showAllPos || ((info.bone.Pos & pos) != 0);

            // 两个条件都满足才显示
            bool visible = typeVisible && posVisible;
            info.boneGameObject.SetActive(visible);
            if (visible)
            {
                visibleCount++;
            }
        }

        // 诊断：打印筛选条件与实际可见数量，定位"显示全部却不显示"的问题
        Debug.Log($"[Visibility] type={type}(全类型={showAllType}), pos={pos}(全部位={showAllPos}), 总数={count}, 可见={visibleCount}, 隐藏={count - visibleCount}");
    }

    /// <summary>
    /// 按身体部位过滤选择骨骼。单次遍历，使用位运算判定。
    /// </summary>
    public void SelectBoneByPos(int pos)
    {
        var items = _registry.GetItems(out int count);
        for (int i = 0; i < count; i++)
        {
            bool visible = (items[i].bone.Pos & pos) != 0;
            items[i].boneGameObject.SetActive(visible);
        }
    }

    /// <summary>
    /// 隐藏指定的骨骼列表。先将所有骨骼设为可见，再隐藏指定骨骼。
    /// </summary>
    public void HideBones(List<int> boneIds)
    {
        var items = _registry.GetItems(out int count);

        // 先将所有骨骼设为可见
        for (int i = 0; i < count; i++)
        {
            items[i].boneGameObject.SetActive(true);
        }

        // 再隐藏指定骨骼
        if (boneIds == null || boneIds.Count == 0) return;

        for (int i = 0; i < count; i++)
        {
            int boneId = items[i].boneId;
            for (int j = 0; j < boneIds.Count; j++)
            {
                if (boneId == boneIds[j])
                {
                    items[i].boneGameObject.SetActive(false);
                    break;
                }
            }
        }
    }

    /// <summary>
    /// 显示指定的骨骼列表
    /// </summary>
    public void ShowBones(List<int> boneIds)
    {
        if (boneIds == null || boneIds.Count == 0) return;

        var items = _registry.GetItems(out int count);

        for (int i = 0; i < count; i++)
        {
            int boneId = items[i].boneId;
            for (int j = 0; j < boneIds.Count; j++)
            {
                if (boneId == boneIds[j])
                {
                    items[i].boneGameObject.SetActive(true);
                    break;
                }
            }
        }
    }

    /// <summary>
    /// 仅显示指定骨骼，隐藏其他所有
    /// </summary>
    public void ShowOnlyBone(int boneId)
    {
        var items = _registry.GetItems(out int count);

        for (int i = 0; i < count; i++)
        {
            items[i].boneGameObject.SetActive(items[i].boneId == boneId);
        }
    }

    /// <summary>
    /// 显示所有骨骼
    /// </summary>
    public void ShowAllBones()
    {
        var items = _registry.GetItems(out int count);

        for (int i = 0; i < count; i++)
        {
            items[i].boneGameObject.SetActive(true);
        }
    }
}
