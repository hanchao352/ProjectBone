
    using System.Collections.Generic;
    using UnityEngine;

    public class ComponentBase:IViewGeneric
    {
        public GameObject Root { get; set; }
        public List<ComponentBase> childComponents { get; set; }
        
        public bool Visible
        {
            get { return visible; }
            set
            {
                if (visible == true)
                {
                    if (value == true)
                    {
                        UpdateView();
                    }
                    else
                    {
                        OnHide();
                    }
                }
                else
                {
                    if (value == true)
                    {
                        OnShow();
                    }
                    else
                    {
                        OnHide();
                    }
                }
                visible = value;
                Root.SetActive(visible);
            }
        }

        private bool visible = false;
      

        public virtual void Initialize()
        {
            childComponents = new List<ComponentBase>();
            Visible = false;
        }

        public virtual void OnShow(params object[] args)
        {
            
        }

        public virtual void UpdateView(params object[] args)
        {
            for (int i = 0; i < childComponents.Count; i++)
            {
                childComponents[i].UpdateView();
            }
        }

        public virtual void Update(float time)
        {
            for (int i = 0; i < childComponents.Count; i++)
            {
                childComponents[i].Update(time);
            }
        }

        public virtual void OnApplicationFocus(bool hasFocus)
        {
          
        }

        public virtual void OnApplicationPause(bool pauseStatus)
        {
            
        }

        public virtual void OnApplicationQuit()
        {
            
        }

        public virtual void OnHide()
        {
            for (int i = 0; i < childComponents.Count; i++)
            {
                childComponents[i].OnHide();
            }
        }

        public virtual void OnEnterMutex()
        {
            for (int i = 0; i < childComponents.Count; i++)
            {
                childComponents[i].OnEnterMutex();
            }
        }

        public virtual void OnExitMutex()
        {
            for (int i = 0; i < childComponents.Count; i++)
            {
                childComponents[i].OnExitMutex();
            }
        }

        public virtual void Dispose()
        {
            for (int i = 0; i < childComponents.Count; i++)
            {
                childComponents[i].Dispose();
            }
        }
        
        public virtual  T MakeComponent<T>(GameObject comroot) where T:ComponentBase
        {
            T com = comroot.MakeComponent<T>();
            if (childComponents.Contains(com)==false)
            {
                childComponents.Add(com);
            }
            return com;
        }
    }
