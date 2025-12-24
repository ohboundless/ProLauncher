using Boundless.Backend.Utils;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Boundless.Backend.Managers.Tokens.Models;

public class Token
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    
    [BsonElement("account_id")] public string AccountId;
    [BsonElement("secret")] public string Secret;
    [BsonElement("creation_date")] public DateTime CreationDate;
    [BsonElement("expires")] public long Expires;
    [BsonElement("ip_address")] public string IpAddress;

    public static Token Generate(string accountId, HttpContext context)
    {
        return new Token
        {
            AccountId = accountId,
            Secret = StringUtils.RandomHex(32),
            CreationDate =  DateTime.UtcNow,
            Expires = 3600,
            IpAddress = context.GetRemoteIpAddress()
        };
    }
}