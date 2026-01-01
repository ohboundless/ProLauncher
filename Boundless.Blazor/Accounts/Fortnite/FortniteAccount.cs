using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Boundless.Blazor.Accounts.Fortnite;

public class FortniteAccount : IGameAccount
{
    public GameAccountType Type { get; }
    public string Icon { get; }
    public string Username { get; }

    [JsonProperty("device_code")]
    public string DeviceCode { get; }
}