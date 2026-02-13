using Newtonsoft.Json;

namespace Boundless.Desktop.Games.EpicGames.Fortnite;

public class FortniteAccount : IGameAccount
{
    public GameAccountType Type => GameAccountType.Fortnite;
    public string Icon { get; set; }
    public string Username { get; set; }
    
    [JsonProperty("refresh_token")]
    public string RefreshToken { get; set; }
    
    [JsonProperty("refresh_expires_at")]
    public DateTime RefreshExpiresAt { get; set; }
}