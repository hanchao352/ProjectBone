using System;
using System.Collections.Generic;
using UnityEngine;

public class ResManager:SingletonManager<ResManager>,IGeneric
{
        Dictionary<string,AssetBundle> _assetBundles = new Dictionary<string, AssetBundle>();
        public override void Initialize()
        {
                base.Initialize();
        }

        public override void Update(float time)
        {
                base.Update(time);
        }
        
        public override void Dispose()
        {
              
        }
        

        public T LoadRes<T>(string resname) where T : UnityEngine.Object
        {
               

              T  go = Resources.Load<T>(resname);
             T obj =   GameObject.Instantiate<T>(go);
                
                return obj;
        }
}
