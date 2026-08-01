using System;
using System.IO;
using System.Text;
using System.Threading;
using UnityEngine;

/// <summary>
/// 文件日志记录器 - 将所有 Debug.Log 输出写入设备本地文件。
/// 日志文件路径: Application.persistentDataPath/unity_log.txt
/// Android 路径通常为: /storage/emulated/0/Android/data/{包名}/files/unity_log.txt
/// </summary>
public class FileLogger : MonoBehaviour
{
    private static string _logFilePath;
    private static StreamWriter _writer;

    private void Awake()
    {
        _logFilePath = Path.Combine(Application.persistentDataPath, "unity_log.txt");

        try
        {
            // 每次启动清空旧日志
            _writer = new StreamWriter(_logFilePath, false);
            _writer.AutoFlush = true;
            _writer.WriteLine($"=== Unity Log Started: {DateTime.Now:yyyy-MM-dd HH:mm:ss} ===");
            _writer.WriteLine($"Platform: {Application.platform}");
            _writer.WriteLine($"Device: {SystemInfo.deviceModel}");
            _writer.WriteLine($"PersistentDataPath: {Application.persistentDataPath}");
            _writer.WriteLine("========================================");
        }
        catch (Exception e)
        {
            Debug.LogError($"[FileLogger] 无法创建日志文件: {e.Message}");
        }
    }

    private void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    private void HandleLog(string message, string stackTrace, LogType type)
    {
        if (_writer == null) return;

        try
        {
            string timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
            string prefix = type switch
            {
                LogType.Error => "ERROR",
                LogType.Exception => "EXCEPTION",
                LogType.Warning => "WARN",
                _ => "INFO"
            };

            _writer.WriteLine($"[{timestamp}] [{prefix}] {message}");

            if (type == LogType.Error || type == LogType.Exception)
            {
                _writer.WriteLine($"  StackTrace: {stackTrace}");
            }
        }
        catch
        {
            // 写入失败时静默忽略，避免递归
        }
    }

    private void OnDestroy()
    {
        if (_writer != null)
        {
            _writer.WriteLine($"=== Unity Log Ended: {DateTime.Now:yyyy-MM-dd HH:mm:ss} ===");
            _writer.Close();
            _writer = null;
        }
    }
}

/// <summary>
/// 启动性能测量日志。只记录关键时间点，不改变原有初始化流程。
/// 文件位于 Application.persistentDataPath/startup_timing.log。
/// </summary>
public static class StartupTimingLogger
{
    private static readonly object SyncRoot = new object();
    private static StreamWriter _writer;
    private static double _lastRealtimeMilliseconds;

    public static string LogFilePath { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
    private static void InitializeBeforeSplashScreen()
    {
        Mark("unity_before_splash_screen");
    }

    public static void Mark(string stage, string detail = null)
    {
        try
        {
            lock (SyncRoot)
            {
                EnsureInitialized();

                double realtimeMilliseconds = Time.realtimeSinceStartupAsDouble * 1000.0;
                double deltaMilliseconds = realtimeMilliseconds - _lastRealtimeMilliseconds;
                _lastRealtimeMilliseconds = realtimeMilliseconds;

                string safeDetail = string.IsNullOrEmpty(detail)
                    ? ""
                    : detail.Replace('\r', ' ').Replace('\n', ' ');

                _writer.WriteLine(
                    $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}|" +
                    $"realtime_ms={realtimeMilliseconds:F2}|" +
                    $"delta_ms={deltaMilliseconds:F2}|" +
                    $"frame={Time.frameCount}|" +
                    $"thread={Thread.CurrentThread.ManagedThreadId}|" +
                    $"managed_mb={GC.GetTotalMemory(false) / 1048576.0:F2}|" +
                    $"stage={stage}|{safeDetail}");
            }
        }
        catch
        {
            // 性能日志失败不能影响应用启动。
        }
    }

    public static void MarkDuration(string stage, System.Diagnostics.Stopwatch stopwatch, string detail = null)
    {
        string duration = $"duration_ms={stopwatch.Elapsed.TotalMilliseconds:F2}";
        Mark(stage, string.IsNullOrEmpty(detail) ? duration : $"{duration}|{detail}");
    }

    private static void EnsureInitialized()
    {
        if (_writer != null)
        {
            return;
        }

        LogFilePath = Path.Combine(Application.persistentDataPath, "startup_timing.log");
        _writer = new StreamWriter(LogFilePath, false, new UTF8Encoding(false), 4096)
        {
            // 启动中途崩溃时也尽量保留已记录的阶段。
            AutoFlush = true
        };

        _lastRealtimeMilliseconds = Time.realtimeSinceStartupAsDouble * 1000.0;
        _writer.WriteLine("Startup Timing Log v1");
        _writer.WriteLine(
            $"package={Application.identifier}|unity={Application.unityVersion}|" +
            $"platform={Application.platform}|device={SystemInfo.deviceModel}|" +
            $"system_memory_mb={SystemInfo.systemMemorySize}|path={LogFilePath}");
        _writer.WriteLine(
            "timestamp|realtime_ms|delta_ms|frame|thread|managed_mb|stage|detail");
    }
}
