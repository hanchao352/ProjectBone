using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

public class ToolsWindow : OdinMenuEditorWindow
{
    private SVNMergeWindow SvnMergeWindow = new SVNMergeWindow();
    private OdinMenuTree odintree;
    [MenuItem("工具/小工具")]
    public static void ShowToolsWindow()
    {
        SVNManager.Instance().Init();
        ToolsWindow ToolsWindow = GetWindow<ToolsWindow>();
        ToolsWindow.name = "工具";
    }

    protected override OdinMenuTree BuildMenuTree()
    {
        OdinMenuStyle odinMenuStyle = new OdinMenuStyle();
        odinMenuStyle.SetBorders(true);
        odinMenuStyle.SetSelectedColorDarkSkin(Color.blue);
        OdinMenuTree tree = new OdinMenuTree(true,odinMenuStyle);
        tree.Add("SVN/合并工具",SvnMergeWindow);
        tree.Selection.SelectionChanged += OnSelcectChange;
        odintree = tree;
        return tree;
    }

    private void OnSelcectChange(SelectionChangedType obj)
    {
        if (obj==SelectionChangedType.SelectionCleared)
        {
            if (odintree==null)
                return;
            for (int i = 0; i < odintree.MenuItems.Count; i++)
            {
                odintree.MenuItems[i].Toggled = false;
            }
            
        }
        else if (obj==SelectionChangedType.ItemRemoved)
        {
            if (odintree==null)
                return;
            for (int i = 0; i < odintree.MenuItems.Count; i++)
            {
                odintree.MenuItems[i].Toggled = false;
            }
        }
        else if (obj==SelectionChangedType.ItemAdded)
        {
            if (odintree==null)
                return;
            if (odintree.Selection!=null)
            {
                odintree.Selection[0].Toggled = true;
            }
        }
    }
}
