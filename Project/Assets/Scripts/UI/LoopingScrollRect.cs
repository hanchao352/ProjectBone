using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoopingScrollRect : MonoBehaviour
{
    public GameObject itemPrefab;
    public int itemCount;
    public float itemHeight;
    public int poolSize;

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
            itemPool.Enqueue(item);
        }
    }

    private void UpdateItem(GameObject item, int dataIndex)
    {
        // 更新 item 的内容，例如：item.GetComponent<Text>().text = "Item " + dataIndex;
        item.GetComponent<TMPro.TextMeshProUGUI>().text = "Item " + dataIndex;
    }
}