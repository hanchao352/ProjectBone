
    using FairyGUI;
    using UnityEngine;

    public class RichTextComponent :GComponent
    {
        public GRichTextField tipsText;
        private TipsView TipsView;
        float starposY;
        float starposX;
        public override void ConstructFromXML(FairyGUI.Utils.XML cxml)
        {
            base.ConstructFromXML(cxml);

            tipsText= this.GetChild("n0").asRichTextField;
            this.visible = false;
        }
        
        public void ShowText(string text)
        {
            this.visible = true;
            this.position=new Vector2(0,Screen.height/2);
            tipsText.text = text;
          GTweener gTweener= TweenMoveY(-200, 2);
          gTweener.SetEase(EaseType.Linear);
          gTweener.OnComplete(OnTweenComplete);
        }

        private void OnTweenComplete(GTweener tweener)
        {
            if (TipsView != null)
            {
                this.position=new Vector2(0,Screen.height/2);
                this.visible = false;
                if (TipsView.usedTips.Contains(this))
                {
                    TipsView.usedTips.Remove(this);
                }
                if (TipsView.unusedTips.Contains(this)==false)
                {
                    TipsView.unusedTips.Add(this);
                }
                
            }
            
        }

        public void SetParent(TipsView tipsView)
        {
            TipsView = tipsView;
        }
    }
