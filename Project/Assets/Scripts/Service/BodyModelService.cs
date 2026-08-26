using System;
using UnityEngine;
using Stopwatch = System.Diagnostics.Stopwatch;

/// <summary>
/// 模型加载与生命周期服务。负责人体模型预制体加载、MeshCollider 初始化、
/// 骨骼注册、Layer 设置、变换重置和可见性控制。
/// </summary>
public class BodyModelService
{
    /// <summary>
    /// 人体模型资源路径（Resources 下，不含扩展名）。
    /// 必须使用扁平静态网格预制体 jirou_nan：根节点 jirou_nan 下直接挂载约 1423 个
    /// 以骨骼ID命名的子节点，每个子节点为静态 MeshFilter+MeshRenderer+MeshCollider。
    /// 不能使用原始绑定模型 jirou_01.FBX：它是 Generic 骨骼绑定(蒙皮)模型，实例化后是
    /// 蒙皮骨架(SkinnedMeshRenderer)，没有以ID命名的扁平静态网格子节点，会导致
    /// RegisterBones 注册不到任何骨骼，表现为"只有根节点、没有子节点"。
    /// </summary>
    private const string ModelResourcePath = "Model/jirou_nan";

    private readonly SkeletonRegistry _registry;
    private GameObject _body;
    private Vector3 _initPos;
    private Vector3 _initScale;
    private Vector3 _initAngle;
    private Action<ModelLoadProgressInfo> _progressCallback;
    private int _lastReportedProgress = -1;
    private string _lastReportedStage;
    private string _lastReportedStatus;

    /// <summary>
    /// 当前加载的人体模型 GameObject
    /// </summary>
    public GameObject Body => _body;

    public BodyModelService(SkeletonRegistry registry)
    {
        _registry = registry;
    }

    /// <summary>
    /// 完整的模型加载流程：加载预制体 → 初始化碰撞体 → 注册骨骼 → 设置Layer → 记录初始变换
    /// </summary>
    public void LoadBody(Action<ModelLoadProgressInfo> progressCallback = null)
    {
        Stopwatch totalTimer = Stopwatch.StartNew();
        Stopwatch stageTimer = Stopwatch.StartNew();
        StartupTimingLogger.Mark("body_load_enter");

        _progressCallback = progressCallback;
        _lastReportedProgress = -1;
        _lastReportedStage = null;
        _lastReportedStatus = null;

        try
        {
            ReportProgress(0, "starting", "loading");
            ReportProgress(5, "loading_model", "loading");
            GameObject obj = LoadModelPrefab();
            if (!obj)
            {
                const string error = "模型预制体资源加载失败";
                Debug.LogError($"[BodyModel] 模型预制体加载失败! Resources.Load(\"{ModelResourcePath}\") 返回 null");
                ReportProgress(_lastReportedProgress, "loading_model", "failed", error);
                return;
            }

            StartupTimingLogger.MarkDuration(
                "body_prefab_ready", stageTimer, $"children={obj.transform.childCount}");

            stageTimer.Restart();
            ReportProgress(35, "validating_model", "loading");

            StartupTimingLogger.MarkDuration(
                "body_hierarchy_diagnostics_complete", stageTimer,
                $"children={obj.transform.childCount}");

            // 检查 MeshFilter 的网格是否丢失（定位"打包后层级在、但 MeshFilter.mesh 丢失"的问题）
            stageTimer.Restart();
            CheckMeshIntegrity(obj);
            StartupTimingLogger.MarkDuration("body_mesh_integrity_check_complete", stageTimer);

            obj.transform.position = new Vector3(0, 0, 0);

            ReportProgress(45, "setting_layer", "loading");
            // 先设置 Layer，确保所有节点（包括未激活的）都有正确的 Layer
            stageTimer.Restart();
            SetBodyLayer(obj, UnityLayer.Layer_Body);
            StartupTimingLogger.MarkDuration("body_layer_setup_complete", stageTimer);

            ReportProgress(55, "initializing_colliders", "loading");
            stageTimer.Restart();
            InitializeBoneColliders(obj, progress =>
                ReportProgress(55 + Mathf.RoundToInt(progress * 20f), "initializing_colliders", "loading"));
            StartupTimingLogger.MarkDuration(
                "body_collider_scan_complete", stageTimer, $"children={obj.transform.childCount}");

            ReportProgress(75, "registering_bones", "loading");
            stageTimer.Restart();
            RegisterBones(obj, progress =>
                ReportProgress(75 + Mathf.RoundToInt(progress * 20f), "registering_bones", "loading"));
            StartupTimingLogger.MarkDuration(
                "body_bone_registration_complete", stageTimer, $"bones={_registry.Count}");

            ReportProgress(95, "finalizing", "loading");
            obj.transform.position = new Vector3(0, 0, 0.5f);
            _body = obj;
            _body.transform.localScale = new Vector3(10, 10, 10);

            _initPos = _body.transform.position;
            _initScale = _body.transform.localScale;
            _initAngle = _body.transform.eulerAngles;

            ReportProgress(100, "completed", "completed");
            StartupTimingLogger.MarkDuration(
                "body_load_complete", totalTimer,
                $"children={_body.transform.childCount}|bones={_registry.Count}");
        }
        catch (Exception exception)
        {
            StartupTimingLogger.MarkDuration(
                "body_load_failed", totalTimer,
                $"exception={exception.GetType().Name}|message={exception.Message}");
            ReportProgress(Mathf.Max(_lastReportedProgress, 0), "failed", "failed", exception.Message);
            throw;
        }
        finally
        {
            _progressCallback = null;
        }
    }

    private void ReportProgress(int progress, string stage, string status, string error = null)
    {
        progress = Mathf.Clamp(progress, 0, 100);
        if (progress == _lastReportedProgress &&
            stage == _lastReportedStage &&
            status == _lastReportedStatus)
        {
            return;
        }

        _lastReportedProgress = progress;
        _lastReportedStage = stage;
        _lastReportedStatus = status;
        _progressCallback?.Invoke(new ModelLoadProgressInfo
        {
            progress = progress,
            stage = stage,
            status = status,
            error = error
        });
    }

    /// <summary>
    /// 校验子节点 MeshFilter 的网格是否丢失(sharedMesh 为 null)，丢失时报错。
    /// 用于定位"打包后层级在、但 MeshFilter.mesh 丢失"的问题：网格数据并不在预制体里，
    /// 而是来自被跨资源引用的 FBX 子资源(jirou_01.FBX)。若该 FBX 的网格未被打进包，
    /// 则组件仍在、但 sharedMesh 为 null，表现为节点都在却看不到模型。
    /// </summary>
    private void CheckMeshIntegrity(GameObject root)
    {
        MeshFilter[] filters = root.GetComponentsInChildren<MeshFilter>(true);
        int total = filters.Length;
        int missing = 0;
        string sample = "";
        int sampleCount = 0;

        for (int i = 0; i < total; i++)
        {
            if (filters[i].sharedMesh)
            {
                continue;
            }

            missing++;
            if (sampleCount < 5)
            {
                sample = sampleCount == 0 ? filters[i].gameObject.name : sample + ", " + filters[i].gameObject.name;
                sampleCount++;
            }
        }

        if (missing > 0)
        {
            Debug.LogError($"[BodyModel] 网格丢失诊断: MeshFilter共{total}个, sharedMesh为null的有{missing}个! 丢失样本=[{sample}]。" +
                           "网格数据来自被引用的FBX子资源(jirou_01.FBX)，请确认其网格已正确打进包(LFS真实文件已拉取/未被裁剪/跨资源引用未失效)。");
        }
    }

    /// <summary>
    /// 从 Resources 加载人体模型预制体并实例化。
    /// 加载扁平静态网格预制体 jirou_nan(根节点下挂载以骨骼ID命名的静态网格子节点)，
    /// 而非原始绑定模型 jirou_01.FBX(蒙皮骨架)，否则实例化结果只有根节点、没有骨骼子节点。
    /// </summary>
    public GameObject LoadModelPrefab()
    {
        Stopwatch stageTimer = Stopwatch.StartNew();
        GameObject source = Resources.Load<GameObject>(ModelResourcePath);
        StartupTimingLogger.MarkDuration(
            "body_source_resources_load_complete", stageTimer,
            $"resource={ModelResourcePath}|success={source != null}");
        if (!source)
        {
            Debug.LogError($"[BodyModel] 源资源加载失败: Resources.Load(\"{ModelResourcePath}\") 返回 null");
            return null;
        }

        stageTimer.Restart();
        GameObject instance = ResManager.Instance.LoadRes<GameObject>(ModelResourcePath);
        StartupTimingLogger.MarkDuration(
            "body_res_manager_load_returned", stageTimer,
            $"resource={ModelResourcePath}|success={instance != null}");
        return instance;
    }

    /// <summary>
    /// 为每个子对象添加 MeshCollider（如果不存在）
    /// </summary>
    public void InitializeBoneColliders(GameObject root, Action<float> progressCallback = null)
    {
        int childCount = root.transform.childCount;
        if (childCount == 0)
        {
            progressCallback?.Invoke(1f);
            return;
        }

        for (int i = 0; i < childCount; i++)
        {
            GameObject child = root.transform.GetChild(i).gameObject;
            if (child.GetComponent<MeshCollider>() == null)
            {
                child.AddComponent<MeshCollider>();
            }

            progressCallback?.Invoke((i + 1f) / childCount);
        }
    }

    /// <summary>
    /// 解析子对象名称，创建 SkeletonInfo 并注册到 SkeletonRegistry 和 BoneMod.Instance.boneDic。
    /// 先统计有效骨骼数量，再初始化 SkeletonRegistry 容量，最后逐个注册。
    /// </summary>
    public void RegisterBones(GameObject root, Action<float> progressCallback = null)
    {
        int childCount = root.transform.childCount;

        if (childCount == 0)
        {
            _registry.Initialize(0);
            Debug.LogWarning("[BodyModel] RegisterBones: 直接子节点为 0! 模型层级异常，请检查 prefab 打包结果。");
            progressCallback?.Invoke(1f);
            return;
        }

        // 第一遍：统计有效骨骼数量（名称可解析为 int 的子对象）
        int validCount = 0;
        for (int i = 0; i < childCount; i++)
        {
            if (int.TryParse(root.transform.GetChild(i).gameObject.name, out _))
            {
                validCount++;
            }

            progressCallback?.Invoke((i + 1f) / childCount * 0.5f);
        }

        _registry.Initialize(validCount);

        // 第二遍：注册骨骼
        for (int i = 0; i < root.transform.childCount; i++)
        {
            GameObject boneObj = root.transform.GetChild(i).gameObject;
            string name = boneObj.name;

            if (int.TryParse(name, out int id))
            {
                Bone bone = new Bone();
                bone.Id = id;

                MeshRenderer meshRenderer = boneObj.GetComponent<MeshRenderer>();
                _registry.Register(id, bone, boneObj, meshRenderer);

                if (BoneMod.Instance.boneDic.ContainsKey(id))
                {
                    BoneMod.Instance.boneDic[id] = bone;
                }
                else
                {
                    BoneMod.Instance.boneDic.Add(id, bone);
                }
            }

            progressCallback?.Invoke(0.5f + (i + 1f) / childCount * 0.5f);
        }
    }

    /// <summary>
    /// 设置根对象和所有子对象的 Layer（递归设置所有后代）
    /// </summary>
    public void SetBodyLayer(GameObject root, int layer)
    {
        // 设置根节点
        root.layer = layer;

        // 递归设置所有子节点
        SetLayerRecursively(root.transform, layer);
    }

    /// <summary>
    /// 递归设置 Transform 及其所有子节点的 Layer（包括未激活的节点）
    /// </summary>
    private void SetLayerRecursively(Transform trans, int layer)
    {
        int oldLayer = trans.gameObject.layer;
        trans.gameObject.layer = layer;

        // 诊断：验证 Layer 是否真的被设置
        if (trans.gameObject.layer != layer)
        {
            Debug.LogWarning($"[BodyModel] Layer设置失败! 节点={trans.name}, 期望Layer={layer}, 实际Layer={trans.gameObject.layer}, 原Layer={oldLayer}");
        }

        // 使用 GetChild 遍历，确保包括未激活的子节点
        for (int i = 0; i < trans.childCount; i++)
        {
            Transform child = trans.GetChild(i);
            SetLayerRecursively(child, layer);
        }
    }

    /// <summary>
    /// 重置模型位置到初始值
    /// </summary>
    public void ResetPosition()
    {
        if (_body == null) return;
        _body.transform.position = _initPos;
    }

    /// <summary>
    /// 重置模型缩放到初始值
    /// </summary>
    public void ResetScale()
    {
        if (_body == null) return;
        _body.transform.localScale = _initScale;
    }

    /// <summary>
    /// 重置模型旋转到初始值
    /// </summary>
    public void ResetRotation()
    {
        if (_body == null) return;
        _body.transform.eulerAngles = _initAngle;
    }

    /// <summary>
    /// 完全重置变换（位置 + 缩放 + 旋转）
    /// </summary>
    public void ResetTransform()
    {
        ResetPosition();
        ResetScale();
        ResetRotation();
    }

    /// <summary>
    /// 设置 Body 的可见性。
    /// visible=true 时激活并确保 Layer 为 Layer_Body；
    /// visible=false 时停用但保持 Layer 不变（避免影响后续显示）。
    /// </summary>
    public void SetBodyVisible(bool visible)
    {
        if (_body == null) return;

        if (visible)
        {
            // 显示时确保 Layer 正确
            SetBodyLayer(_body, UnityLayer.Layer_Body);
            _body.SetActive(true);
        }
        else
        {
            // 隐藏时只停用，不修改 Layer（保持为 Body Layer）
            _body.SetActive(false);
        }
    }
}
