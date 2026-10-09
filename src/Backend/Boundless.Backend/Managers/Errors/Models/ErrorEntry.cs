using Boundless.Backend.Utils;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Boundless.Backend.Managers.Errors.Models;

public class ErrorEntry
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    
    [BsonGuidRepresentation(GuidRepresentation.Standard)]
    [BsonElement("guid")] 
    public Guid Guid;
    
    [BsonElement("account_id")] 
    public string? AccountId;
    
    [BsonElement("exception")] 
    public Exception Exception;
    
    [BsonElement("message")] 
    public string Message;
    
    [BsonElement("stack_trace")] 
    public string? StackTrace;
    
    [BsonElement("creation_date")] 
    public DateTime CreationDate;
    
    [BsonElement("ip_address")] 
    public string IpAddress;

    public ErrorEntry(string? accountId, Exception exception, HttpContext context)
    {
        Guid = Guid.CreateVersion7();
        AccountId = accountId;
        Exception = exception;
        Message = exception.Message;
        StackTrace = exception.StackTrace;
        CreationDate = DateTime.UtcNow;
        IpAddress = context.GetRemoteIpAddress();
    }
}