using System.Text;
using UnityEngine;

/// <summary>
/// 运行时层级结构诊断工具。将 GameObject 的 Transform 层级树打印到日志，
/// 真机(打包后)无 Hierarchy 窗口时，可借此在 unity_log.txt 中查看场景对象层级。
/// 带最大深度与每层最大广度限制，避免大模型(上千子节点)刷爆日志。
/// 整棵树拼成单条字符串后一次性输出，减少 FileLogger 的文件写入次数。
/// </summary>
public static class HierarchyDumper
{
    // 默认最大递归深度
    private const int DefaultMaxDepth = 4;

    // 默认每个节点最多打印的子节点数
    private const int DefaultMaxChildrenPerNode = 12;

    /// <summary>
    /// 打印指定根对象的层级树到日志。
    /// </summary>
    public static void Dump(GameObject root, int maxDepth = DefaultMaxDepth, int maxChildrenPerNode = DefaultMaxChildrenPerNode)
    {
        if (!root)
        {
            Debug.LogWarning("[Hierarchy] Dump 失败: root 为 null");
            return;
        }

        StringBuilder sb = new StringBuilder(2048);
        sb.Append("[Hierarchy] ===== 层级 Dump 开始: ").Append(root.name).Append(" =====\n");
        AppendNode(sb, root.transform, 0, maxDepth, maxChildrenPerNode);
        sb.Append("[Hierarchy] ===== 层级 Dump 结束 =====");
        Debug.Log(sb.ToString());
    }

    private static void AppendNode(StringBuilder sb, Transform node, int depth, int maxDepth, int maxChildrenPerNode)
    {
        for (int i = 0; i < depth; i++)
        {
            sb.Append("    ");
        }

        int childCount = node.childCount;
        sb.Append("- ").Append(node.name)
          .Append(" (active=").Append(node.gameObject.activeSelf)
          .Append(", children=").Append(childCount);

        if (node.GetComponent<MeshRenderer>())
        {
            sb.Append(", MeshRenderer");
        }
        if (node.GetComponent<MeshFilter>())
        {
            sb.Append(", MeshFilter");
        }
        if (node.GetComponent<SkinnedMeshRenderer>())
        {
            sb.Append(", SkinnedMeshRenderer");
        }
        if (node.GetComponent<MeshCollider>())
        {
            sb.Append(", MeshCollider");
        }

        sb.Append(")\n");

        if (depth >= maxDepth)
        {
            if (childCount > 0)
            {
                AppendIndent(sb, depth + 1);
                sb.Append("... 已达最大深度，省略 ").Append(childCount).Append(" 个子节点\n");
            }
            return;
        }

        int printCount = childCount < maxChildrenPerNode ? childCount : maxChildrenPerNode;
        for (int i = 0; i < printCount; i++)
        {
            AppendNode(sb, node.GetChild(i), depth + 1, maxDepth, maxChildrenPerNode);
        }

        if (childCount > printCount)
        {
            AppendIndent(sb, depth + 1);
            sb.Append("... 还有 ").Append(childCount - printCount).Append(" 个子节点未显示\n");
        }
    }

    private static void AppendIndent(StringBuilder sb, int depth)
    {
        for (int i = 0; i < depth; i++)
        {
            sb.Append("    ");
        }
    }
}
