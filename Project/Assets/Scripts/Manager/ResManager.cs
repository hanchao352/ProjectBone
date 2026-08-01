using System;
using System.Collections.Generic;
using UnityEngine;
using Stopwatch = System.Diagnostics.Stopwatch;

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
                Stopwatch stageTimer = Stopwatch.StartNew();
                T go = Resources.Load<T>(resname);
                StartupTimingLogger.MarkDuration(
                        "resources_load_complete", stageTimer,
                        $"resource={resname}|type={typeof(T).Name}|success={go != null}");
                if (!go)
                {
                        Debug.LogError($"[ResManager] Resources.Load 失败: {resname} (type={typeof(T).Name}) 返回 null");
                        return null;
                }

                stageTimer.Restart();
                T obj = UnityEngine.Object.Instantiate<T>(go);
                StartupTimingLogger.MarkDuration(
                        "resource_instantiate_complete", stageTimer,
                        $"resource={resname}|type={typeof(T).Name}|success={obj != null}");
                return obj;
        }
}
