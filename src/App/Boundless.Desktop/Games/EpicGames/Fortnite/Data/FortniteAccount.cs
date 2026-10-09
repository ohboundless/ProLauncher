using Boundless.Desktop.Games.EpicGames.Fortnite.API;
using Newtonsoft.Json;
using static Boundless.Desktop.Config.Configuration;

namespace Boundless.Desktop.Games.EpicGames.Fortnite.Data;

public class FortniteAccount : IGameAccount
{
    // Most recently loaded FortniteAccount
    public static FortniteAccount? Recent;

    public FortniteProfileData? ProfileData;
    
    public GameAccountType Type => GameAccountType.Fortnite;
    public string Icon { get; set; }
    public string Id { get; set; }
    public string Username { get; set; }
    public DateTime? LastLaunchTime { get; set; }
    
    [JsonProperty("device_auth")]
    public FortniteDeviceAuth DeviceAuth { get; set; }

    public FortniteAccount()
    { }
    
    public FortniteAccount(FortniteToken token, FortniteDeviceAuth deviceAuth)
    {
        Id = token.AccountId;
        Username = token.DisplayName;
        Icon = "https://fortnite-api.com/images/cosmetics/br/cid_028_athena_commando_f/icon.png"; // Renegade Raider placeholder
        DeviceAuth = deviceAuth;

        Recent = this;
        ConfigData!.CurrentAccounts.AccountIdsByGame["Fortnite"] = token.AccountId;
        SaveConfig();
    }
}