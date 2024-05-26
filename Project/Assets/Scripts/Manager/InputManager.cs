using System;
using DigitalRubyShared;
using UnityEngine;

public class InputManager:SingletonManager<InputManager>, IGeneric
{
    private PanGestureRecognizer panGesture;
    private ScaleGestureRecognizer scaleGesture;
    private OneTouchRotateGestureRecognizer rotateGesture;
    public LayerMask targetLayer;
    public Vector3 scalemin = new Vector3(0.5f, 0.5f, 0.5f);
    public Vector3 scalemax = new Vector3(10f, 10f, 10f);
    public override void Initialize()
    {
        base.Initialize();
       
        targetLayer = 1 << UnityLayer.Layer_Body;
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
           
            Vector3 scale = GameObjectManager.Instance.Body.transform.localScale;
            scale *= scaleGesture.ScaleMultiplier;
            scale.x = Mathf.Clamp(scale.x, scalemin.x, scalemax.x);
            scale.y = Mathf.Clamp(scale.y, scalemin.y, scalemax.y);
            scale.z = Mathf.Clamp(scale.z, scalemin.z, scalemax.z);
            GameObjectManager.Instance.Body.transform.localScale = scale;
            //GameObjectManager.Instance.Body.transform.localScale *= scaleGesture.ScaleMultiplier;
         
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
        
         float rotationSpeed = 1f; // 根据需要调整这个值
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
       
           
            float deltaX = panGesture.DeltaX / 500.0f*-1 ;
            float deltaY = panGesture.DeltaY / 500.0f;
            
            Vector3 pos = GameObjectManager.Instance.Body.transform.position;
            pos.x -= deltaX;
            pos.y += deltaY;
            GameObjectManager.Instance.Body.transform.position = pos;
        }
    }
  
  



    public override void Update(float time)
    {
        base.Update(time);
        CheckBoneClick();
    }

    public void CheckBoneClick()
    {
        // 对于PC，使用鼠标输入
        if (Input.GetMouseButtonDown(0))
        {
            RaycastFromInput(Input.mousePosition);
        }

        // 对于移动设备，使用触摸输入
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == UnityEngine.TouchPhase.Began)
        {
            RaycastFromInput(Input.GetTouch(0).position);
        }
    }
    
    void RaycastFromInput(Vector2 inputPosition)
    {
       
        // 将输入位置（鼠标位置或触摸位置）转换为从相机到屏幕的射线
        Ray ray = UIManager.Instance.ModelCamera.ScreenPointToRay(inputPosition);
        RaycastHit hit;

        // 发射射线，最大距离设置为Mathf.Infinity表示射线长度无限
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, targetLayer))
        {
            // 如果射线与指定层上的对象相交，输出该对象的名称
            string boneName = hit.collider.gameObject.name;
            int boneId;
           // 使用 int.TryParse() 判断是不是能够转换成id
           if (int.TryParse(boneName, out  boneId))
           {
               
               BoneMod.Instance.CurrentBoneId = boneId;
               UserMod.Instance.Muscleid = boneId;
               EventManager.Instance.TriggerEvent(EventDefine.BoneClickEvent ,boneId);
           }
           else
           {
               
           }

            
          
        }
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
