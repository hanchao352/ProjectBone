using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using Newtonsoft.Json;

public class NativeAPI
{
#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
    public static extern void sendMessageToMobileApp(string message);
#endif
}


public class ButtonBehaviorCustomData // 定义可序列化数据结构 - 通讯协议外层封装
{
    public string msg;      // 实际业务数据的JSON字符串
    public int code;        // 消息类型代码
}

/// <summary>
/// 骨骼配置数据结构体（值类型） - 用于序列化和反序列化。
/// 从 class 改为 struct 以消除每个配置项的堆分配，
/// 配合预分配数组实现零GC的序列化缓冲区使用。
/// Newtonsoft.Json 原生支持 struct 的序列化/反序列化。
/// </summary>
public struct BoneData
{
    public int id;              // 骨骼ID
    public int type;            // 模型数据类型 (EnumBone: Other=0, Bone=2, Muscle=4, Fascia=8)
    public int position;        // 骨骼位置 (EnumPos: 上肢、肩背、下肢等)
    public int direction;       // 骨骼方向 (EnumDirection: None=0, Left=1, Right=2, Other=4)
}

/// <summary>
/// 骨骼点击信息类 - 用于传递骨骼点击信息（App端根据ID可获取完整数据）
/// </summary>
public class BoneClickInfo
{
    public int id;              // 骨骼ID
}

/// <summary>
/// 模型加载进度信息。progress 为 0-100 的整数百分比，status 取值为
/// loading、completed 或 failed；加载失败时 error 包含错误原因。
/// </summary>
public class ModelLoadProgressInfo
{
    public int progress;
    public string stage;
    public string status;
    public string error;
}

/// <summary>
/// App调用Unity功能的消息代码（App -> Unity）
/// </summary>
public static class AppToUnityCode
{
    public const int ShowModel = 1;                  // 显示模型
    public const int HideModel = 2;                  // 隐藏模型
    public const int ReceiveBoneConfig = 3;          // 接收骨骼配置
    public const int ExportBoneConfig = 4;           // 导出骨骼配置（请求导出）
    public const int ShowByType = 5;                 // 按类型(BoneShowType:骨骼/肌肉/筋膜)筛选，与当前部位筛选叠加(只影响当前显示的部位)
    public const int ShowByPosition = 6;             // 根据位置显示
    public const int HideBone = 10;                  // 隐藏选中的骨骼
    public const int ShowBone = 11;                  // 显示选中的骨骼
    public const int HideOtherBone = 12;             // 隐藏其他骨骼（只显示选中的）
    public const int ShowOtherBone = 13;             // 显示其他骨骼
    public const int ShowAllBone = 14;               // 显示所有骨骼
    public const int TransparentBone = 15;           // 透明选中的骨骼
    public const int SolidBone = 16;                 // 实体选中的骨骼（恢复不透明）
    public const int TransparentOtherBone = 17;      // 透明其他骨骼（只有选中的不透明）
    public const int SolidOtherBone = 18;            // 实体其他骨骼（恢复不透明）
    public const int ResetBoneColor = 19;            // 重置骨骼颜色
    public const int ResetBoneTransparency = 20;     // 重置骨骼透明度
    public const int ResetTransform = 21;            // 复位模型变换（位置、角度、大小）
    public const int ResetAll = 22;                  // 完全重置（包括选中状态）
}

/// <summary>
/// Unity主动通知App的消息代码（Unity -> App）
/// </summary>
public static class UnityToAppCode
{
    public const int BoneSelected = 1;               // 骨骼被选中（用户点击时自动发送）
    public const int ExportBoneConfigResult = 2;     // 导出骨骼配置结果（响应App请求）
    public const int BoneDeselected = 3;             // 骨骼取消选中（用户再次点击时自动发送）
    public const int ModelLoadProgress = 4;           // 模型加载进度（Unity启动加载时主动发送）
}

public class ButtonBehavior : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    /// <summary>
    /// 发送消息到移动端（底层通讯方法）
    /// </summary>
    private void SendMessageToMobile(string jsonString)
    {
        Debug.Log("---- 通信脚本触发 ----" + jsonString);
        if (Application.platform == RuntimePlatform.Android)
        {
            using (AndroidJavaClass jc = new AndroidJavaClass("com.azesmwayreactnativeunity.ReactNativeUnityViewManager"))
            {
                jc.CallStatic("sendMessageToMobileApp", jsonString);
            }
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
#if UNITY_IOS && !UNITY_EDITOR
            NativeAPI.sendMessageToMobileApp(jsonString);
#endif
        }
    }

    /// <summary>
    /// 封装并发送消息（外层包装 ButtonBehaviorCustomData）
    /// </summary>
    private void SendWrappedMessage(int code, string msg)
    {
        ButtonBehaviorCustomData customData = new ButtonBehaviorCustomData
        {
            code = code,
            msg = msg
        };
        string jsonString = JsonConvert.SerializeObject(customData);
        SendMessageToMobile(jsonString);
    }

    /// <summary>
    /// 统一消息接收入口（从移动端接收）
    /// </summary>
    public void ReceiveMessage(string jsonString)
    {
        Debug.Log($"[ReceiveMessage] 收到消息, 长度: {(jsonString != null ? jsonString.Length : 0)}, 前100字符: {(jsonString != null && jsonString.Length > 100 ? jsonString.Substring(0, 100) : jsonString)}");
        
        try
        {
            // 第一层反序列化：解析 ButtonBehaviorCustomData
            ButtonBehaviorCustomData customData = JsonConvert.DeserializeObject<ButtonBehaviorCustomData>(jsonString);
            
            if (customData == null)
            {
                Debug.LogError("反序列化 ButtonBehaviorCustomData 失败");
                return;
            }

            // 根据 code 分发消息，msg 为具体业务数据
            ProcessMessage(customData.code, customData.msg);
        }
        catch (Exception e)
        {
            Debug.LogError($"处理消息异常: {e.Message}");
        }
    }

    /// <summary>
    /// 处理具体业务消息（来自App的消息）
    /// </summary>
    private void ProcessMessage(int code, string msg)
    {
        switch (code)
        {
            case AppToUnityCode.ShowModel:
                ShowModel();
                break;
            
            case AppToUnityCode.HideModel:
                HideModel();
                break;
            
            case AppToUnityCode.ReceiveBoneConfig:
                ReceiveBoneConfigInternal(msg);
                break;
            
            case AppToUnityCode.ExportBoneConfig:
                ExportBoneConfig();
                break;
            
            case AppToUnityCode.ShowByType:
                if (int.TryParse(msg, out int type))
                {
                    ShowModelByType(type);
                }
                break;
            
            case AppToUnityCode.ShowByPosition:
                if (int.TryParse(msg, out int position))
                {
                    ShowModelByPosition(position);
                }
                break;
            
            case AppToUnityCode.HideBone:
                HideBone();
                break;
            
            case AppToUnityCode.ShowBone:
                ShowBone();
                break;
            
            case AppToUnityCode.HideOtherBone:
                HideOtherBone();
                break;
            
            case AppToUnityCode.ShowOtherBone:
                ShowOtherBone();
                break;
            
            case AppToUnityCode.ShowAllBone:
                ShowAllBone();
                break;
            
            case AppToUnityCode.ResetBoneColor:
                ResetBoneColor();
                break;
            
            case AppToUnityCode.TransparentBone:
                TransparentBone();
                break;
            
            case AppToUnityCode.SolidBone:
                SolidBone();
                break;
            
            case AppToUnityCode.TransparentOtherBone:
                TransparentOtherBone();
                break;
            
            case AppToUnityCode.SolidOtherBone:
                SolidOtherBone();
                break;
            
            case AppToUnityCode.ResetBoneTransparency:
                ResetBoneTransparency();
                break;
            
            case AppToUnityCode.ResetTransform:
                ResetTransform();
                break;
            
            case AppToUnityCode.ResetAll:
                ResetAll();
                break;
            
            default:
                Debug.LogWarning($"未知的消息类型代码: {code}");
                break;
        }
    }

    /// <summary>
    /// 兼容旧的 ButtonPressed 方法（保持向后兼容）
    /// </summary>
    public void ButtonPressed(string jsonString)
    {
        SendMessageToMobile(jsonString);
    }

    /// <summary>
    /// 显示模型
    /// </summary>
    public void ShowModel()
    {
        Debug.Log("---- 显示模型 ----");
        GameObjectManager.Instance.BodyVisible = true;
    }

    /// <summary>
    /// 隐藏模型
    /// </summary>
    public void HideModel()
    {
        Debug.Log("---- 隐藏模型 ----");
        GameObjectManager.Instance.BodyVisible = false;
    }

    /// <summary>
    /// 序列化骨骼数据列表为JSON字符串
    /// </summary>
    private string SerializeBoneDataList(List<BoneData> dataList)
    {
        return JsonConvert.SerializeObject(dataList);
    }

    /// <summary>
    /// 反序列化JSON字符串为骨骼数据列表
    /// </summary>
    private List<BoneData> DeserializeBoneDataList(string jsonString)
    {
        return JsonConvert.DeserializeObject<List<BoneData>>(jsonString);
    }

    /// <summary>
    /// 接收并应用骨骼配置数据（公开接口，兼容旧调用）
    /// </summary>
    public void ReceiveBoneConfig(string jsonString)
    {
        Debug.Log("---- 接收骨骼配置数据（兼容方法） ----" + jsonString);
        ReceiveBoneConfigInternal(jsonString);
    }

    /// <summary>
    /// 内部处理：接收并应用骨骼配置数据
    /// </summary>
    private void ReceiveBoneConfigInternal(string msg)
    {
        Debug.Log($"[BoneConfig] 开始处理骨骼配置数据, 数据长度: {(msg != null ? msg.Length : 0)}");
        try
        {
            // 第二层反序列化：从 msg 中解析具体的骨骼数据
            List<BoneData> boneDataList = DeserializeBoneDataList(msg);
            if (boneDataList != null && boneDataList.Count > 0)
            {
                Debug.Log($"[BoneConfig] 解析成功, 骨骼数量: {boneDataList.Count}");
                GameObjectManager.Instance.ApplyBoneConfig(boneDataList);
                BoneMod.Instance.boneLoaded = true;
                // 模型显示由 ApplyBoneConfig 统一负责（数据应用后激活根节点，并处理模型未就绪的时序缓存）
                Debug.Log($"[BoneConfig] 配置应用完成, Body={GameObjectManager.Instance.Body != null}");
            }
            else
            {
                Debug.LogWarning("[BoneConfig] 骨骼配置数据为空或解析为null");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[BoneConfig] 处理骨骼配置数据异常: {e.Message}\n{e.StackTrace}");
        }
    }

    /// <summary>
    /// 导出当前所有骨骼配置
    /// </summary>
    public void ExportBoneConfig()
    {
        Debug.Log("---- 导出骨骼配置 ----");
        List<BoneData> boneDataList = GameObjectManager.Instance.ExportBoneConfig();
        string msg = SerializeBoneDataList(boneDataList);
        SendWrappedMessage(UnityToAppCode.ExportBoneConfigResult, msg);
    }

    /// <summary>
    /// 按显示类型(BoneShowType)筛选模型。
    /// 注意：内部会与当前部位筛选(EnumPos)叠加取交集，即只对"当前显示的部位"做类型筛选。
    /// 例如已选中头颈(ShowByPosition)，再调用此接口只影响头颈范围内的骨骼/肌肉/筋膜，不会翻出其他部位。
    /// </summary>
    /// <param name="type">显示类型位组合 (BoneShowType: Bone=1, Muscle=2, Fascia=4, All=7)</param>
    public void ShowModelByType(int type)
    {
        Debug.Log($"---- 根据类型显示模型 ---- type: {type}");
        GameObjectManager.Instance.ShowBoneByType(type);
    }

    /// <summary>
    /// 根据骨骼位置显示模型
    /// </summary>
    /// <param name="position">骨骼位置 (EnumPos: 上肢、肩背、下肢等)</param>
    public void ShowModelByPosition(int position)
    {
        Debug.Log($"---- 根据位置显示模型 ---- position: {position}");
        GameObjectManager.Instance.SelectBoneByPos(position);
    }

    /// <summary>
    /// 通知移动端骨骼被选中
    /// </summary>
    /// <param name="boneId">被选中的骨骼ID</param>
    public void NotifyBoneSelected(int boneId)
    {
        Debug.Log($"---- 通知移动端骨骼被选中 ---- boneId: {boneId}");
        
        BoneClickInfo clickInfo = new BoneClickInfo { id = boneId };
        string boneInfoJson = JsonConvert.SerializeObject(clickInfo);
        SendWrappedMessage(UnityToAppCode.BoneSelected, boneInfoJson);
    }

    /// <summary>
    /// 通知移动端骨骼取消选中
    /// </summary>
    /// <param name="boneId">取消选中的骨骼ID</param>
    public void NotifyBoneDeselected(int boneId)
    {
        Debug.Log($"---- 通知移动端骨骼取消选中 ---- boneId: {boneId}");
        
        BoneClickInfo clickInfo = new BoneClickInfo { id = boneId };
        string boneInfoJson = JsonConvert.SerializeObject(clickInfo);
        SendWrappedMessage(UnityToAppCode.BoneDeselected, boneInfoJson);
    }

    /// <summary>
    /// 通知移动端当前模型加载进度。
    /// </summary>
    public void NotifyModelLoadProgress(ModelLoadProgressInfo progressInfo)
    {
        if (progressInfo == null)
        {
            Debug.LogWarning("[ModelLoadProgress] 忽略空的进度信息");
            return;
        }

        progressInfo.progress = Mathf.Clamp(progressInfo.progress, 0, 100);
        string progressJson = JsonConvert.SerializeObject(progressInfo);
        SendWrappedMessage(UnityToAppCode.ModelLoadProgress, progressJson);
    }

    /// <summary>
    /// 隐藏选中的骨骼
    /// </summary>
    public void HideBone()
    {
        Debug.Log("---- 隐藏选中的骨骼 ----");
        GameObjectManager.Instance.HideBone();
    }

    /// <summary>
    /// 显示选中的骨骼（恢复被隐藏的选中骨骼）
    /// </summary>
    public void ShowBone()
    {
        Debug.Log("---- 显示选中的骨骼 ----");
        GameObjectManager.Instance.ShowBone();
    }

    /// <summary>
    /// 隐藏其他骨骼（只显示选中的）
    /// </summary>
    public void HideOtherBone()
    {
        Debug.Log("---- 隐藏其他骨骼 ----");
        GameObjectManager.Instance.HideOtherBone();
    }

    /// <summary>
    /// 显示其他骨骼（恢复被隐藏的其他骨骼）
    /// </summary>
    public void ShowOtherBone()
    {
        Debug.Log("---- 显示其他骨骼 ----");
        GameObjectManager.Instance.ShowOtherBone();
    }

    /// <summary>
    /// 显示所有骨骼
    /// </summary>
    public void ShowAllBone()
    {
        Debug.Log("---- 显示所有骨骼 ----");
        GameObjectManager.Instance.ShowBoneByType((int)BoneShowType.All);
    }

    /// <summary>
    /// 重置骨骼颜色
    /// </summary>
    public void ResetBoneColor()
    {
        Debug.Log("---- 重置骨骼颜色 ----");
        GameObjectManager.Instance.ResetBoneColor();
    }

    /// <summary>
    /// 透明选中的骨骼
    /// </summary>
    public void TransparentBone()
    {
        Debug.Log("---- 透明选中的骨骼 ----");
        GameObjectManager.Instance.TransparentBone();
    }

    /// <summary>
    /// 实体选中的骨骼（恢复不透明）
    /// </summary>
    public void SolidBone()
    {
        Debug.Log("---- 实体选中的骨骼 ----");
        GameObjectManager.Instance.SolidBone();
    }

    /// <summary>
    /// 透明其他骨骼（只有选中的不透明）
    /// </summary>
    public void TransparentOtherBone()
    {
        Debug.Log("---- 透明其他骨骼 ----");
        GameObjectManager.Instance.TransparentOtherBone();
    }

    /// <summary>
    /// 实体其他骨骼（恢复不透明）
    /// </summary>
    public void SolidOtherBone()
    {
        Debug.Log("---- 实体其他骨骼 ----");
        GameObjectManager.Instance.SolidOtherBone();
    }

    /// <summary>
    /// 重置骨骼透明度（恢复所有骨骼为不透明）
    /// </summary>
    public void ResetBoneTransparency()
    {
        Debug.Log("---- 重置骨骼透明度 ----");
        GameObjectManager.Instance.ResetBoneTransparency();
    }

    /// <summary>
    /// 复位模型变换（位置、角度、大小恢复初始值）
    /// </summary>
    public void ResetTransform()
    {
        Debug.Log("---- 复位模型变换 ----");
        GameObjectManager.Instance.ResetTransform();
    }

    /// <summary>
    /// 完全重置（包括模型变换和选中状态）
    /// </summary>
    public void ResetAll()
    {
        Debug.Log("---- 完全重置 ----");
        GameObjectManager.Instance.ResetAll();
    }
}
