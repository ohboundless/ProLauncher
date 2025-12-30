using System.Text.Json.Serialization;

namespace Boundless.Blazor.Accounts;

public enum GameAccountType
{
    Fortnite,
    Valorant
}

public interface IGameAccount
{
    [JsonPropertyName("type")]
    public GameAccountType Type { get; }
    
    [JsonPropertyName("icon")]
    public string Icon { get; }
    
    [JsonPropertyName("username")]
    public string Username { get; }
}