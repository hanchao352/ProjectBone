using System;


    public abstract class SingletonMod<T> : ModBase where T : SingletonMod<T>, new()
    {
        private static readonly Lazy<T> _instance = new Lazy<T>(OnCreate);
        
        private static T OnCreate()
        {
            T instance = new T();
            ModManager.Instance.RegisterMod(instance);
            return instance;
        }
        
        public static T Instance
        {
            get { return _instance.Value; }
        }
        
       
    }
