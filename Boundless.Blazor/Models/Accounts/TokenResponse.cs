using Newtonsoft.Json;

namespace Boundless.Blazor.Models.Accounts;

public class TokenResponse
{
    [JsonProperty("userId")]
    public string UserId { get; private set; }
    
    [JsonProperty("secret")]
    public string Secret { get; private set; }
    
    [JsonProperty("secretExpiresAt")]
    public DateTime SecretExpiresAt { get; private set; }
    
    [JsonProperty("nextSecret")]
    public string NextSecret { get; private set; }
}