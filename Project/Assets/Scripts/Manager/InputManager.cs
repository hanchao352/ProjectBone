using System;
using UnityEngine;

public class InputManager:Singleton<InputManager>,IDisposable
{
    
    public event Action<Vector2> OnTap;
    public event Action<Vector2, Vector2> OnDrag;
    public event Action<float> OnRotate;
    public event Action<float> OnZoom;

    private Vector2 initialTouchPosition;
    private Vector2 lastTouchPosition;
    private bool isDragging = false;
    private float dragThreshold = 5f; // 拖拽阈值
        public override void Initialize()
        {
                base.Initialize();
        }

        public override void Update(float time)
        {
            base.Update(time);
#if UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_EDITOR
            HandleMouseInput();
#else
            HandleTouchInput();
#endif
        }

        
        private void HandleMouseInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                initialTouchPosition = Input.mousePosition;
                lastTouchPosition = Input.mousePosition;
                isDragging = false;
            }
            else if (Input.GetMouseButton(0))
            {
                if (((Vector2)Input.mousePosition - lastTouchPosition).magnitude > dragThreshold)
                {
                    if (!isDragging)
                    {
                        isDragging = true;
                    }
                    OnDrag?.Invoke(lastTouchPosition, (Vector2)Input.mousePosition);
                }
                lastTouchPosition = Input.mousePosition;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (!isDragging)
                {
                    OnTap?.Invoke(Input.mousePosition);
                }
                isDragging = false;
            }
        }

    private void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    initialTouchPosition = touch.position;
                    lastTouchPosition = touch.position;
                    break;

                case TouchPhase.Moved:
                    if (isDragging)
                    {
                        OnDrag?.Invoke(lastTouchPosition, touch.position);
                    }
                    else
                    {
                        isDragging = true;
                    }
                    lastTouchPosition = touch.position;
                    break;

                case TouchPhase.Ended:
                    if (!isDragging)
                    {
                        OnTap?.Invoke(touch.position);
                    }
                    isDragging = false;
                    break;
            }
        }

        if (Input.touchCount == 2)
        {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            Vector2 touch0PrevPos = touch0.position - touch0.deltaPosition;
            Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;

            float prevMagnitude = (touch0PrevPos - touch1PrevPos).magnitude;
            float currentMagnitude = (touch0.position - touch1.position).magnitude;

            float difference = currentMagnitude - prevMagnitude;

            OnZoom?.Invoke(difference);

            float angle = GetAngleBetweenTouches(touch0, touch1);
            OnRotate?.Invoke(angle);
        }
    }



    private float GetAngleBetweenTouches(Touch touch0, Touch touch1)
    {
        Vector2 diff = touch1.position - touch0.position;
        return Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
    }
        
        public override void Destroy()
        {
                base.Destroy();
        }
        
        void IDisposable.Dispose()
        {
                
        }
}
