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
                base.Dispose();
        }
        

        public GameObject LoadRes(string resname)
        {

                AssetBundle ab = null;
                GameObject obj = null;
                if (_assetBundles.ContainsKey(resname))
                {
                         ab = _assetBundles[resname];
                }
                else
                {
                         ab = AssetBundle.LoadFromFile(Application.dataPath + "/AssetBundles/" + resname);
                        _assetBundles.Add(resname,ab);
                }
                
                GameObject go = ab.LoadAsset<GameObject>(resname);
                obj = GameObject.Instantiate(go);
                

                return obj;

        }
}
