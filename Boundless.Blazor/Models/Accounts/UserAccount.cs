using System.Text.Json.Serialization;
using Boundless.Blazor.Accounts;
using Boundless.Blazor.Games;

namespace Boundless.Blazor.Models.Accounts;

public class UserAccount
{
    [JsonPropertyName("icon")]
    public string Icon { get; }

    [JsonPropertyName("username")]
    public string Username { get; }

    [JsonPropertyName("Accounts")]
    public IGameAccount[] Accounts { get; }
}