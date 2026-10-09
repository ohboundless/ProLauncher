using Boundless.Backend.Managers.Tokens.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Boundless.Backend.Controllers.Account.Models;

public class TokenResponse
{
    /*[JsonProperty("userId")]
    public string UserId { get; private set; }
    
    [JsonProperty("secret")]
    public string Secret { get; private set; }
    
    [JsonProperty("secretExpiresAt")]
    public DateTime SecretExpiresAt { get; private set; }
    
    [JsonProperty("nextSecret")]
    public string NextSecret { get; private set; }*/

    public static JsonResult Create(Token token)
    {
        return new JsonResult(new
        {
            userId = token.AccountId,
            secret = token.Secret,
            secretExpiresAt = token.ExpiresAt,
            nextSecret = token.NextSecret
        });
    }
}