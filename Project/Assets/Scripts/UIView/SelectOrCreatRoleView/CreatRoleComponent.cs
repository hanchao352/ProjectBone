using FairyGUI;
using FairyGUI.Utils;

public class CreatRoleComponent:GComponent
{
     GButton retruButton;
     GButton entergameButton;
     GTextInput nameinput;
     private CreatRoleBtnsComponent btns;
     private GenderGroupComponent _genderGroupComponent;

    

     public override void ConstructFromResource()
     {
         base.ConstructFromResource();
         retruButton = GetChild("btnReturn").asButton;
         retruButton.onClick.Add(OnReturnBtnClick);
         entergameButton = GetChild("btnEnterGame").asButton;
         entergameButton.onClick.Add(OnEnterGameBtnClick);
         nameinput = GetChild("txtName").asTextInput;
     }

     private void OnEnterGameBtnClick(EventContext context)
     {
         string name = nameinput.text;
         if (string.IsNullOrEmpty(nameinput.text))
         {
             UIManager.Instance.ShowTips("角色名不能为空");
         }
         else
         {
              // CreateRoleMod.Instance.CreareRoleRquest(name,_genderGroupComponent.gener);
         }
     }

     private void OnReturnBtnClick(EventContext context)
     {
         
     }


     protected override void OnUpdate()
     {
         base.OnUpdate();
     }
}
