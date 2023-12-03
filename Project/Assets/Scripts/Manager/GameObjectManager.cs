using UnityEngine;

public class GameObjectManager:SingletonManager<GameObjectManager>, IGeneric
{
        public GameObject Body;
        public bool dragenable = false;
        public bool rotateenable = false;
        public override void Initialize()
        {
                base.Initialize();
                InputManager.Instance.OnRotate+=OnRotate;
                InputManager.Instance.OnZoom+=OnZoom;
                InputManager.Instance.OnDrag+=OnDrag;
        }

        private void OnDrag(Vector2 lastpos, Vector2 curpos)
        {
               //拖拽模型位置
               if (Body != null && dragenable)
               {
                       Vector2 diff = curpos - lastpos;
                       Vector3 pos = Body.transform.position;  
                       pos.x += diff.x * 0.001f;
                       pos.y += diff.y * 0.001f;
                       Body.transform.position = pos;
               }
                
                
                
        }

        private void OnZoom(float obj)
        {
               //修改相机视角
                 Camera.main.fieldOfView -= obj;
                
        }

        private void OnRotate(float angle)
        {
                Debug.Log("旋转角度："+angle);
                if (Body != null && rotateenable)
                {
                        //当前角度+旋转角度
                        float obj = Body.transform.rotation.eulerAngles.y - angle;
                        Body.transform.rotation = Quaternion.Euler(0, obj, 0);
                       
                }       
        }


        public override void Update(float time)
        {
                base.Update(time);
        }
        
        public override void Dispose()
        {
                base.Dispose();
        }
}
