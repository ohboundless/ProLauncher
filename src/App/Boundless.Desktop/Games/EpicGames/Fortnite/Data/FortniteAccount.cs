using Newtonsoft.Json;

namespace Boundless.Desktop.Games.EpicGames.Fortnite.Data;

public class FortniteAccount : IGameAccount
{
    public FortniteProfileData? ProfileData;

    public GameAccountType Type => GameAccountType.Fortnite;

    // Get equipped character or default to Renegade Raider
    public string Icon => ProfileData?.EquippedCharacterId == null
        ? "https://fortnite-api.com/images/cosmetics/br/cid_028_athena_commando_f/icon.png"
        : $"https://fortnite-api.com/images/cosmetics/br/{ProfileData.EquippedCharacterId}/icon.png";

    public string Id { get; set; }

    public string UsernamePrivate { get; set; }

    [JsonIgnore] public string Username => Settings.HideAccountNames ? $"Name Hidden ({AccountNumber + 1})" : UsernamePrivate;

    public DateTime? LastLaunchTime { get; set; }
    public int AccountNumber { get; set; }

    [JsonProperty("device_auth")] public FortniteDeviceAuth DeviceAuth { get; set; }

    public FortniteAccount()
    {
    }

    public FortniteAccount(FortniteToken token, FortniteDeviceAuth deviceAuth)
    {
        Id = token.AccountId;
        UsernamePrivate = token.DisplayName;
        DeviceAuth = deviceAuth;
        AccountNumber = Accounts.Fortnite.Accounts?.Count ?? 0;

        ConfigData.CurrentAccounts.AccountIdsByGame["Fortnite"] = token.AccountId;
        ConfigData.Save();
    }
}