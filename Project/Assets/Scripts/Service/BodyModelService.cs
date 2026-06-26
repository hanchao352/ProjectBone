using UnityEngine;

/// <summary>
/// 模型加载与生命周期服务。负责人体模型预制体加载、MeshCollider 初始化、
/// 骨骼注册、Layer 设置、变换重置和可见性控制。
/// </summary>
public class BodyModelService
{
    private readonly SkeletonRegistry _registry;
    private GameObject _body;
    private Vector3 _initPos;
    private Vector3 _initScale;
    private Vector3 _initAngle;

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
    public void LoadBody()
    {
        Debug.Log("[BodyModel] 开始加载模型...");
        GameObject obj = LoadModelPrefab();
        if (!obj)
        {
            Debug.LogError("[BodyModel] 模型预制体加载失败! Resources.Load(\"Model/jirou_nan\") 返回 null");
            return;
        }

        // 诊断：直接子节点 vs 全部后代 vs 各类渲染器，用于定位真机层级差异（编辑器有子节点、真机 childCount=0）
        int directChild = obj.transform.childCount;
        int allTransforms = obj.GetComponentsInChildren<Transform>(true).Length;
        int meshRenderers = obj.GetComponentsInChildren<MeshRenderer>(true).Length;
        int skinnedMesh = obj.GetComponentsInChildren<SkinnedMeshRenderer>(true).Length;
        Debug.Log($"[BodyModel] 实例诊断: name={obj.name}, 直接子节点={directChild}, 全部后代Transform={allTransforms}, MeshRenderer={meshRenderers}, SkinnedMeshRenderer={skinnedMesh}");

        // 诊断：打印模型加载后的原始层级树（真机无 Hierarchy 窗口，借此在日志中查看层级）
        HierarchyDumper.Dump(obj);

        obj.transform.position = new Vector3(0, 0, 0);

        // 先设置 Layer，确保所有节点（包括未激活的）都有正确的 Layer
        SetBodyLayer(obj, UnityLayer.Layer_Body);

        InitializeBoneColliders(obj);
        RegisterBones(obj);

        obj.transform.position = new Vector3(0, 0, 0.5f);
        _body = obj;
        _body.transform.localScale = new Vector3(10, 10, 10);

        _initPos = _body.transform.position;
        _initScale = _body.transform.localScale;
        _initAngle = _body.transform.eulerAngles;
        
        Debug.Log($"[BodyModel] 模型初始化完成, 注册骨骼数: {_registry.Count}, Body active: {_body.activeSelf}");

        // 诊断：打印模型坐标/缩放/包围盒及相机对比，定位"加载成功却看不见"的问题
        LogModelTransform(_body);
    }

    /// <summary>
    /// 诊断输出：模型的坐标、缩放、世界包围盒，以及 ModelCamera 信息与可见性判断。
    /// 用于定位模型已加载但不在相机视野内（位置偏离 / 缩放过大过小 / 被 Layer 剔除）的问题。
    /// </summary>
    private void LogModelTransform(GameObject body)
    {
        if (!body)
        {
            Debug.LogWarning("[BodyModel] LogModelTransform: body 为 null");
            return;
        }

        Transform t = body.transform;
        Debug.Log($"[BodyModel] 变换: 世界坐标={t.position}, 本地坐标={t.localPosition}, localScale={t.localScale}, lossyScale={t.lossyScale}, 旋转={t.eulerAngles}, Layer={body.layer}");

        // 合并所有 Renderer 的世界包围盒，得到模型实际占用空间与中心
        Renderer[] renderers = body.GetComponentsInChildren<Renderer>(true);
        if (renderers.Length == 0)
        {
            Debug.LogWarning("[BodyModel] 变换: 未找到任何 Renderer，无法计算包围盒");
        }
        else
        {
            Bounds bounds = renderers[0].bounds;
            for (int i = 1; i < renderers.Length; i++)
            {
                bounds.Encapsulate(renderers[i].bounds);
            }
            // 渲染剔除看的是 Renderer 所在节点的 Layer（子节点），不是根节点
            int rendererLayer = renderers[0].gameObject.layer;
            bool firstRendererEnabled = renderers[0].enabled;
            Debug.Log($"[BodyModel] 包围盒(世界): 中心={bounds.center}, 尺寸={bounds.size}, min={bounds.min}, max={bounds.max}, 子节点Layer={rendererLayer}, 首个Renderer.enabled={firstRendererEnabled}");

            Camera cam = UIManager.Instance.ModelCamera;
            if (!cam)
            {
                Debug.LogWarning("[BodyModel] ModelCamera 为 null，无法做相机可见性对比");
            }
            else
            {
                Vector3 toCenter = bounds.center - cam.transform.position;
                float dist = toCenter.magnitude;
                float dot = Vector3.Dot(toCenter.normalized, cam.transform.forward);
                bool inCullingMask = (cam.cullingMask & (1 << rendererLayer)) != 0;
                Debug.Log($"[BodyModel] ModelCamera: 坐标={cam.transform.position}, 朝向={cam.transform.forward}, near={cam.nearClipPlane}, far={cam.farClipPlane}, fov={cam.fieldOfView}, 正交={cam.orthographic}, cullingMask={cam.cullingMask}");
                Debug.Log($"[BodyModel] 可见性判断: 模型中心距相机={dist:F3}, 前方点积={dot:F3}(>0在相机前方), 子节点Layer在相机CullingMask内={inCullingMask}");
            }
        }
    }

    /// <summary>
    /// 从 Resources 加载模型预制体并实例化
    /// </summary>
    public GameObject LoadModelPrefab()
    {
        // 直接加载 FBX 文件而不是预制体，避免预制体引用问题
        GameObject source = Resources.Load<GameObject>("Model/jirou_01");
        if (!source)
        {
            Debug.LogError("[BodyModel] 源资源加载失败: Resources.Load(\"Model/jirou_01\") 返回 null");
            return null;
        }
        Debug.Log($"[BodyModel] 源资源诊断: name={source.name}, 直接子节点={source.transform.childCount}, 全部后代Transform={source.GetComponentsInChildren<Transform>(true).Length}");

        return ResManager.Instance.LoadRes<GameObject>("Model/jirou_01");
    }

    /// <summary>
    /// 为每个子对象添加 MeshCollider（如果不存在）
    /// </summary>
    public void InitializeBoneColliders(GameObject root)
    {
        for (int i = 0; i < root.transform.childCount; i++)
        {
            GameObject child = root.transform.GetChild(i).gameObject;
            if (child.GetComponent<MeshCollider>() == null)
            {
                child.AddComponent<MeshCollider>();
            }
        }
    }

    /// <summary>
    /// 解析子对象名称，创建 SkeletonInfo 并注册到 SkeletonRegistry 和 BoneMod.Instance.boneDic。
    /// 先统计有效骨骼数量，再初始化 SkeletonRegistry 容量，最后逐个注册。
    /// </summary>
    public void RegisterBones(GameObject root)
    {
        int childCount = root.transform.childCount;

        // 第一遍：统计有效骨骼数量（名称可解析为 int 的子对象）
        int validCount = 0;
        for (int i = 0; i < childCount; i++)
        {
            if (int.TryParse(root.transform.GetChild(i).gameObject.name, out _))
            {
                validCount++;
            }
        }

        // 诊断：打印实际子节点名字样本，定位真机上的名称/层级问题
        if (childCount > 0)
        {
            string n0 = root.transform.GetChild(0).gameObject.name;
            string n1 = childCount > 1 ? root.transform.GetChild(1).gameObject.name : "-";
            string n2 = childCount > 2 ? root.transform.GetChild(2).gameObject.name : "-";
            Debug.Log($"[BodyModel] RegisterBones: 直接子节点={childCount}, 可解析为ID={validCount}, 子节点样本=[{n0}, {n1}, {n2}]");
        }
        else
        {
            Debug.LogWarning("[BodyModel] RegisterBones: 直接子节点为 0! 模型层级异常，请检查 prefab 打包结果。");
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
        trans.gameObject.layer = layer;

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
    /// visible=true 时设置 Layer 为 Layer_Body 并激活；
    /// visible=false 时设置 Layer 为 Layer_Default 并停用。
    /// </summary>
    public void SetBodyVisible(bool visible)
    {
        if (_body == null) return;

        if (visible)
        {
            SetBodyLayer(_body, UnityLayer.Layer_Body);
        }
        else
        {
            SetBodyLayer(_body, UnityLayer.Layer_Default);
        }

        _body.SetActive(visible);
    }
}
