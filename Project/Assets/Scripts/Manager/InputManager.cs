using System;
using DigitalRubyShared;
using UnityEngine;

public class InputManager : SingletonManager<InputManager>, IGeneric
{
    // 手势识别器
    private PanGestureRecognizer oneFingerPanGesture;
    private PanGestureRecognizer twoFingerPanGesture;
    private ScaleGestureRecognizer scaleGesture;
    private TapGestureRecognizer tapGesture;

    public LayerMask targetLayer;

    // 缩放限制
    public float scaleMin = 0.1f;
    public float scaleMax = float.MaxValue;

    // 灵敏度
    public float rotationSpeed = 0.3f;
    public float panSpeed = 0.003f;

    // 惯性参数
    private bool isRotationInertia = false;
    private Vector2 rotationVelocity;
    private float inertiaDecay = 5f;           // 惯性衰减速度
    private float inertiaThreshold = 0.1f;     // 惯性停止阈值
    private float velocitySmoothing = 0.1f;    // 速度平滑系数

    // 平滑旋转
    private Vector2 smoothedDelta;
    private Vector2 lastDelta;

    // 旋转轴锁定
    private bool axisLocked = false;
    private bool lockedToHorizontal = false;
    private float axisLockThreshold = 5f;      // 累计多少像素后锁定方向
    private Vector2 accumulatedDelta;

    public override void Initialize()
    {
        base.Initialize();
        targetLayer = 1 << UnityLayer.Layer_Body;

        CreateTapGesture();
        CreateOneFingerPanGesture();
        CreateTwoFingerPanGesture();
        CreateScaleGesture();

        // 双指平移和缩放可以同时执行
        twoFingerPanGesture.AllowSimultaneousExecution(scaleGesture);
        scaleGesture.AllowSimultaneousExecution(twoFingerPanGesture);

        // 单指拖拽和双指平移允许同时执行（由回调内部判断是否响应）
        oneFingerPanGesture.AllowSimultaneousExecution(twoFingerPanGesture);
        twoFingerPanGesture.AllowSimultaneousExecution(oneFingerPanGesture);
        oneFingerPanGesture.AllowSimultaneousExecution(scaleGesture);
        scaleGesture.AllowSimultaneousExecution(oneFingerPanGesture);

        // Tap 必须等单指拖拽判定失败才触发
        tapGesture.RequireGestureRecognizerToFail = oneFingerPanGesture;
    }

    #region 手势创建

    private void CreateTapGesture()
    {
        tapGesture = new TapGestureRecognizer();
        tapGesture.StateUpdated += OnTapGesture;
        FingersScript.Instance.AddGesture(tapGesture);
    }

    private void CreateOneFingerPanGesture()
    {
        oneFingerPanGesture = new PanGestureRecognizer();
        oneFingerPanGesture.MinimumNumberOfTouchesToTrack = 1;
        oneFingerPanGesture.MaximumNumberOfTouchesToTrack = 1;
        oneFingerPanGesture.ThresholdUnits = 0.01f;
        oneFingerPanGesture.StateUpdated += OnOneFingerPanGesture;
        FingersScript.Instance.AddGesture(oneFingerPanGesture);
    }

    private void CreateTwoFingerPanGesture()
    {
        twoFingerPanGesture = new PanGestureRecognizer();
        twoFingerPanGesture.MinimumNumberOfTouchesToTrack = 2;
        twoFingerPanGesture.MaximumNumberOfTouchesToTrack = 2;
        twoFingerPanGesture.ThresholdUnits = 0.01f;
        twoFingerPanGesture.StateUpdated += OnTwoFingerPanGesture;
        FingersScript.Instance.AddGesture(twoFingerPanGesture);
    }

    private void CreateScaleGesture()
    {
        scaleGesture = new ScaleGestureRecognizer();
        scaleGesture.ZoomSpeed = 1.5f;
        scaleGesture.StateUpdated += OnScaleGesture;
        FingersScript.Instance.AddGesture(scaleGesture);
    }

    #endregion

    #region 手势回调

    private void OnTapGesture(GestureRecognizer gesture)
    {
        if (gesture.State == GestureRecognizerState.Ended)
        {
            // 点击时停止惯性
            isRotationInertia = false;
            RaycastFromInput(new Vector2(gesture.FocusX, gesture.FocusY));
        }
    }

    private void OnOneFingerPanGesture(GestureRecognizer gesture)
    {
        Transform body = GetBodyTransform();
        if (body == null) return;

        // 如果当前有多指触摸，单指手势不处理旋转（让双指平移/缩放优先）
        if (Input.touchCount >= 2)
        {
            // 多指时重置状态，避免抬指后残留速度导致旋转
            lastDelta = Vector2.zero;
            rotationVelocity = Vector2.zero;
            return;
        }

        if (gesture.State == GestureRecognizerState.Began)
        {
            // 新一次滑动开始，重置轴锁定
            axisLocked = false;
            accumulatedDelta = Vector2.zero;
            lastDelta = Vector2.zero;
            isRotationInertia = false;
        }
        else if (gesture.State == GestureRecognizerState.Executing)
        {
            // 双重保险：Executing 期间如果多指也跳过
            if (Input.touchCount >= 2)
            {
                lastDelta = Vector2.zero;
                rotationVelocity = Vector2.zero;
                return;
            }

            float deltaX = oneFingerPanGesture.DeltaX;
            float deltaY = oneFingerPanGesture.DeltaY;

            // 平滑处理（不锁定轴，X和Y同时生效，支持斜向旋转）
            smoothedDelta.x = Mathf.Lerp(lastDelta.x, deltaX, 1f - velocitySmoothing);
            smoothedDelta.y = Mathf.Lerp(lastDelta.y, deltaY, 1f - velocitySmoothing);
            lastDelta = smoothedDelta;

            ApplyRotation(smoothedDelta.x * rotationSpeed, smoothedDelta.y * rotationSpeed);

            // 记录速度用于惯性
            rotationVelocity.x = smoothedDelta.x * rotationSpeed;
            rotationVelocity.y = smoothedDelta.y * rotationSpeed;
        }
        else if (gesture.State == GestureRecognizerState.Ended)
        {
            // 松手时如果速度够大，启动惯性
            if (rotationVelocity.magnitude > inertiaThreshold)
            {
                isRotationInertia = true;
            }
            lastDelta = Vector2.zero;
        }
    }

    private void OnTwoFingerPanGesture(GestureRecognizer gesture)
    {
        if (gesture.State == GestureRecognizerState.Executing)
        {
            Transform body = GetBodyTransform();
            if (body == null) return;

            // 根据屏幕尺寸归一化，保证不同分辨率体验一致
            float deltaX = twoFingerPanGesture.DeltaX * panSpeed;
            float deltaY = twoFingerPanGesture.DeltaY * panSpeed;

            Vector3 pos = body.position;
            pos.x += deltaX;
            pos.y += deltaY;
            body.position = pos;
        }
    }

    private void OnScaleGesture(GestureRecognizer gesture)
    {
        if (gesture.State == GestureRecognizerState.Executing)
        {
            Transform body = GetBodyTransform();
            if (body == null) return;

            // 使用 Lerp 平滑缩放，避免跳变
            float targetScale = body.localScale.x * scaleGesture.ScaleMultiplier;
            targetScale = Mathf.Clamp(targetScale, scaleMin, scaleMax);

            float smoothScale = Mathf.Lerp(body.localScale.x, targetScale, 0.5f);
            body.localScale = Vector3.one * smoothScale;
        }
    }

    #endregion

    #region 核心逻辑

    // 累计旋转角度
    private float rotationYaw = 0f;    // 左右（绕世界Y轴）
    private float rotationPitch = 0f;  // 上下（绕本地X轴）
    private Quaternion initialRotation = Quaternion.identity;

    private void ApplyRotation(float deltaX, float deltaY)
    {
        Transform body = GetBodyTransform();
        if (body == null) return;

        // 累加角度
        rotationYaw += -deltaX;
        rotationPitch += deltaY;

        // 限制上下旋转范围，防止翻转
        rotationPitch = Mathf.Clamp(rotationPitch, -90f, 90f);

        // 上下旋转绕世界X轴（固定水平轴），左右旋转绕模型自身Y轴
        // 先应用世界X轴的Pitch，再应用本地Y轴的Yaw
        Quaternion pitchRotation = Quaternion.AngleAxis(rotationPitch, Vector3.right);
        Quaternion yawRotation = Quaternion.AngleAxis(rotationYaw, Vector3.up);
        body.rotation = pitchRotation * initialRotation * yawRotation;
    }

    /// <summary>
    /// 重置旋转（复位时调用）
    /// </summary>
    public void ResetRotation()
    {
        rotationYaw = 0f;
        rotationPitch = 0f;
        Transform body = GetBodyTransform();
        if (body != null)
        {
            body.rotation = initialRotation;
        }
    }

    /// <summary>
    /// 同步当前模型旋转到内部状态（模型加载后调用）
    /// </summary>
    public void SyncRotationFromBody()
    {
        Transform body = GetBodyTransform();
        if (body != null)
        {
            initialRotation = body.rotation;
            rotationYaw = 0f;
            rotationPitch = 0f;
        }
    }

    private void UpdateInertia(float deltaTime)
    {
        if (!isRotationInertia) return;

        // 指数衰减
        rotationVelocity = Vector2.Lerp(rotationVelocity, Vector2.zero, inertiaDecay * deltaTime);

        if (rotationVelocity.magnitude < inertiaThreshold * 0.1f)
        {
            isRotationInertia = false;
            rotationVelocity = Vector2.zero;
            return;
        }

        ApplyRotation(rotationVelocity.x, rotationVelocity.y);
    }

    private Transform GetBodyTransform()
    {
        GameObject body = GameObjectManager.Instance.Body;
        if (body == null) return null;
        return body.transform;
    }

    #endregion

    #region 射线检测

    private void RaycastFromInput(Vector2 inputPosition)
    {
        Ray ray = UIManager.Instance.ModelCamera.ScreenPointToRay(inputPosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, targetLayer))
        {
            GameObject hitObj = hit.collider.gameObject;
            if (!hitObj.activeInHierarchy) return;

            MeshRenderer mr = hitObj.GetComponent<MeshRenderer>();
            if (mr == null || !mr.enabled) return;

            int boneId;
            if (int.TryParse(hitObj.name, out boneId))
            {
                BoneMod.Instance.CurrentBoneId = boneId;
                UserMod.Instance.Muscleid = boneId;
                EventManager.Instance.TriggerEvent(EventDefine.BoneClickEvent, boneId);

                // 打印选中骨骼的详细信息
                if (BoneMod.Instance.boneDic.ContainsKey(boneId))
                {
                    Bone bone = BoneMod.Instance.boneDic[boneId];
                    string typeCn = ((EnumBone)bone.Boneenum) switch
                    {
                        EnumBone.Bone => "骨骼",
                        EnumBone.Muscle => "肌肉",
                        EnumBone.Fascia => "筋膜",
                        _ => "未知"
                    };
                    string posCn = GetPosChinese(bone.Pos);
                    Debug.Log($"[选中骨骼] ID: {boneId}, 类型: {typeCn}({(EnumBone)bone.Boneenum}), 部位: {posCn}({(EnumPos)bone.Pos})");
                }
            }
        }
    }

    #endregion

    #region 辅助方法

    private string GetPosChinese(int pos)
    {
        var parts = new System.Collections.Generic.List<string>();
        if ((pos & (int)EnumPos.UpperLimbs) != 0) parts.Add("上肢");
        if ((pos & (int)EnumPos.ShoulderBack) != 0) parts.Add("肩背");
        if ((pos & (int)EnumPos.LowerLimbs) != 0) parts.Add("下肢");
        if ((pos & (int)EnumPos.Pelvis) != 0) parts.Add("盆骨");
        if ((pos & (int)EnumPos.HeadAndNeck) != 0) parts.Add("头颈");
        if ((pos & (int)EnumPos.ChestAndAbdomen) != 0) parts.Add("胸腹");
        if ((pos & (int)EnumPos.Spine) != 0) parts.Add("脊柱");
        if ((pos & (int)EnumPos.Scapula) != 0) parts.Add("肩胛带");
        return parts.Count > 0 ? string.Join(",", parts) : "无";
    }

    #endregion

    public override void Update(float time)
    {
        base.Update(time);
        UpdateInertia(time);
    }

    public override void Dispose()
    {
        base.Dispose();
    }

    public override void OnApplicationFocus(bool hasFocus)
    {
        base.OnApplicationFocus(hasFocus);
    }

    public override void OnApplicationPause(bool pauseStatus)
    {
        base.OnApplicationPause(pauseStatus);
    }

    public override void OnApplicationQuit()
    {
        base.OnApplicationQuit();
    }
}
