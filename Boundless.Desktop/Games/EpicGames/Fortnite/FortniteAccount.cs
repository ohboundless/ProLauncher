using Boundless.Desktop.Config;
using Newtonsoft.Json;
using static Boundless.Desktop.Config.Configuration;

namespace Boundless.Desktop.Games.EpicGames.Fortnite;

public class FortniteAccount : IGameAccount
{
    // Most recently loaded FortniteAccount
    public static FortniteAccount? Recent;
    
    public GameAccountType Type => GameAccountType.Fortnite;
    public string Icon { get; set; }
    public string Id { get; set; }
    public string Username { get; set; }
    
    [JsonProperty("refresh_token")]
    public string RefreshToken { get; set; }
    
    [JsonProperty("refresh_expires_at")]
    public DateTime RefreshExpiresAt { get; set; }

    public FortniteAccount()
    { }
    
    public FortniteAccount(FortniteHandler.FortniteToken token)
    {
        Id = token.AccountId;
        Username = token.DisplayName;
        Icon = "https://fortnite.gg/img/items/899/style-1.jpg?4"; // Renegade Raider placeholder
        RefreshToken = token.RefreshToken;
        RefreshExpiresAt = token.RefreshExpiresAt;

        Recent = this;
        ConfigData!.CurrentAccounts.AccountIdsByGame["Fortnite"] = token.AccountId;
        SaveConfig();
    }
}