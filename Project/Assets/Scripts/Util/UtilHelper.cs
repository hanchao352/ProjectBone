using System;
using FairyGUI;

public static class UtilHelper
{
        public static RoleInfo ToRoleInfo(this NRoleInfo roleInfo)
        {
            return new RoleInfo()
            {
                AccountId = roleInfo.Accountid,
                RoleId = roleInfo.RoleId,
                RoleName = roleInfo.Name,
                Level = roleInfo.Level,
                BigServerId = roleInfo.BigServerId,
                SmallServerId = roleInfo.SmallServerId
            };
        }
        
       
}
