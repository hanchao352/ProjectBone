using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[InitializeOnLoad]
public class SelectGameObjectInGameView 
{
    public static LayerMask targetLayer;
    private static void OnHierarchyChanged()
    {
        if (Selection.activeGameObject != null)
        {
            // 遍历选中对象的所有父对象并展开
            Transform currentSelection = Selection.activeTransform;
            // while (currentSelection.parent != null)
            // {
            //     // 在Hierarchy视图中展开父对象
            //     EditorGUIUtility.PingObject(currentSelection.parent.gameObject);
            //     currentSelection = currentSelection.parent;
            // }
        }
    }
    
    static SelectGameObjectInGameView()
    {
        targetLayer = 1 << UnityLayer.Layer_Body;
      EditorApplication.update += Update;
      EditorApplication.hierarchyChanged += OnHierarchyChanged;
    }
   static void RaycastFromInput(Vector2 inputPosition)
    {
       
        // 将输入位置（鼠标位置或触摸位置）转换为从相机到屏幕的射线
        Ray ray = UIManager.Instance.ModelCamera.ScreenPointToRay(inputPosition);
        RaycastHit hit;

        // 发射射线，最大距离设置为Mathf.Infinity表示射线长度无限
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, targetLayer))
        {
            // 如果射线与指定层上的对象相交，输出该对象的名称
            Selection.activeGameObject = hit.collider.gameObject;

            
          
        }
    }
    private static void Update()
    {
        if ( Input.GetKey(KeyCode.LeftAlt) && Input.GetMouseButtonDown(0))
        {
            RaycastFromInput(Input.mousePosition);
        }
        
      
        
    }

}
