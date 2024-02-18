using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class ContentSizeFitterExtensions
{
    public static void AddListener(this ContentSizeFitter csf,System.Action<Vector2> change)
    {
        ContentSizeFitterEvent csfe = csf.gameObject.GetComponent<ContentSizeFitterEvent>() ?? csf.gameObject.AddComponent<ContentSizeFitterEvent>();
        csfe.change += change;
    }
    
    public static void RemoveListener(this ContentSizeFitter csf,System.Action<Vector2> change)
    {
        ContentSizeFitterEvent csfe = csf.gameObject.GetComponent<ContentSizeFitterEvent>();
        if (csfe != null)
        {       
            csfe.change -= change;
        }
    }
    
    [ExecuteInEditMode]
    public class ContentSizeFitterEvent:UIBehaviour
    {
        public System.Action<Vector2> change = null;

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            change?.Invoke((transform as RectTransform).sizeDelta);
        }
    }
    
}
