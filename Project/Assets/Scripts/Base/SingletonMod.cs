using System;


    public abstract class SingletonMod<T> : ModBase where T : SingletonMod<T>, new()
    {
        private static readonly Lazy<T> _instance = new Lazy<T>(() => new T());

        public static T Instance
        {
            get { return _instance.Value; }
        }
    }
