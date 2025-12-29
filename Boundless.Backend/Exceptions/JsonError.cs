using Boundless.Backend.Managers.Errors.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Boundless.Backend.Exceptions;

public class JsonError
{
    [JsonProperty("guid")]
    public string Guid { get; private set; }
    
    [JsonProperty("account_id")]
    public string? AccountId { get; private set; }
    
    [JsonProperty("error")]
    public string Error { get; private set; }
    
    [JsonProperty("message")]
    public string Message { get; private set; }
    
    public static async Task<JsonResult> HandleInternalAsync(HttpContext context, string? accountId, Exception exception, int errorCode = 500)
    {
        var error = new ErrorEntry(accountId, exception, context);
        await ErrorInstance!.AddAsync(error);

        return new JsonResult(new JsonError
        {
            AccountId = accountId,
            Guid = error.Guid.ToString(),
            Error = exception.GetType().Name,
            Message = exception.Message
        }) { StatusCode = errorCode };
    }
    
    public static async Task<JsonResult> HandleAsync(HttpContext context, string? accountId, string errorType, string message, int errorCode = 400)
    {
#if DEBUG // Only long non-internal errors when debugging
        var error = new ErrorEntry(accountId, new Exception(message), context);
        await ErrorInstance!.AddAsync(error);
#endif

        return new JsonResult(new JsonError
        {
            AccountId = accountId,
            Guid = error.Guid.ToString(),
            Error = errorType,
            Message = message
        }) { StatusCode = errorCode };
    }
}