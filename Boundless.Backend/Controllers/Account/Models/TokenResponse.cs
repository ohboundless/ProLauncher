using Boundless.Backend.Managers.Tokens.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Boundless.Backend.Controllers.Account.Models;

public class TokenResponse
{
    [JsonProperty("token")]
    public string Token { get; private set; }

    public static JsonResult Create(Token token)
    {
        return new JsonResult(new TokenResponse { Token = token.Secret });
    }
}