

using MongoDB.Bson;

public class AccountInfo
{
    public ObjectId Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }

    public List<RoleInfo> rolelist { get; set; }

   
}