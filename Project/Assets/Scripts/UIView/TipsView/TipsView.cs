
    using System.Collections.Generic;
    using System.Runtime.InteropServices.WindowsRuntime;
    using FairyGUI;
    using UnityEngine;

    public class TipsView:UIBase
    {
        public RichTextComponent tipsText;
        public List<RichTextComponent> usedTips=new List<RichTextComponent>();
        public List<RichTextComponent> unusedTips=new List<RichTextComponent>();
        protected override void OnInit(params object[] args)
        {
            UIObjectFactory.SetPackageItemExtension("ui://UITipsWindow/RichTipsComponent", typeof(RichTextComponent));
            MainComponent.sortingOrder = int.MaxValue;
        }

        protected override void OnShow(params object[] args)
        {
            
        }

       

        protected override void OnHide()
        {
           
        }

        protected override void OnDestroy()
        {
            
        }

        public void ShowTips(string tips)
        {
            RichTextComponent richTextComponent = GetRichTextComponent();
            if (richTextComponent != null)
            {
                if (unusedTips.Contains(richTextComponent))
                {
                    unusedTips.Remove(richTextComponent);
                }

                if (usedTips.Contains(richTextComponent) == false)
                {
                    usedTips.Add(richTextComponent);
                }
                richTextComponent.ShowText(tips);
            }
            else
            {
                richTextComponent = UIPackage.CreateObject("UITipsWindow", "RichTipsComponent").asCom as RichTextComponent;
               
                richTextComponent.SetParent(this);
                MainComponent.AddChild(richTextComponent);
                richTextComponent.ShowText(tips);
                if (usedTips.Contains(richTextComponent) == false)
                {
                    usedTips.Add(richTextComponent);
                }
            }
          
           
        }

        public RichTextComponent GetRichTextComponent()
        {
            if (unusedTips.Count>0)
            {
                return unusedTips[0];
            }

            return null;
        }
    }
