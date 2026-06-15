using System.IO;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;

/// <summary>
/// 骨骼数据测试窗口 — 在 Unity 运行模式下加载本地 bonedata.txt，
/// 通过 ButtonBehavior.ReceiveMessage 注入，完整复刻真机数据接收流程。
///
/// 真机流程：App(原生) -> ButtonBehavior.ReceiveMessage(jsonString)
/// 本窗口流程：读取本地文件文本 -> ButtonBehavior.ReceiveMessage(fileText)
/// 两者入口完全一致，仅数据来源不同（原生消息 vs 本地文件）。
/// </summary>
public class BoneDataTestWindow : EditorWindow
{
    // 默认数据路径（Resources 下的 bonedata.txt）
    private const string DefaultRelativePath = "Assets/Resources/bonedata.txt";

    private string _dataPath;
    private Vector2 _scroll;
    private string _lastMessage = "";

    // ButtonBehavior 消息代码（与 AppToUnityCode 保持一致）
    private const int CodeReceiveBoneConfig = 3;   // 接收骨骼配置
    private const int CodeShowByType = 5;          // 按类型筛选（EnumBone）
    private const int CodeShowByPosition = 6;      // 按部位筛选

    // 可筛选的类型（EnumBone：骨骼/肌肉/筋膜）
    private static readonly (string label, EnumBone value)[] TypeOptions =
    {
        ("骨骼", EnumBone.Bone),
        ("肌肉", EnumBone.Muscle),
        ("筋膜", EnumBone.Fascia),
    };

    // 当前各类型的勾选状态
    private bool[] _typeSelected = new bool[TypeOptions.Length];

    // 可筛选的部位（排除 None / All，单独提供按钮）
    private static readonly (string label, EnumPos value)[] PosOptions =
    {
        ("上肢", EnumPos.UpperLimbs),
        ("肩背", EnumPos.ShoulderBack),
        ("下肢", EnumPos.LowerLimbs),
        ("盆骨", EnumPos.Pelvis),
        ("头颈", EnumPos.HeadAndNeck),
        ("胸腹", EnumPos.ChestAndAbdomen),
        ("脊柱", EnumPos.Spine),
        ("肩胛带", EnumPos.Scapula),
    };

    // 当前各部位的勾选状态
    private bool[] _posSelected = new bool[PosOptions.Length];

    [MenuItem("Tools/骨骼工具/骨骼数据测试 (模拟真机)")]
    public static void Open()
    {
        var window = GetWindow<BoneDataTestWindow>("骨骼数据测试");
        window.minSize = new Vector2(420, 260);
        window.Show();
    }

    private void OnEnable()
    {
        if (string.IsNullOrEmpty(_dataPath))
        {
            // 转换为绝对路径，便于读取与显示
            _dataPath = Path.GetFullPath(DefaultRelativePath);
        }

        // 类型筛选默认全选（进入时视为显示全部类型）
        for (int i = 0; i < _typeSelected.Length; i++) _typeSelected[i] = true;
    }

    private void OnGUI()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("模拟真机数据注入", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "在 Unity 运行模式下，读取本地 bonedata.txt 并通过 " +
            "ButtonBehavior.ReceiveMessage 注入，流程与真机一致。",
            MessageType.Info);

        EditorGUILayout.Space();

        // 数据文件路径
        EditorGUILayout.LabelField("数据文件路径", EditorStyles.miniBoldLabel);
        using (new EditorGUILayout.HorizontalScope())
        {
            _dataPath = EditorGUILayout.TextField(_dataPath);
            if (GUILayout.Button("浏览", GUILayout.Width(60)))
            {
                string picked = EditorUtility.OpenFilePanel("选择骨骼数据文件", Application.dataPath, "txt");
                if (!string.IsNullOrEmpty(picked))
                {
                    _dataPath = picked;
                }
            }
            if (GUILayout.Button("重置", GUILayout.Width(60)))
            {
                _dataPath = Path.GetFullPath(DefaultRelativePath);
            }
        }

        bool fileExists = File.Exists(_dataPath);
        if (!fileExists)
        {
            EditorGUILayout.HelpBox("文件不存在：" + _dataPath, MessageType.Warning);
        }

        EditorGUILayout.Space();

        // 运行状态检查
        bool isPlaying = EditorApplication.isPlaying;
        if (!isPlaying)
        {
            EditorGUILayout.HelpBox("请先进入 Play 模式（运行）后再注入数据。", MessageType.Warning);
        }

        using (new EditorGUI.DisabledScope(!isPlaying || !fileExists))
        {
            if (GUILayout.Button("加载并注入数据 (走真机流程)", GUILayout.Height(36)))
            {
                LoadAndInject();
            }
        }

        EditorGUILayout.Space();

        DrawPositionFilter(isPlaying);

        EditorGUILayout.Space();

        DrawBoneTypeFilter(isPlaying);

        EditorGUILayout.Space();

        // 上次注入的消息预览
        if (!string.IsNullOrEmpty(_lastMessage))
        {
            EditorGUILayout.LabelField("上次注入内容预览", EditorStyles.miniBoldLabel);
            _scroll = EditorGUILayout.BeginScrollView(_scroll, GUILayout.Height(120));
            EditorGUILayout.TextArea(_lastMessage, GUILayout.ExpandHeight(true));
            EditorGUILayout.EndScrollView();
        }
    }

    /// <summary>
    /// 绘制部位筛选区域：每个部位一个开关，支持单选与任意组合。
    /// 筛选通过 ButtonBehavior.ReceiveMessage(code=6) 下发，与真机流程一致。
    /// </summary>
    private void DrawPositionFilter(bool isPlaying)
    {
        EditorGUILayout.LabelField("按部位筛选 (EnumPos)", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "勾选一个或多个部位后点击「应用部位筛选」，通过真机消息入口(code=6)下发组合位标志。",
            MessageType.None);

        using (new EditorGUI.DisabledScope(!isPlaying))
        {
            // 每行放 4 个开关
            const int perRow = 4;
            for (int i = 0; i < PosOptions.Length; i++)
            {
                if (i % perRow == 0) EditorGUILayout.BeginHorizontal();

                _posSelected[i] = GUILayout.Toggle(
                    _posSelected[i], PosOptions[i].label, "Button", GUILayout.Height(26));

                if (i % perRow == perRow - 1 || i == PosOptions.Length - 1)
                    EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space();

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("全选", GUILayout.Width(60)))
                {
                    for (int i = 0; i < _posSelected.Length; i++) _posSelected[i] = true;
                }
                if (GUILayout.Button("清空", GUILayout.Width(60)))
                {
                    for (int i = 0; i < _posSelected.Length; i++) _posSelected[i] = false;
                }
            }

            EditorGUILayout.Space();

            int flags = GetSelectedPosFlags();
            EditorGUILayout.LabelField("当前组合值", flags == 0 ? "无 (0)" : $"{(EnumPos)flags} ({flags})");

            if (GUILayout.Button("应用部位筛选 (走真机流程)", GUILayout.Height(32)))
            {
                ApplyPositionFilter(flags);
            }

            if (GUILayout.Button("显示全部 (All)", GUILayout.Height(24)))
            {
                ApplyPositionFilter((int)EnumPos.All);
            }
        }
    }

    private int GetSelectedPosFlags()
    {
        int flags = 0;
        for (int i = 0; i < PosOptions.Length; i++)
        {
            if (_posSelected[i]) flags |= (int)PosOptions[i].value;
        }
        return flags;
    }

    /// <summary>
    /// 按真机流程下发部位筛选：ButtonBehavior.ReceiveMessage(code=6, msg=位标志整数)
    /// </summary>
    private void ApplyPositionFilter(int posFlags)
    {
        ButtonBehavior buttonBehavior = Object.FindObjectOfType<ButtonBehavior>();
        if (buttonBehavior == null)
        {
            Debug.LogError("[BoneDataTest] 场景中未找到 ButtonBehavior，无法应用部位筛选。");
            return;
        }

        // 构造与真机一致的外层封装消息：{"code":6,"msg":"<posFlags>"}
        var payload = new ButtonBehaviorCustomData
        {
            code = CodeShowByPosition,
            msg = posFlags.ToString()
        };
        string jsonString = JsonConvert.SerializeObject(payload);

        _lastMessage = jsonString;
        buttonBehavior.ReceiveMessage(jsonString);
        Debug.Log($"[BoneDataTest] 已应用部位筛选：{(EnumPos)posFlags} ({posFlags})");
        Repaint();
    }

    /// <summary>
    /// 绘制类型筛选区域：骨骼 / 肌肉 / 筋膜（EnumBone）。
    /// 筛选通过 ButtonBehavior.ReceiveMessage(code=5) 下发，内部会与当前部位筛选叠加(交集)，
    /// 即只对"当前显示的部位"做类型筛选，与真机流程一致。
    /// </summary>
    private void DrawBoneTypeFilter(bool isPlaying)
    {
        EditorGUILayout.LabelField("按类型筛选 (EnumBone)", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "勾选骨骼/肌肉/筋膜后点击「应用类型筛选」，通过真机消息入口(code=5)下发组合位标志。\n" +
            "该筛选只作用于当前显示的部位：例如先选「头颈」再筛类型，只影响头颈范围。",
            MessageType.None);

        using (new EditorGUI.DisabledScope(!isPlaying))
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                for (int i = 0; i < TypeOptions.Length; i++)
                {
                    _typeSelected[i] = GUILayout.Toggle(
                        _typeSelected[i], TypeOptions[i].label, "Button", GUILayout.Height(26));
                }
            }

            EditorGUILayout.Space();

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("全选", GUILayout.Width(60)))
                {
                    for (int i = 0; i < _typeSelected.Length; i++) _typeSelected[i] = true;
                }
                if (GUILayout.Button("清空", GUILayout.Width(60)))
                {
                    for (int i = 0; i < _typeSelected.Length; i++) _typeSelected[i] = false;
                }
            }

            EditorGUILayout.Space();

            int flags = GetSelectedTypeFlags();
            EditorGUILayout.LabelField("当前组合值", flags == 0 ? "无 (0)" : $"{(EnumBone)flags} ({flags})");

            if (GUILayout.Button("应用类型筛选 (走真机流程)", GUILayout.Height(32)))
            {
                ApplyBoneTypeFilter(flags);
            }

            if (GUILayout.Button("显示全部类型 (All)", GUILayout.Height(24)))
            {
                ApplyBoneTypeFilter((int)EnumBone.All);
            }
        }
    }

    private int GetSelectedTypeFlags()
    {
        int flags = 0;
        for (int i = 0; i < TypeOptions.Length; i++)
        {
            if (_typeSelected[i]) flags |= (int)TypeOptions[i].value;
        }
        return flags;
    }

    /// <summary>
    /// 按真机流程下发类型筛选：ButtonBehavior.ReceiveMessage(code=5, msg=位标志整数)。
    /// 内部 GameObjectManager 会用该类型与当前部位状态取交集，只影响当前显示的部位。
    /// </summary>
    private void ApplyBoneTypeFilter(int typeFlags)
    {
        ButtonBehavior buttonBehavior = Object.FindObjectOfType<ButtonBehavior>();
        if (!buttonBehavior)
        {
            Debug.LogError("[BoneDataTest] 场景中未找到 ButtonBehavior，无法应用类型筛选。");
            return;
        }

        // 构造与真机一致的外层封装消息：{"code":5,"msg":"<typeFlags>"}
        var payload = new ButtonBehaviorCustomData
        {
            code = CodeShowByType,
            msg = typeFlags.ToString()
        };
        string jsonString = JsonConvert.SerializeObject(payload);

        _lastMessage = jsonString;
        buttonBehavior.ReceiveMessage(jsonString);
        Debug.Log($"[BoneDataTest] 已应用类型筛选：{(EnumBone)typeFlags} ({typeFlags})");
        Repaint();
    }

    private void LoadAndInject()
    {
        if (!File.Exists(_dataPath))
        {
            Debug.LogError("[BoneDataTest] 数据文件不存在：" + _dataPath);
            return;
        }

        string json;
        try
        {
            json = File.ReadAllText(_dataPath);
        }
        catch (System.Exception e)
        {
            Debug.LogError("[BoneDataTest] 读取文件失败：" + e.Message);
            return;
        }

        if (string.IsNullOrWhiteSpace(json))
        {
            Debug.LogWarning("[BoneDataTest] 数据文件内容为空");
            return;
        }

        // 查找场景中的 ButtonBehavior（真机消息入口）
        ButtonBehavior buttonBehavior = Object.FindObjectOfType<ButtonBehavior>();
        if (buttonBehavior == null)
        {
            Debug.LogError("[BoneDataTest] 场景中未找到 ButtonBehavior，无法注入数据。请确认已运行并初始化。");
            return;
        }

        // 完整复刻真机流程：调用与原生侧相同的入口方法
        _lastMessage = json.Length > 2000 ? json.Substring(0, 2000) + "\n...(已截断)" : json;
        buttonBehavior.ReceiveMessage(json);
        Debug.Log("[BoneDataTest] 已注入骨骼数据，长度：" + json.Length);
        Repaint();
    }
}
