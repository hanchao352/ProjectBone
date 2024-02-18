using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;  

public class ForceRebuild : MonoBehaviour
{
    public List<GameObject> tarList;
    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        for (int i = 0; i < tarList.Count; i++)
        {
            AddListener(tarList[i]);
        }
    }
    
    public void RebuildLayout()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
    } 
    
    public void AddListener(GameObject go)
    {
        if (go)
        {
            ContentSizeFitter csf = go.GetComponent<ContentSizeFitter>();
            if (csf)
            {
                csf.AddListener(OnValueChanged);
            }
        }
    }

    public void RemoveListener(GameObject go)
    {
        if (go)
        {
            ContentSizeFitter csf = go.GetComponent<ContentSizeFitter>();
            if (csf)
            {
                csf.RemoveListener(OnValueChanged);
            }
        }
    }

    public void OnValueChanged(Vector2 pos)
    {
        if (rectTransform)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        }
    }

    public void OnDestroy()
    {
        for (int i = 0; i < tarList.Count; i++)
        {
            RemoveListener(tarList[i]);
        }
    }
}
