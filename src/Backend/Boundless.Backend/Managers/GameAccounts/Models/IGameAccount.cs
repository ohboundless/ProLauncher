using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Boundless.Backend.Managers.GameAccounts.Models;

public interface IGameAccount
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    
    [BsonElement("username")] 
    public string Username { get; set; }
}