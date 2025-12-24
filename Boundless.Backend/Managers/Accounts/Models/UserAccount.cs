using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Boundless.Backend.Managers.Accounts.Models;

public class UserAccount
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("account_id")] public string AccountId = string.Empty;
    [BsonElement("username")] public string Username = string.Empty;
    [BsonElement("password")] public string Password = string.Empty; // SHA256
    [BsonElement("message")] public string? Message;
    [BsonElement("tokens")] public List<string> Tokens = new(); // Token.Secret
}