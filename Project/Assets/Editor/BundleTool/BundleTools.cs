using System.Collections;
using System.Collections.Generic;
using System.IO;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

public class BundleTools : OdinEditorWindow
{
    [MenuItem("Tools/BundleTools")]
    private static void OpenWindow()
    {
        GetWindow<BundleTools>().Show();
    }
   
    [Button("BuildAndroid")]
    private void BuildAndroid()
    {
        BuildAB(BuildTarget.Android);
    }
    [Button("BuildIOS")]
    private void BuildIOS()
    {
        
        BuildAB(BuildTarget.iOS);
    }
    [Button("BuildWindows")]
    
    private void BuildWindows()
    {
        BuildAB(BuildTarget.StandaloneWindows);
    }
    
    
    [Button("拖拽")]
    private void Drag()
    {
        GameObjectManager.Instance.rotateenable = false;
        GameObjectManager.Instance.dragenable = true;
    }
    [Button("旋转")]
    private void Rotate()
    {
        GameObjectManager.Instance.rotateenable = true;
        GameObjectManager.Instance.dragenable = false;
    }
    private void BuildAB(BuildTarget buildTarget)
    {
        
        
        string assetBundleDirectory = "Assets/AssetBundles";
        string specificDirectory = "Assets/Res"; // 替换为你的指定目录
        if (!Directory.Exists(assetBundleDirectory))
        {
            Directory.CreateDirectory(assetBundleDirectory);
        }

        DirectoryInfo dir = new DirectoryInfo(specificDirectory);
        FileInfo[] files = dir.GetFiles("*", SearchOption.AllDirectories);

        foreach (FileInfo file in files)
        {
            if (!file.FullName.EndsWith(".meta"))
            {
                string relativePath = "Assets" + file.FullName.Substring(Application.dataPath.Length);
                var importer = AssetImporter.GetAtPath(relativePath);
                if (importer != null)
                {
                    string assetBundleName = Path.GetFileNameWithoutExtension(relativePath).ToLower();
                    importer.assetBundleName = assetBundleName;
                }
            }
        }

        BuildPipeline.BuildAssetBundles(assetBundleDirectory, 
            BuildAssetBundleOptions.None, 
            buildTarget); // 替换为目标平台
       
        AssetDatabase.Refresh();
        
       
    }
}
