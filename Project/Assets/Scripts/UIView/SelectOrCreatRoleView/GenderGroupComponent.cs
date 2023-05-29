using FairyGUI;
using FairyGUI.Utils;
using UnityEngine;

public class GenderGroupComponent:GComponent
{
        GButton btnMen;
        GButton btnWomen;
        
        public int gener = 1; 
        
        public override void ConstructFromResource()
        {
                base.ConstructFromResource();
                btnMen = GetChild("rdoMen").asButton;
                btnWomen = GetChild("rdoWomen").asButton;
                btnMen.onClick.Add(OnMenBtnClick);
                btnWomen.onClick.Add(OnWomenBtnClick);
                btnMen.onClick.Call();
        }
        
      

        private void OnWomenBtnClick(EventContext context)
        {
                gener = (int)EnumGender.Female;
        }

        private void OnMenBtnClick(EventContext context)
        {
                gener = (int)EnumGender.Male;
        }
}
