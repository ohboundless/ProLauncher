using System.Text.Json.Serialization;
using Boundless.Desktop.Games;

namespace Boundless.Desktop.Models.Accounts;

public class UserAccount
{
    [JsonPropertyName("icon")]
    public string Icon { get; }

    [JsonPropertyName("username")]
    public string Username { get; }

    [JsonPropertyName("Accounts")]
    public IGameAccount[] Accounts { get; }
}