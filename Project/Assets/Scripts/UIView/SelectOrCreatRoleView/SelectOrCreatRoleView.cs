using FairyGUI;
using UnityEngine;

public class SelectOrCreatRoleView:UIBase
{
    private GList rolelist;
    GButton retruButton;
    GButton entergameButton;
    CreatRoleComponent creatRoleComponent;
    SelectRoleComponent selectRoleComponent;

    protected override void SetPackageItemExtension()
    {
        Debug.Log("子类的SetPackageItemExtension");
        UIObjectFactory.SetPackageItemExtension("ui://SelectOrCreateRole/CreateRoleView", typeof(CreatRoleComponent));
        UIObjectFactory.SetPackageItemExtension("ui://SelectOrCreateRole/SelectRoleView", typeof(SelectRoleComponent));
        UIObjectFactory.SetPackageItemExtension("ui://SelectOrCreateRole/RdosGender", typeof(GenderGroupComponent));
        UIObjectFactory.SetPackageItemExtension("ui://SelectOrCreateRole/Btns", typeof(CreatRoleBtnsComponent));
        UIObjectFactory.SetPackageItemExtension("ui://SelectOrCreateRole/SelectRoleBtn", typeof(SelectRoleBtn));
    }

    protected override void OnInit(params object[] args)
    {
        creatRoleComponent =MainComponent.GetChild("n1").asCom as CreatRoleComponent   ;
        selectRoleComponent =MainComponent.GetChild("n0").asCom as SelectRoleComponent   ;
         
    }

   



    protected override void OnShow(params object[] args)
    {
        // bool hasrole = LoginMod.Instance.rolist.Count > 0;
        //  creatRoleComponent.visible = !hasrole; 
        //  selectRoleComponent.visible=hasrole; 
    }

    protected override void OnHide()
    {
        
    }

    protected override void OnDestroy()
    {
        
    }
}
