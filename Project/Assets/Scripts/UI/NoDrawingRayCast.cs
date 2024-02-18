using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasRenderer))]
public class NoDrawingRayCast : Graphic,ICanvasRaycastFilter
{
    private PolygonCollider2D _polygonCollider2D;

    protected override void Start()
    {
        base.Start();
        _polygonCollider2D = GetComponent<PolygonCollider2D>();
    }    
    public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
    {
        if (_polygonCollider2D)
        {
            Vector3 pos;
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform,sp,eventCamera,out pos))
            {
                return _polygonCollider2D.OverlapPoint(pos);
            }
            else
            {
                return false;
            }
        }
        return true;
    }
    
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
    }
}
