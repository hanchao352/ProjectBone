using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArcLoopScrollRect1 : MonoBehaviour
{
     public GameObject itemPrefab;
    public int itemCount;
    public float itemHeight;
    public int poolSize;
    public float curveFactor;

    private ScrollRect scrollRect;
    private RectTransform content;
    private Queue<GameObject> itemPool = new Queue<GameObject>();
    private void Awake()
    {
        scrollRect = GetComponent<ScrollRect>();
        content = scrollRect.content;
        scrollRect.onValueChanged.AddListener(OnScroll);
        InitializeItems();
        UpdateContentSize();
    }

    private void InitializeItems()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject item = Instantiate(itemPrefab, content);
            item.SetActive(false);
            itemPool.Enqueue(item);
        }
        UpdateItems();
        
       
    }

    private void UpdateContentSize()
    {
        float totalHeight = itemCount * itemHeight;
        content.sizeDelta = new Vector2(content.sizeDelta.x, totalHeight);
    }

    private void OnScroll(Vector2 scrollPosition)
    {
        UpdateItems();
    }

    private void UpdateItems()
    {
        float scrollPos = content.anchoredPosition.y;
        int firstItemIndex = Mathf.Clamp(Mathf.FloorToInt(scrollPos / itemHeight), 0, itemCount - 1);
        float itemOffset = firstItemIndex * itemHeight;

        content.anchoredPosition = new Vector2(content.anchoredPosition.x, scrollPos);

        int itemsInView = Mathf.CeilToInt(scrollRect.viewport.rect.height / itemHeight) + 1;
        for (int i = 0; i < itemPool.Count; i++)
        {
            GameObject item = itemPool.Dequeue();
            item.SetActive(false);
            itemPool.Enqueue(item);
        }

        float xoffset = 0;
        for (int i = firstItemIndex; i < firstItemIndex + itemsInView; i++)
        {
            if (i >= itemCount)
            {
                break;
            }
            Vector2 pos =new Vector2(0, -itemOffset - (i - firstItemIndex) * itemHeight);
            float normalizedPosition = (pos.y + scrollPos) / (scrollRect.viewport.rect.height - itemHeight);
            float curveOffset = Mathf.Sin(normalizedPosition * Mathf.PI);
            float curvedX = curveFactor * curveOffset;
           
            xoffset = Math.Max(xoffset, Math.Abs(curvedX));
        }
        for (int i = firstItemIndex; i < firstItemIndex + itemsInView; i++)
        {
            if (i >= itemCount)
            {
                break;
            }

            GameObject item = itemPool.Dequeue();
            RectTransform itemTransform = item.GetComponent<RectTransform>();
            itemTransform.anchoredPosition = new Vector2(itemTransform.anchoredPosition.x, -itemOffset - (i - firstItemIndex) * itemHeight);
            UpdateItem(item, i);
            item.SetActive(true);

            float normalizedPosition = (itemTransform.anchoredPosition.y + scrollPos) / (scrollRect.viewport.rect.height - itemHeight);
            float curveOffset = Mathf.Sin(normalizedPosition * Mathf.PI);
            float curvedX = curveFactor * curveOffset;
            item.transform.localPosition = new Vector3(curvedX+xoffset, item.transform.localPosition.y, item.transform.localPosition.z);
            item.GetComponent<RectTransform>().SetAsLastSibling(); // 把当前物体移到最后，确保它显示在最上方
            itemPool.Enqueue(item);
        }
    }
    private void UpdateItem(GameObject item, int dataIndex)
    {
        // 更新 item 的内容，例如：item.GetComponent<Text>().text = "Item " + dataIndex;
        item.GetComponent<TMPro.TextMeshProUGUI>().text = "Item " + dataIndex;
        item.name = dataIndex.ToString();
    }
}
