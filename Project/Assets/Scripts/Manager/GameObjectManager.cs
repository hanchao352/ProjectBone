using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameObjectManager:SingletonManager<GameObjectManager>, IGeneric
{
        public GameObject Body;
        public bool dragenable = false;
        public bool rotateenable = false;
        public List<SkeletonInfo> skeletonInfos;
        private bool bodyVisible = false;
        private Color NormalColor = new Color(0.7353569f, 0.7353569f, 0.7353569f, 1f);
        private Color SelectColor = Color.cyan;
        private int boneShowType =(int) BoneShowType.All;
        private int selectBoneType = (int)EnumPos.All;
        //初始坐标位置
        private Vector3 initpos = new Vector3(0, 0, 0);
        //初始大小
        private Vector3 initscale = new Vector3(1, 1, 1);
        //初始角度
        private Vector3 initangle = new Vector3(0, 0, 0);
        
        public bool BodyVisible
        {
                get
                {
                        return bodyVisible
                                ;
                }
                set
                {
                       
                        if (Body!= null)
                        { 
                                bodyVisible = value;
                                // if (value == true)
                                // {
                                //         ChangeBodyLayer(UnityLayer.Layer_Body);        
                                // }
                                // else
                                // {
                                //         ChangeBodyLayer(UnityLayer.Layer_Default);
                                // }
                                Body.SetActive(value);
                        }
                        {
                                bodyVisible = false;
                        }
                }
             
        }

        public int ShowType
        {
                get { return boneShowType; }
                set
                {
                        boneShowType = value;
                        ShowBoneByType(boneShowType);
                }
        }

        public int SelectBoneType
        {
                get { return selectBoneType; }
                set
                {
                        selectBoneType = value;
                        SelectBoneByPos(selectBoneType);
                }
        }

        public override void Initialize()
        {
                base.Initialize();
                skeletonInfos = new List<SkeletonInfo>();
            
        }
        
        public override void AllManagerInitialize()
        {
                base.AllManagerInitialize();
                LoadBody();
                BodyVisible = false;
        }
        
        void ChangeBodyLayer(int layer)
        {
                if (Body != null)
                {
                        
                        for (int i = 0; i < Body.transform.childCount; i++)
                        {
                                Body.transform.GetChild(i).gameObject.layer = layer;
                        }
                }
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
                                skeletonInfo.meshRenderer = boneobj.GetComponent<MeshRenderer>();
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
                Body.transform.localScale = new Vector3(1, 1, 1);
                initpos = Body.transform.position;
                initscale = Body.transform.localScale;
                initangle = Body.transform.eulerAngles;
        }
        
        public void SelectBone(int boneid)
        {
                for (int i = 0; i < skeletonInfos.Count; i++)
                {
                        SkeletonInfo skeletonInfo = skeletonInfos[i];
                        if (skeletonInfo.boneId == boneid)
                        {
                                skeletonInfo.meshRenderer.material.color = SelectColor;
                        }
                        else
                        {
                                skeletonInfo.meshRenderer.material.color = NormalColor;
                        }
                }
        }
        public void ResetBoneColor()
        {
                for (int i = 0; i < skeletonInfos.Count; i++)
                {
                        SkeletonInfo skeletonInfo = skeletonInfos[i];
                        skeletonInfo.meshRenderer.material.color = NormalColor;
                }
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
        
        public void HideBone()
        {
                var selectedBoneIds = BoneMod.Instance.selectedBoneIds;
                for (int i = 0; i < skeletonInfos.Count; i++)
                {
                        SkeletonInfo skeletonInfo = skeletonInfos[i];
                        skeletonInfo.boneGameObject.SetActive(true);
                        for (int j = 0; j < selectedBoneIds.Count; j++)
                        {
                                if (skeletonInfo.boneId == selectedBoneIds[j])
                                {
                                        skeletonInfo.boneGameObject.SetActive(false);
                                }
                        }
                }
        }       
      public void HideOtherBone()
      {
              var boneId = BoneMod.Instance.CurrentBoneId;
              for (int i = 0; i < skeletonInfos.Count; i++)
              {
                      SkeletonInfo skeletonInfo = skeletonInfos[i];
                        skeletonInfo.boneGameObject.SetActive((skeletonInfo.boneId == boneId));
              }
      }

      public void ShowBoneByType(int type)
      {
              for (int i = 0; i < skeletonInfos.Count; i++)
              {
                      SkeletonInfo skeletonInfo = skeletonInfos[i];
                      skeletonInfo.boneGameObject.SetActive(false);
                              
              }
             
              if (UtilHelper.IsContains(type,(int)BoneShowType.All) )
              {
                      for (int i = 0; i < skeletonInfos.Count; i++)
                      {
                              SkeletonInfo skeletonInfo = skeletonInfos[i];
                              skeletonInfo.boneGameObject.SetActive(true);
                              
                      }
              }
              else
              {
                      if (UtilHelper.IsContains(type,(int)BoneShowType.Bone) )
                      {
                              for (int i = 0; i < skeletonInfos.Count; i++)
                              {
                                      SkeletonInfo skeletonInfo = skeletonInfos[i];
                                      if (skeletonInfo.bone.Boneenum == EnumBone.Bone)
                                      {
                                              skeletonInfo.boneGameObject.SetActive(true);
                                      }
                                    
                                      
                              }
                      }
                      if (UtilHelper.IsContains(type,(int)BoneShowType.Muscle))
                      {
                              for (int i = 0; i < skeletonInfos.Count; i++)
                              {
                                      SkeletonInfo skeletonInfo = skeletonInfos[i];
                                      if (skeletonInfo.bone.Boneenum == EnumBone.Muscle)
                                      {
                                              skeletonInfo.boneGameObject.SetActive(true);
                                      }
                                    
                                      
                              }
                      }
                      
                      if (UtilHelper.IsContains(type,(int)BoneShowType.Fascia))
                      {
                              for (int i = 0; i < skeletonInfos.Count; i++)
                              {
                                      SkeletonInfo skeletonInfo = skeletonInfos[i];
                                      if (skeletonInfo.bone.Boneenum == EnumBone.Fascia)
                                      {
                                              skeletonInfo.boneGameObject.SetActive(true);
                                      }
                                    
                                      
                              }
                      }
              }
             
           
      }
      

      public void SelectBoneByPos(int pos)
      {
              for (int i = 0; i < skeletonInfos.Count; i++)
              {
                      SkeletonInfo skeletonInfo = skeletonInfos[i];
                      if (skeletonInfo.bone.Id == 2243)
                      {
                              Debug.Log("骨骼所属位置:"+skeletonInfo.bone.Pos);
                      }
                      if (UtilHelper.IsContains(pos,skeletonInfo.bone.Pos)  )
                      {
                              skeletonInfo.boneGameObject.SetActive(true);
                              Debug.Log("骨骼所属位置:"+skeletonInfo.bone.Pos);
                      }
                      else
                      {
                              skeletonInfo.boneGameObject.SetActive(false);
                      }
              }
            
      }
      
      public SkeletonInfo GetSkeletonInfo(int boneid)
      {
              for (int i = 0; i < skeletonInfos.Count; i++)
              {
                      SkeletonInfo skeletonInfo = skeletonInfos[i];
                      if (skeletonInfo.boneId == boneid)
                      {
                              return skeletonInfo;
                      }
              }
              return null;
      }

      public void ReSetPos()
      {
              if (Body != null)
              {
                      Body.transform.position = initpos;
              }
              {
                      
              }
      }
      
      public void ReSetScale()
      {
              if (Body != null)
              {
                      Body.transform.localScale = initscale;
              }
              {
                      
              }
      }
      
      public void ResetAngle()
      {
              if (Body != null)
              {
                      Body.transform.eulerAngles = initangle;
              }
              {
                      
              }
      }
      
      public void ReSet()
      {
              ReSetPos();
              ReSetScale();
              ResetAngle();
      }
}
