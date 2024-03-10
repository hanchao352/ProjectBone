using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[InitializeOnLoad]
public class SelectGameObjectInGameView
{
    static SelectGameObjectInGameView()
    {
        EditorApplication.update += Update;
    }

    private static void Update()
    {
        if (Input.GetKey(KeyCode.LeftAlt) && Input.GetMouseButtonDown(0))
        {
            RaycastFromInput(Input.mousePosition);
        }
    }

    static void RaycastFromInput(Vector2 inputPosition)
    {
        // 尝试3D射线检测
        Ray ray = Camera.main.ScreenPointToRay(inputPosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            Selection.activeGameObject = hit.collider.gameObject;
            return; // 如果在3D中找到对象，就不再继续尝试UI检测
        }

        // 尝试UI射线检测
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current)
        {
            position = inputPosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, results);

        foreach (RaycastResult result in results)
        {
            Selection.activeGameObject = result.gameObject;
            // 不再检查raycastTarget，直接选中第一个命中的UI对象
            break; // 假定我们只关心第一个匹配的对象
        }
    }
}