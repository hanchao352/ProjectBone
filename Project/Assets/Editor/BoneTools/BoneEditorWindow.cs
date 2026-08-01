using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

/// <summary>
/// 骨骼编辑器窗口 — Unity 运行后自动加载数据并显示模型。
/// 提供部位切换按钮、骨骼操作按钮、肌肉加减功能。
/// </summary>
public class BoneEditorWindow : EditorWindow
{
    // ========== Data Structures ==========

    [Serializable]
    private class BoneEntry
    {
        public int id;
        public string name;
        public int typeId;
        public int direction;
        public int[] positionList;

        public string TypeName => typeId switch
        {
            0 => "其它",
            1 => "骨骼",
            2 => "肌肉",
            3 => "筋膜",
            _ => "未知"
        };

        public int PositionFlags
        {
            get
            {
                if (positionList == null) return 0;
                int flags = 0;
                foreach (int pos in positionList)
                    flags |= (1 << pos);
                return flags;
            }
        }
    }

    [Serializable]
    private class BoneEntryRaw
    {
        public int id;
        public string type;
        public string name;
        public int direction;
        public string position;
        public int type_id;
        public int[] position_list;
    }

    [Serializable]
    private class ModelDataContent
    {
        public List<BoneEntryRaw> list;
    }

    [Serializable]
    private class ModelData
    {
        public int code;
        public string msg;
        public ModelDataContent data;
    }

    // ========== State ==========

    private List<BoneEntry> boneEntries;
    private bool dataLoaded = false;
    private bool configApplied = false;
    private bool autoInitDone = false;
    private string errorMessage;

    // 肌肉层级
    private int muscleLayerIndex = 2; // 0=Bone, 1=Bone|Muscle, 2=All
    private static readonly int[] layerValues = { 1, 3, 7 }; // Bone, Bone|Muscle, All

    // 当前选中的部位按钮索引
    private int selectedPosIndex = 0; // 0=全身

    // 部位按钮定义
    private static readonly string[] posButtonLabels = { "全身", "头颈", "肩背", "上肢", "胸腹", "脊柱", "下肢", "盆骨" };
    private static readonly int[] posButtonValues = {
        (int)EnumPos.All,
        (int)EnumPos.HeadAndNeck,
        (int)EnumPos.ShoulderBack,
        (int)EnumPos.UpperLimbs,
        (int)EnumPos.ChestAndAbdomen,
        (int)EnumPos.Spine,
        (int)EnumPos.LowerLimbs,
        (int)EnumPos.Pelvis
    };

    // ========== Menu Entry ==========

    [MenuItem("Tools/Bone Editor Window")]
    static void ShowWindow()
    {
        GetWindow<BoneEditorWindow>("Bone Editor");
    }

    // ========== Lifecycle ==========

    private void OnEnable()
    {
        boneEntries = new List<BoneEntry>();
        dataLoaded = false;
        configApplied = false;
        autoInitDone = false;
        errorMessage = null;
        muscleLayerIndex = 2;
        selectedPosIndex = 0;

        LoadBoneData();

        EditorApplication.playModeStateChanged -= OnPlayModeChanged;
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    private void OnDisable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeChanged;
    }

    private void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingPlayMode)
        {
            configApplied = false;
            autoInitDone = false;
        }
    }

    // ========== Data Loading ==========

    private void LoadBoneData()
    {
        boneEntries = new List<BoneEntry>();

        string jsonText = null;
        TextAsset textAsset = Resources.Load<TextAsset>("modeldate");
        if (textAsset != null)
        {
            jsonText = textAsset.text;
        }
        else
        {
            string filePath = System.IO.Path.Combine(Application.dataPath, "Resources", "modeldate");
            if (System.IO.File.Exists(filePath))
            {
                jsonText = System.IO.File.ReadAllText(filePath);
            }
            else
            {
                errorMessage = "无法加载 modeldate 文件";
                Debug.LogError("[BoneEditorWindow] " + errorMessage);
                return;
            }
        }

        try
        {
            ModelData modelData = JsonConvert.DeserializeObject<ModelData>(jsonText);
            if (modelData == null || modelData.data == null || modelData.data.list == null || modelData.data.list.Count == 0)
            {
                errorMessage = "modeldate 中无骨骼数据";
                Debug.LogError("[BoneEditorWindow] " + errorMessage);
                return;
            }

            foreach (var raw in modelData.data.list)
            {
                boneEntries.Add(new BoneEntry
                {
                    id = raw.id,
                    name = raw.name,
                    typeId = raw.type_id,
                    direction = raw.direction,
                    positionList = raw.position_list
                });
            }

            dataLoaded = true;
            errorMessage = null;
        }
        catch (Exception e)
        {
            errorMessage = "modeldate 解析失败: " + e.Message;
            Debug.LogError("[BoneEditorWindow] " + errorMessage);
        }
    }

    /// <summary>
    /// 用 modeldate 数据初始化骨骼配置（Pos、Boneenum、Direction）
    /// </summary>
    private void ApplyBoneConfig()
    {
        if (configApplied) return;
        if (BoneMod.Instance == null || BoneMod.Instance.boneDic == null) return;
        if (boneEntries == null || boneEntries.Count == 0) return;

        foreach (var entry in boneEntries)
        {
            if (BoneMod.Instance.boneDic.ContainsKey(entry.id))
            {
                Bone bone = BoneMod.Instance.boneDic[entry.id];
                bone.Boneenum = BoneTypeMapper.FromTypeId(entry.typeId);
                bone.Pos = entry.PositionFlags;
                bone.Direction = entry.direction;
            }
        }

        configApplied = true;
    }

    /// <summary>
    /// 自动初始化：应用配置 + 显示模型 + 显示全部
    /// </summary>
    private void AutoInit()
    {
        if (autoInitDone) return;
        if (GameObjectManager.Instance == null) return;
        if (GameObjectManager.Instance.Body == null) return;

        ApplyBoneConfig();

        if (!configApplied) return;

        GameObjectManager.Instance.BodyVisible = true;
        GameObjectManager.Instance.ShowBoneByType((int)BoneShowType.All);

        autoInitDone = true;
        Debug.Log("[BoneEditorWindow] 自动初始化完成");
    }

    // ========== OnGUI ==========

    private void OnGUI()
    {
        bool isPlaying = EditorApplication.isPlaying;

        if (!isPlaying)
        {
            EditorGUILayout.HelpBox("请进入 Play Mode 后使用", MessageType.Warning);
            return;
        }

        if (!string.IsNullOrEmpty(errorMessage))
        {
            EditorGUILayout.HelpBox(errorMessage, MessageType.Error);
            return;
        }

        // 自动初始化
        if (!autoInitDone)
        {
            AutoInit();
            if (!autoInitDone)
            {
                EditorGUILayout.HelpBox("等待模型加载...", MessageType.Info);
                Repaint();
                return;
            }
        }

        // === 部位切换按钮 ===
        DrawPositionButtons();

        EditorGUILayout.Space(8);

        // === 肌肉加减 ===
        DrawMuscleLayerButtons();

        EditorGUILayout.Space(8);

        // === 选中信息 & 操作按钮 ===
        DrawSelectedInfo();
        DrawActionButtons();

        EditorGUILayout.Space(8);

        // === 重置 ===
        DrawResetButtons();
    }

    // ========== UI Drawing ==========

    private void DrawPositionButtons()
    {
        EditorGUILayout.LabelField("部位切换", EditorStyles.boldLabel);

        // 第一行：全身
        if (GUILayout.Button("全身", selectedPosIndex == 0 ? GetSelectedButtonStyle() : GUI.skin.button))
        {
            selectedPosIndex = 0;
            GameObjectManager.Instance.ShowBoneByType(layerValues[muscleLayerIndex]);
            // 全身不需要 SelectBoneByPos，ShowBoneByType(All) 已经显示全部
        }

        // 其他部位按钮，每行2个
        for (int i = 1; i < posButtonLabels.Length; i += 2)
        {
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button(posButtonLabels[i], selectedPosIndex == i ? GetSelectedButtonStyle() : GUI.skin.button))
            {
                selectedPosIndex = i;
                GameObjectManager.Instance.SelectBoneByPos(posButtonValues[i]);
            }

            if (i + 1 < posButtonLabels.Length)
            {
                if (GUILayout.Button(posButtonLabels[i + 1], selectedPosIndex == i + 1 ? GetSelectedButtonStyle() : GUI.skin.button))
                {
                    selectedPosIndex = i + 1;
                    GameObjectManager.Instance.SelectBoneByPos(posButtonValues[i + 1]);
                }
            }

            EditorGUILayout.EndHorizontal();
        }
    }

    private void DrawMuscleLayerButtons()
    {
        EditorGUILayout.LabelField("肌肉层级", EditorStyles.boldLabel);

        string layerInfo = muscleLayerIndex switch
        {
            0 => "当前: 骨骼",
            1 => "当前: 骨骼 + 肌肉",
            2 => "当前: 全部",
            _ => ""
        };
        EditorGUILayout.LabelField(layerInfo, EditorStyles.miniLabel);

        EditorGUILayout.BeginHorizontal();

        EditorGUI.BeginDisabledGroup(muscleLayerIndex >= 2);
        if (GUILayout.Button("肌肉加"))
        {
            muscleLayerIndex++;
            GameObjectManager.Instance.ShowBoneByType(layerValues[muscleLayerIndex]);
        }
        EditorGUI.EndDisabledGroup();

        EditorGUI.BeginDisabledGroup(muscleLayerIndex <= 0);
        if (GUILayout.Button("肌肉减"))
        {
            muscleLayerIndex--;
            GameObjectManager.Instance.ShowBoneByType(layerValues[muscleLayerIndex]);
        }
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.EndHorizontal();
    }

    private void DrawSelectedInfo()
    {
        EditorGUILayout.LabelField("选中骨骼", EditorStyles.boldLabel);

        if (BoneMod.Instance != null && BoneMod.Instance.selectedBoneIds != null && BoneMod.Instance.selectedBoneIds.Count > 0)
        {
            int currentId = BoneMod.Instance.CurrentBoneId;
            if (currentId != 0 && BoneMod.Instance.boneDic.ContainsKey(currentId))
            {
                var bone = BoneMod.Instance.boneDic[currentId];
                EditorGUILayout.LabelField($"{bone.Name} (ID: {currentId})");
            }
            else
            {
                EditorGUILayout.LabelField($"ID: {currentId}");
            }

            if (BoneMod.Instance.selectedBoneIds.Count > 1)
            {
                EditorGUILayout.LabelField($"共选中 {BoneMod.Instance.selectedBoneIds.Count} 个");
            }
        }
        else
        {
            EditorGUILayout.LabelField("点击模型选中骨骼");
        }
    }

    private void DrawActionButtons()
    {
        bool hasSelection = BoneMod.Instance != null
            && BoneMod.Instance.selectedBoneIds != null
            && BoneMod.Instance.selectedBoneIds.Count > 0;

        EditorGUI.BeginDisabledGroup(!hasSelection);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("隐藏")) GameObjectManager.Instance.HideBone();
        if (GUILayout.Button("显示")) GameObjectManager.Instance.ShowBone();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("透明")) GameObjectManager.Instance.TransparentBone();
        if (GUILayout.Button("实体")) GameObjectManager.Instance.SolidBone();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("单独显示")) GameObjectManager.Instance.HideOtherBone();
        if (GUILayout.Button("显示其他")) GameObjectManager.Instance.ShowOtherBone();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("透明其他")) GameObjectManager.Instance.TransparentOtherBone();
        if (GUILayout.Button("实体其他")) GameObjectManager.Instance.SolidOtherBone();
        EditorGUILayout.EndHorizontal();

        EditorGUI.EndDisabledGroup();
    }

    private void DrawResetButtons()
    {
        EditorGUILayout.LabelField("重置", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("重置颜色")) GameObjectManager.Instance.ResetBoneColor();
        if (GUILayout.Button("重置透明度")) GameObjectManager.Instance.ResetBoneTransparency();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("复位变换")) GameObjectManager.Instance.ResetTransform();
        if (GUILayout.Button("完全重置"))
        {
            GameObjectManager.Instance.ResetAll();
            muscleLayerIndex = 2;
            selectedPosIndex = 0;
        }
        EditorGUILayout.EndHorizontal();
    }

    // ========== Helpers ==========

    private GUIStyle _selectedBtnStyle;
    private GUIStyle GetSelectedButtonStyle()
    {
        if (_selectedBtnStyle == null)
        {
            _selectedBtnStyle = new GUIStyle(GUI.skin.button);
            _selectedBtnStyle.normal.textColor = Color.white;
            _selectedBtnStyle.fontStyle = FontStyle.Bold;
            _selectedBtnStyle.normal.background = MakeTex(2, 2, new Color(0.2f, 0.5f, 0.9f, 0.8f));
        }
        return _selectedBtnStyle;
    }

    private static Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; i++) pix[i] = col;
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }
}
