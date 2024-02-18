using System.Collections.Generic;
using UnityEditor.Timeline;
using UnityEngine;

public class GameObjectManager:SingletonManager<GameObjectManager>, IGeneric
{
        public GameObject Body;
        public bool dragenable = false;
        public bool rotateenable = false;
        public List<SkeletonInfo> skeletonInfos;
        public override void Initialize()
        {
                base.Initialize();
                skeletonInfos = new List<SkeletonInfo>();
            
        }
        
        public override void AllManagerInitialize()
        {
                base.AllManagerInitialize();
                LoadBody();
        }
        void LoadBody()
        {
                GameObject obj = ResManager.Instance.LoadRes<GameObject>("Model/jirou_nan");
                obj.transform.position = new Vector3(0, 0, 0);
                for (int i = 0; i < obj.transform.childCount; i++)
                {
                        GameObject boneobj = obj.transform.GetChild(i).gameObject;
                        string name = boneobj.name;
                        if (int.TryParse(name,out int id))
                        {
                                SkeletonInfo skeletonInfo = new SkeletonInfo();
                                Bone  bone = new Bone();
                                bone.Id = id;
                                skeletonInfo.boneId = id;
                                skeletonInfo.bone = bone;
                                skeletonInfo.boneGameObject = boneobj;
                                if (BoneMod.Instance.boneDic.ContainsKey(id))
                                {
                                        BoneMod.Instance.boneDic[id] = bone;
                                }
                                else
                                {
                                        BoneMod.Instance.boneDic.Add(id,bone);
                                }
                                skeletonInfos.Add(skeletonInfo);
                        }
                        obj.transform.GetChild(i).gameObject.layer = UnityLayer.Layer_Body;
                        if ( obj.transform.GetChild(i).gameObject.GetComponent<MeshCollider>() == null)
                        {
                                obj.transform.GetChild(i).gameObject.AddComponent<MeshCollider>();
                        }
                      
                }
                obj.transform.position = new Vector3(0, 0, 0.5f);
                Body = obj;
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

        public override void OnApplicationQuit()
        {
                base.OnApplicationQuit();
        }

        public override void Dispose()
        {
                base.Dispose();
        }

        public override void OnApplicationFocus(bool hasFocus)
        {
                base.OnApplicationFocus(hasFocus);
        }

        public override void OnApplicationPause(bool pauseStatus)
        {
                base.OnApplicationPause(pauseStatus);
        }
        
        public void HideBone(int boneid = -1 )
        {
                if (boneid == -1)
                {
                        boneid = BoneMod.Instance.currentBoneId;
                }
                for (int i = 0; i < skeletonInfos.Count; i++)
                {
                        SkeletonInfo skeletonInfo = skeletonInfos[i];
                        if (skeletonInfo.boneId == boneid)
                        {
                                skeletonInfo.boneGameObject.SetActive(false);
                        }
                        else
                        {
                                skeletonInfo.boneGameObject.SetActive(true);
                        }
                }
        }       
      public void HideOtherBone(int boneid = -1)
      {
              if (boneid == -1)
              {
                      boneid = BoneMod.Instance.currentBoneId;
              }
              for (int i = 0; i < skeletonInfos.Count; i++)
              {
                      SkeletonInfo skeletonInfo = skeletonInfos[i];
                      if (skeletonInfo.boneId != boneid)
                      {
                              skeletonInfo.boneGameObject.SetActive(false);
                      }
                      else
                      {
                              skeletonInfo.boneGameObject.SetActive(true);
                      }
              }
      }
}
