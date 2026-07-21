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
    
    [JsonProperty("device_auth")]
    public FortniteDeviceAuth DeviceAuth { get; set; }

    public FortniteAccount()
    { }
    
    public FortniteAccount(FortniteHandler.FortniteToken token, FortniteDeviceAuth deviceAuth)
    {
        Id = token.AccountId;
        Username = token.DisplayName;
        Icon = "https://fortnite.gg/img/items/899/style-1.jpg?4"; // Renegade Raider placeholder
        DeviceAuth = deviceAuth;

        Recent = this;
        ConfigData!.CurrentAccounts.AccountIdsByGame["Fortnite"] = token.AccountId;
        SaveConfig();
    }
}