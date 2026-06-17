using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using RuntimeInspectorNamespace;

/// <summary>
/// 运行时检视器(RuntimeInspector &amp; Hierarchy)开关。
/// 提供两种方式打开/关闭运行时层级树与属性检视面板:
///   1) OnGUI(IMGUI)悬浮按钮 —— 不依赖 Canvas/EventSystem, 永远绘制在最顶层, 最稳妥;
///   2) 单指长按 10 秒手势 —— 隐蔽无按钮, 适合不想占用屏幕时使用。
/// 用于真机(无编辑器 Hierarchy/Inspector 窗口)环境下调试场景对象、查看与编辑骨骼模型节点。
///
/// 设计要点:
/// - 通过 RuntimeInitializeOnLoadMethod 自动引导, 无需手动挂载或配置场景;
/// - 面板(连同其 Canvas/EventSystem)懒加载, 首次打开才创建, 不打开则零 UI 开销;
/// - 长按检测基于旧版输入系统, 仅单指、移动不超过阈值时才计时, 避免与旋转/拖拽冲突;
/// - 面板交互走独立高层级 Overlay Canvas, 不污染业务 UI。
/// </summary>
public class RuntimeInspectorToggle : MonoBehaviour
{
    // 预制体位于 Assets/Plugins/RuntimeInspector/Resources/RuntimeInspector/ 下, 走 Resources 加载
    private const string HierarchyResourcePath = "RuntimeInspector/RuntimeHierarchy";
    private const string InspectorResourcePath = "RuntimeInspector/RuntimeInspector";

    // 单指长按触发时长(秒)
    private const float LongPressDuration = 10f;

    // 常驻单例(DontDestroyOnLoad), 供静态 API 访问
    private static RuntimeInspectorToggle _instance;

    // 触发方式开关: 如只需其中一种, 将另一个置为 false 即可
    private bool _enableOnGUIButton = true;
    private bool _enableLongPressGesture = true;

    private Canvas _canvas;
    private RectTransform _panelRoot;          // 承载 hierarchy + inspector, 整体显隐
    private RuntimeHierarchy _hierarchy;
    private RuntimeInspector _inspector;
    private bool _panelVisible;

    // 长按手势运行时状态
    private bool _pressing;
    private float _pressTimer;
    private Vector2 _pressStartPos;
    private bool _longPressConsumed;

    // OnGUI 按钮样式缓存(避免每帧分配)
    private GUIStyle _buttonStyle;

    /// <summary>游戏启动后自动创建常驻开关(无需场景配置)。</summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Bootstrap()
    {
        if (_instance)
        {
            return;
        }

        GameObject go = new GameObject("[RuntimeInspectorToggle]");
        DontDestroyOnLoad(go);
        _instance = go.AddComponent<RuntimeInspectorToggle>();
    }

    /// <summary>打开检视面板。</summary>
    public static void Show()
    {
        if (_instance)
        {
            _instance.SetPanelVisible(true);
        }
    }

    /// <summary>关闭检视面板。</summary>
    public static void Hide()
    {
        if (_instance)
        {
            _instance.SetPanelVisible(false);
        }
    }

    /// <summary>切换检视面板显隐。</summary>
    public static void Toggle()
    {
        if (_instance)
        {
            _instance.SetPanelVisible(!_instance._panelVisible);
        }
    }

    private void Update()
    {
        if (_enableLongPressGesture)
        {
            UpdateLongPress();
        }
    }

    private void OnGUI()
    {
        if (!_enableOnGUIButton)
        {
            return;
        }

        if (_buttonStyle == null)
        {
            _buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = Mathf.RoundToInt(Screen.height * 0.022f)
            };
        }

        float w = Screen.width * 0.26f;
        float h = Screen.height * 0.06f;
        float margin = Screen.height * 0.015f;
        Rect rect = new Rect(Screen.width - w - margin, margin, w, h);

        if (GUI.Button(rect, _panelVisible ? "关闭检视器" : "打开检视器", _buttonStyle))
        {
            SetPanelVisible(!_panelVisible);
        }
    }

    /// <summary>检测单指长按: 持续按住、移动不超过阈值, 达到时长则切换面板。</summary>
    private void UpdateLongPress()
    {
        Vector2 pos;
        if (!IsSinglePointerDown(out pos))
        {
            ResetLongPress();
            return;
        }

        if (!_pressing)
        {
            _pressing = true;
            _pressTimer = 0f;
            _pressStartPos = pos;
            _longPressConsumed = false;
            return;
        }

        // 移动超过容差视为拖拽(如旋转模型), 取消本次长按计时
        float tolerance = Screen.height * 0.05f;
        if ((pos - _pressStartPos).sqrMagnitude > tolerance * tolerance)
        {
            ResetLongPress();
            return;
        }

        if (_longPressConsumed)
        {
            return;
        }

        _pressTimer += Time.unscaledDeltaTime;
        if (_pressTimer >= LongPressDuration)
        {
            _longPressConsumed = true; // 触发后需抬起再按, 避免重复触发
            SetPanelVisible(!_panelVisible);
        }
    }

    private void ResetLongPress()
    {
        _pressing = false;
        _pressTimer = 0f;
        _longPressConsumed = false;
    }

    /// <summary>当前是否恰好有单个指针(单指触摸优先, 无触摸时退回鼠标)按下, 并输出其屏幕坐标。</summary>
    private static bool IsSinglePointerDown(out Vector2 position)
    {
        position = default;

        int touchCount = Input.touchCount;
        if (touchCount > 0)
        {
            if (touchCount != 1)
            {
                return false; // 多指不算单指长按
            }

            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                return false;
            }

            position = touch.position;
            return true;
        }

        // 无触摸输入时退回鼠标(编辑器/PC 调试)
        if (Input.GetMouseButton(0))
        {
            position = Input.mousePosition;
            return true;
        }

        return false;
    }

    /// <summary>设置检视面板显隐; 首次打开时懒加载创建 Canvas/EventSystem 与面板。</summary>
    private void SetPanelVisible(bool visible)
    {
        if (visible && !_panelRoot)
        {
            EnsureCanvas();
            EnsureEventSystem();
            BuildPanel();
        }

        if (_panelRoot)
        {
            _panelRoot.gameObject.SetActive(visible);
        }

        _panelVisible = visible;
    }

    /// <summary>构建独立的全屏 Overlay Canvas, 置于最顶层。</summary>
    private void EnsureCanvas()
    {
        if (_canvas)
        {
            return;
        }

        _canvas = gameObject.AddComponent<Canvas>();
        _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        _canvas.sortingOrder = short.MaxValue;

        CanvasScaler scaler = gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080f, 1920f); // 项目为竖屏
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        gameObject.AddComponent<GraphicRaycaster>();
    }

    /// <summary>确保场景中存在 EventSystem(面板 UI 交互所需); 没有则补建。项目使用旧版输入系统。</summary>
    private void EnsureEventSystem()
    {
        if (EventSystem.current || FindAnyObjectByType<EventSystem>())
        {
            return;
        }

        GameObject es = new GameObject("[RuntimeInspectorToggle] EventSystem");
        es.transform.SetParent(transform, false);
        es.AddComponent<EventSystem>();
        es.AddComponent<StandaloneInputModule>();
    }

    /// <summary>懒加载并装配 RuntimeHierarchy 与 RuntimeInspector 面板, 并双向连接。</summary>
    private void BuildPanel()
    {
        _panelRoot = (RectTransform)CreateUIElement("InspectorPanel", _canvas.transform).transform;
        AnchorStretch(_panelRoot, 0f, 0f, 1f, 1f);

        _hierarchy = InstantiatePanel<RuntimeHierarchy>(HierarchyResourcePath, "RuntimeHierarchy");
        _inspector = InstantiatePanel<RuntimeInspector>(InspectorResourcePath, "RuntimeInspector");

        // 在层级中选中对象时, 检视器自动检视该对象; 反之检视器中高亮引用时, 同步层级选中
        if (_hierarchy)
        {
            _hierarchy.ConnectedInspector = _inspector;
        }
        if (_inspector)
        {
            _inspector.ConnectedHierarchy = _hierarchy;
        }

        LayoutPanels();
    }

    /// <summary>竖屏布局: 层级树占上半, 检视器占下半。</summary>
    private void LayoutPanels()
    {
        if (_hierarchy)
        {
            AnchorStretch((RectTransform)_hierarchy.transform, 0.02f, 0.40f, 0.98f, 0.88f);
        }
        if (_inspector)
        {
            AnchorStretch((RectTransform)_inspector.transform, 0.02f, 0.02f, 0.98f, 0.38f);
        }
    }

    private T InstantiatePanel<T>(string resourcePath, string displayName) where T : Component
    {
        T prefab = Resources.Load<T>(resourcePath);
        if (!prefab)
        {
            Debug.LogError($"[RuntimeInspectorToggle] 预制体加载失败: Resources/{resourcePath}");
            return null;
        }

        T instance = Instantiate(prefab, _panelRoot, false);
        instance.name = displayName;
        return instance;
    }

    private static GameObject CreateUIElement(string name, Transform parent)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.layer = LayerMask.NameToLayer("UI");
        go.transform.SetParent(parent, false);
        return go;
    }

    private static void AnchorStretch(RectTransform rt, float minX, float minY, float maxX, float maxY)
    {
        rt.anchorMin = new Vector2(minX, minY);
        rt.anchorMax = new Vector2(maxX, maxY);
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }
}
