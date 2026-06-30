using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace DarkTool.BuildTools
{
    /// <summary>
    /// Android 全量导出工具。
    /// 用 BuildOptions.CleanBuildCache 在导出前强制清空增量构建缓存，
    /// 避免 Unity 走"仅脚本(script only)"增量导出、导致 data.unity3d(模型/网格等资源)沿用旧缓存。
    /// 复用 Build Settings 里已配置的导出目录与启用场景，不硬编码路径。
    /// </summary>
    public static class AndroidCleanExporter
    {
        [MenuItem("DarkTool/Android 全量导出(Clean)")]
        public static void ExportAndroidClean()
        {
            string location = EditorUserBuildSettings.GetBuildLocation(BuildTarget.Android);
            if (string.IsNullOrEmpty(location))
            {
                location = EditorUtility.SaveFolderPanel("选择 unityLib 导出目录", "", "");
                if (string.IsNullOrEmpty(location))
                {
                    Debug.LogWarning("[AndroidCleanExporter] 已取消：未选择导出目录");
                    return;
                }
            }

            // 导出为 Gradle 工程(unityLibrary)，而非直接出 APK
            EditorUserBuildSettings.exportAsGoogleAndroidProject = true;

            BuildPlayerOptions options = new BuildPlayerOptions
            {
                scenes = GetEnabledScenes(),
                locationPathName = location,
                target = BuildTarget.Android,
                targetGroup = BuildTargetGroup.Android,
                // CleanBuildCache: 清空增量缓存，强制全量重建(含 data.unity3d)
                // AcceptExternalModificationsToPlayer: 导出工程模式(配合 exportAsGoogleAndroidProject)
                options = BuildOptions.CleanBuildCache | BuildOptions.AcceptExternalModificationsToPlayer
            };

            Debug.Log($"[AndroidCleanExporter] 开始全量导出(CleanBuildCache) -> {location}");

            BuildReport report = BuildPipeline.BuildPlayer(options);
            BuildSummary summary = report.summary;

            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log($"[AndroidCleanExporter] 导出成功: 总大小={summary.totalSize} bytes, 耗时={summary.totalTime}, 输出={summary.outputPath}");
            }
            else
            {
                Debug.LogError($"[AndroidCleanExporter] 导出失败: result={summary.result}, 错误数={summary.totalErrors}");
            }
        }

        /// <summary>
        /// 取 Build Settings 中已启用的场景路径(零 LINQ，两次遍历先计数再填充)。
        /// </summary>
        private static string[] GetEnabledScenes()
        {
            EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;

            int count = 0;
            for (int i = 0; i < scenes.Length; i++)
            {
                if (scenes[i].enabled)
                {
                    count++;
                }
            }

            string[] result = new string[count];
            int index = 0;
            for (int i = 0; i < scenes.Length; i++)
            {
                if (scenes[i].enabled)
                {
                    result[index] = scenes[i].path;
                    index++;
                }
            }

            return result;
        }
    }
}
