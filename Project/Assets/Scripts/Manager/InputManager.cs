using System;
using DigitalRubyShared;
using UnityEngine;

public class InputManager:SingletonManager<InputManager>, IGeneric
{
    private PanGestureRecognizer panGesture;
    private ScaleGestureRecognizer scaleGesture;
    private OneTouchRotateGestureRecognizer rotateGesture;
    public override void Initialize()
    {
        base.Initialize();
        CreatePanGesture();
        CreateScaleGesture();
        CreateRotateGesture();
         panGesture.AllowSimultaneousExecution(scaleGesture);
        scaleGesture.AllowSimultaneousExecution(panGesture);
      
    }
    //创建缩放手势
    private void CreateScaleGesture()
    {
        scaleGesture = new ScaleGestureRecognizer();
        scaleGesture.StateUpdated += ScaleGestureCallback;
        FingersScript.Instance.AddGesture(scaleGesture);
    }
    
    private void ScaleGestureCallback(DigitalRubyShared.GestureRecognizer gesture)
    {
        if (gesture.State == GestureRecognizerState.Executing)
        {
           
            GameObjectManager.Instance.Body.transform.localScale *= scaleGesture.ScaleMultiplier;
         
        }
    }
    
    //创建旋转手势
    private void CreateRotateGesture()
    {
        
        
        rotateGesture = new OneTouchRotateGestureRecognizer();
        rotateGesture.StateUpdated += RotateGestureCallback;
        FingersScript.Instance.AddGesture(rotateGesture);
    }
    private void RotateGestureCallback(DigitalRubyShared.GestureRecognizer gesture)
    {
        if (gesture.State == GestureRecognizerState.Executing )
        {
        
         float rotationSpeed = 0.1f; // 根据需要调整这个值
         GameObjectManager.Instance.Body.transform.Rotate(Vector3.up, -panGesture.DeltaX * rotationSpeed, Space.World);
          GameObjectManager.Instance.Body.transform.Rotate(Vector3.right, panGesture.DeltaY * rotationSpeed, Space.World);
        }
       
    }
    //创建拖拽手势
    private void CreatePanGesture()
    {
        panGesture = new PanGestureRecognizer();
        panGesture.MinimumNumberOfTouchesToTrack = 2;
        panGesture.StateUpdated += PanGestureCallback;
        FingersScript.Instance.AddGesture(panGesture);
    }
    
    private void PanGestureCallback(DigitalRubyShared.GestureRecognizer gesture)
    {
        if (gesture.State == GestureRecognizerState.Executing)
        {
       
           
            float deltaX = panGesture.DeltaX / 1000.0f;
            float deltaY = panGesture.DeltaY / 1000.0f;
            
            Vector3 pos = GameObjectManager.Instance.Body.transform.position;
            pos.x += deltaX;
            pos.y += deltaY;
            GameObjectManager.Instance.Body.transform.position = pos;
        }
    }
  
  



    public override void Update(float time)
    {
        base.Update(time);
    }

    public override void OnApplicationFocus(bool hasFocus)
    {
        base.OnApplicationFocus(hasFocus);
    }

    public override void OnApplicationPause(bool pauseStatus)
    {
        base.OnApplicationPause(pauseStatus);
    }

    public override void OnApplicationQuit()
    {
        base.OnApplicationQuit();
    }

    public override void Dispose()
    {
        base.Dispose();
    }
}
