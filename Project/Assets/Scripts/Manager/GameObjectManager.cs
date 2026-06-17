using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameObjectManager : SingletonManager<GameObjectManager>, IGeneric
{
    private SkeletonRegistry _registry;
    private BodyModelService _bodyModelService;
    private BoneVisibilityService _visibilityService;
    private BoneSelectionService _selectionService;
    private BoneMaterialService _materialService;
    private BoneConfigService _configService;

    private bool _bodyVisible;
    private int _boneShowType = (int)BoneShowType.All;
    private int _selectBoneType = (int)EnumPos.All;

    // 模型加载前若已收到骨骼数据，先缓存，待模型加载完成后再应用并显示（解决嵌入模式下数据/加载时序竞争）
    private List<BoneData> _pendingBoneData;

    public GameObject Body => _bodyModelService.Body;

    public bool BodyVisible
    {
        get { return _bodyVisible; }
        set
        {
            if (_bodyModelService.Body == null)
            {
                Debug.LogWarning($"[GameObjectManager] BodyVisible 设置失败: Body 为 null, 请求值: {value}");
                _bodyVisible = false;
                return;
            }
            _bodyVisible = value;
            _bodyModelService.SetBodyVisible(value);
            // 诊断：隐藏时打印调用栈，定位"谁把模型根节点隐藏了"
            if (!value)
            {
                Debug.Log($"[GameObjectManager] BodyVisible = false, Body active: {_bodyModelService.Body.activeSelf}\n[隐藏调用栈]\n{StackTraceUtility.ExtractStackTrace()}");
            }
            else
            {
                Debug.Log($"[GameObjectManager] BodyVisible = true, Body active: {_bodyModelService.Body.activeSelf}");
            }
        }
    }

    public int ShowType
    {
        get { return _boneShowType; }
        set
        {
            _boneShowType = value;
            // 同时应用类型过滤和部位过滤
            _visibilityService.ShowBoneByTypeAndPos(_boneShowType, _selectBoneType);
        }
    }

    public int SelectBoneType
    {
        get { return _selectBoneType; }
        set
        {
            _selectBoneType = value;
            // 同时应用类型过滤和部位过滤
            _visibilityService.ShowBoneByTypeAndPos(_boneShowType, _selectBoneType);
        }
    }

    public override void Initialize()
    {
        base.Initialize();
        _registry = new SkeletonRegistry();
        _materialService = new BoneMaterialService(_registry);
        _bodyModelService = new BodyModelService(_registry);
        _visibilityService = new BoneVisibilityService(_registry);
        _selectionService = new BoneSelectionService(_registry, _materialService);
        _configService = new BoneConfigService(_registry);
    }

    public override void AllManagerInitialize()
    {
        base.AllManagerInitialize();
        _bodyModelService.LoadBody();
        _configService.InitializeBuffer(_registry.Count);

        // 模型加载完成：若加载前已收到数据则立即应用并显示，否则先隐藏等待数据
        if (_pendingBoneData != null)
        {
            ApplyBoneConfig(_pendingBoneData);
            _pendingBoneData = null;
        }
        else
        {
            BodyVisible = false;
        }
    }

    public void SelectBone(int boneid)
    {
        _selectionService.SelectBone(boneid);
    }

    public void ResetBoneColor()
    {
        _selectionService.ResetBoneColor();
    }

    public void TransparentOtherBone()
    {
        _materialService.TransparentOtherBone(BoneMod.Instance.selectedBoneIds);
    }

    public void ResetBoneTransparency()
    {
        _materialService.ResetBoneTransparency();
    }

    public void HideBone()
    {
        _visibilityService.HideBones(BoneMod.Instance.selectedBoneIds);
    }

    public void ShowBone()
    {
        _visibilityService.ShowBones(BoneMod.Instance.selectedBoneIds);
    }

    public void HideOtherBone()
    {
        _visibilityService.ShowOnlyBone(BoneMod.Instance.CurrentBoneId);
    }

    public void ShowOtherBone()
    {
        _visibilityService.ShowAllBones();
    }

    public void TransparentBone()
    {
        _materialService.TransparentBones(BoneMod.Instance.selectedBoneIds, 0.3f);
    }

    public void SolidBone()
    {
        _materialService.SolidBones(BoneMod.Instance.selectedBoneIds);
    }

    public void SolidOtherBone()
    {
        _materialService.SolidOtherBones(BoneMod.Instance.selectedBoneIds);
    }

    public void ShowBoneByType(int type)
    {
        _boneShowType = type;
        _visibilityService.ShowBoneByTypeAndPos(_boneShowType, _selectBoneType);
    }

    public void SelectBoneByPos(int pos)
    {
        _selectBoneType = pos;
        _visibilityService.ShowBoneByTypeAndPos(_boneShowType, _selectBoneType);
    }

    public SkeletonInfo? GetSkeletonInfo(int boneid)
    {
        if (_registry.TryGet(boneid, out SkeletonInfo info))
            return info;
        return null;
    }

    public void ResetTransform()
    {
        _bodyModelService.ResetTransform();
    }

    public void ResetAll()
    {
        _bodyModelService.ResetTransform();
        _materialService.ResetBoneTransparency();
        _boneShowType = (int)BoneShowType.All;
        _selectBoneType = (int)EnumPos.All;
        _visibilityService.ShowBoneByTypeAndPos(_boneShowType, _selectBoneType);
        BoneMod.Instance.ClearSelection();
    }

    public void ReSetPos()
    {
        _bodyModelService.ResetPosition();
    }

    public void ReSetScale()
    {
        _bodyModelService.ResetScale();
    }

    public void ResetAngle()
    {
        _bodyModelService.ResetRotation();
    }

    /// <summary>
    /// 完全重置模型（位置、缩放、角度）— 向后兼容
    /// </summary>
    public void ReSet()
    {
        _bodyModelService.ResetTransform();
    }

    public List<BoneData> ExportBoneConfig()
    {
        return _configService.ExportBoneConfigAsList();
    }

    public void ApplyBoneConfig(List<BoneData> boneDataList)
    {
        // 模型尚未加载完成时，先缓存数据，待 AllManagerInitialize 中模型就绪后再应用，避免显示请求丢失
        if (!_bodyModelService.Body)
        {
            _pendingBoneData = boneDataList;
            Debug.LogWarning("[GameObjectManager] 模型未就绪，已缓存骨骼数据，待加载完成后应用并显示");
            return;
        }

        _configService.ApplyBoneConfigFromList(boneDataList);
        ShowBoneByType(_boneShowType);
        SelectBoneByPos(_selectBoneType);
        // 数据应用后激活模型根节点（显示统一收敛到此处，不依赖外部调用方）
        BodyVisible = true;
    }

    public void LoadBoneConfigFromJson(string jsonString)
    {
        _configService.LoadBoneConfigFromJson(jsonString);
        ShowBoneByType(_boneShowType);
        SelectBoneByPos(_selectBoneType);
        // 数据应用后激活模型根节点
        BodyVisible = true;
    }

    public string ExportBoneConfigToJson()
    {
        return _configService.ExportBoneConfigToJson();
    }
}
