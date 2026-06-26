using UnityEngine;
using System.IO;
using System;

/// <summary>
/// 将 Unity 日志写入文件，方便真机调试
/// </summary>
public class LogToFile : MonoBehaviour
{
    private static string logFilePath;
    private static StreamWriter writer;

    void Awake()
    {
        // 日志文件路径
        logFilePath = Path.Combine(Application.persistentDataPath, "game_log.txt");

        // 清空旧日志
        if (File.Exists(logFilePath))
        {
            File.Delete(logFilePath);
        }

        // 注册日志回调
        Application.logMessageReceived += HandleLog;

        Debug.Log($"[LogToFile] 日志文件路径: {logFilePath}");
    }

    void OnDestroy()
    {
        Application.logMessageReceived -= HandleLog;
        if (writer != null)
        {
            writer.Close();
            writer = null;
        }
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        try
        {
            if (writer == null)
            {
                writer = new StreamWriter(logFilePath, true);
                writer.AutoFlush = true;
            }

            string timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
            writer.WriteLine($"[{timestamp}] [{type}] {logString}");

            if (type == LogType.Exception || type == LogType.Error)
            {
                writer.WriteLine(stackTrace);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[LogToFile] 写入日志失败: {e.Message}");
        }
    }
}
