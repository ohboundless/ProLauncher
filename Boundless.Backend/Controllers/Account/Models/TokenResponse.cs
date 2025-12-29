using System.Text.Json.Serialization;
using Boundless.Backend.Managers.Tokens.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Boundless.Backend.Controllers.Account.Models;

public class TokenResponse
{
    [JsonProperty("token")]
    public string Token;
    
    public static ContentResult Create(Token token)
    {
        return new ContentResult
        {
            ContentType = "application/json",
            Content = JsonConvert.SerializeObject(new TokenResponse
            {
                Token = token.Secret
            })
        };
    }
}