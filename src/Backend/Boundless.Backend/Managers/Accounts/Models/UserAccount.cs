using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Boundless.Backend.Managers.Accounts.Models;

public enum AccountType
{
    User,
    Admin,
    Owner
}

public class UserAccount
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("account_id")]
    public string AccountId = string.Empty;
    
    [BsonElement("username")]
    public string Username = string.Empty;

    [BsonElement("friends")] 
    public string[] Friends = [];
    
    [BsonElement("password")] 
    public string Password = string.Empty; // SHA256

    [BsonElement("account_type")] 
    public AccountType AccountType = AccountType.User;
}