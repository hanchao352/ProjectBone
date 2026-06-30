using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace DarkTool.Build
{
    /// <summary>
    /// Android 全量导出工具。
    /// 通过 BuildOptions.CleanBuildCache 在导出前清空增量构建缓存，强制全量重建 data.unity3d，
    /// 避免 Unity 走"仅脚本(Run script only build)"增量导出导致模型/网格等资源沿用旧缓存。
    /// 配合 AcceptExternalModificationsToPlayer + exportAsGoogleAndroidProject 导出 unityLibrary(Gradle) 工程。
    /// </summary>
    public static class AndroidCleanExporter
    {
        [MenuItem("DarkTool/Build/Android 全量导出 (Clean)", false, 0)]
        public static void ExportAndroidClean()
        {
            // 复用 Build Settings 里上次配置的导出目录，避免硬编码路径
            string location = EditorUserBuildSettings.GetBuildLocation(BuildTarget.Android);
            if (string.IsNullOrEmpty(location))
            {
                location = EditorUtility.SaveFolderPanel("选择 unityLibrary(Gradle) 导出目录", "", "");
                if (string.IsNullOrEmpty(location))
                {
                    Debug.LogWarning("[AndroidCleanExporter] 已取消：未选择导出目录");
                    return;
                }
            }

            string[] scenes = GetEnabledScenes();
            if (scenes.Length == 0)
            {
                Debug.LogError("[AndroidCleanExporter] 导出失败：Build Settings 中没有启用任何场景");
                return;
            }

            // 导出为 Gradle 工程(unityLibrary)，而不是直接出 APK
            EditorUserBuildSettings.exportAsGoogleAndroidProject = true;

            BuildPlayerOptions options = new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = location,
                target = BuildTarget.Android,
                targetGroup = BuildTargetGroup.Android,
                // CleanBuildCache: 强制全量重建(含 data.unity3d)；AcceptExternalModificationsToPlayer: 导出工程而非直接出包
                options = BuildOptions.CleanBuildCache | BuildOptions.AcceptExternalModificationsToPlayer
            };

            Debug.Log($"[AndroidCleanExporter] 开始全量导出(CleanBuildCache) -> {location}");
            BuildReport report = BuildPipeline.BuildPlayer(options);
            BuildSummary summary = report.summary;

            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log($"[AndroidCleanExporter] 导出成功：输出={summary.outputPath}, 总大小={summary.totalSize} bytes, 耗时={summary.totalTime}");
            }
            else
            {
                Debug.LogError($"[AndroidCleanExporter] 导出失败：result={summary.result}, 错误数={summary.totalErrors}");
            }
        }

        /// <summary>
        /// 获取 Build Settings 中已启用的场景路径数组（无 LINQ、按需分配）。
        /// </summary>
        private static string[] GetEnabledScenes()
        {
            EditorBuildSettingsScene[] settingsScenes = EditorBuildSettings.scenes;

            int count = 0;
            for (int i = 0; i < settingsScenes.Length; i++)
            {
                if (settingsScenes[i].enabled)
                {
                    count++;
                }
            }

            string[] result = new string[count];
            int index = 0;
            for (int i = 0; i < settingsScenes.Length; i++)
            {
                if (settingsScenes[i].enabled)
                {
                    result[index] = settingsScenes[i].path;
                    index++;
                }
            }

            return result;
        }
    }
}
