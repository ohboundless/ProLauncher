using MongoDB.Bson.Serialization.Attributes;

namespace Boundless.Backend.Managers.GameAccounts.Models;

public class FortniteAccount : IGameAccount
{
    public string? Id { get; set; }
    
    [BsonElement("account_id")] 
    public string AccountId { get; set; }

    [BsonElement("username")] 
    public string Username { get; set; }
    
    [BsonElement("refresh_token")] 
    public string RefreshToken { get; set; }
}