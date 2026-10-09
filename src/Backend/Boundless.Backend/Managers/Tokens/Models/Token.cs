using Boundless.Backend.Utils;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Boundless.Backend.Managers.Tokens.Models;

public class Token
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    
    [BsonElement("account_id")] 
    public string AccountId;
    
    [BsonElement("secret")] 
    public string Secret;
    
    [BsonElement("secret_expires_at")] 
    public DateTime ExpiresAt;
    
    [BsonElement("next_secret")] 
    public string NextSecret; // Expires in 3 months. Rotated on use.
    
    [BsonElement("session_expires_at")] 
    public DateTime SessionExpiresAt;
    
    [BsonElement("creation_date")] 
    public DateTime CreationDate;
    
    [BsonElement("last_used")] 
    public DateTime LastUsed;
    
    [BsonElement("ip_address")] 
    public List<string> IpAddresses = new();
    
    public Token(string accountId, HttpContext context)
    {
        AccountId = accountId;
        SessionExpiresAt = DateTime.UtcNow.AddMonths(3);
        LastUsed = DateTime.UtcNow;
        Generate(context);
    }
    
    public void Generate(HttpContext context)
    {
        Secret = StringUtils.RandomHex(32);
        NextSecret = StringUtils.RandomHex(32);
        ExpiresAt = DateTime.UtcNow.AddHours(3);
        LastUsed = DateTime.UtcNow;
        IpAddresses.Add(context.GetRemoteIpAddress());
    }
}