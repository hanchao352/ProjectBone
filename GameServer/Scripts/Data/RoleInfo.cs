

using MongoDB.Bson;

public class RoleInfo
{
    public string AccountId { get; set; }
    public long RoleId { get; set; }
    public string RoleName { get; set; }
    public int Level { get; set; }
    
    public int BigServerId { get; set; }
    public int SmallServerId { get; set; }
}