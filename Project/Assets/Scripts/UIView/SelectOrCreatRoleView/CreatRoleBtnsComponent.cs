using FairyGUI;
using FairyGUI.Utils;

public class CreatRoleBtnsComponent:GComponent
{
        private GButton btnFashi;
        private GButton btnZhanshi;
        private GButton btnDaoshi;
        protected override void ConstructExtension(ByteBuffer buffer)
        {
                base.ConstructExtension(buffer);
                btnFashi = GetChild("btnFashi").asButton;
                btnZhanshi = GetChild("btnZhanshi").asButton;
                btnDaoshi = GetChild("btnDaoshi").asButton;
                btnFashi.onClick.Add(OnFaShiBtnClick);
                btnZhanshi.onClick.Add(OnZhanshiBtnClick);
                btnDaoshi.onClick.Add(OnDaoShiBtnClick);
        }

        private void OnDaoShiBtnClick(EventContext context)
        {
               
        }

        private void OnZhanshiBtnClick(EventContext context)
        {
               
        }

        private void OnFaShiBtnClick(EventContext context)
        {
                
        }
}
