using FairyGUI;
using FairyGUI.Utils;

public class SelectRoleBtn:GButton
{
        GImage icon;
        GTextField nameText;
        GTextField levelText;
        GTextField jobText;
        GButton deleteBtn;
        public override void ConstructFromXML(XML xml)
        {
                base.ConstructFromXML(xml);
                icon = GetChild("icon").asImage;
                nameText = GetChild("roleName").asTextField;
                levelText = GetChild("roleLevel").asTextField;
                jobText = GetChild("roleJob").asTextField;
                deleteBtn = GetChild("btDel").asButton;
                deleteBtn.onClick.Add(OnDeleteBtnClick);
        }

        private void OnDeleteBtnClick(EventContext context)
        {
               
        }
}
