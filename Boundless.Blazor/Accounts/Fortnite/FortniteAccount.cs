using System.Text.Json.Serialization;

namespace Boundless.Blazor.Accounts.Fortnite;

public class FortniteAccount : IGameAccount
{
    public GameAccountType Type { get; }
    public string Icon { get; }
    public string Username { get; }

    [JsonPropertyName("device_code")]
    public string DeviceCode;
}