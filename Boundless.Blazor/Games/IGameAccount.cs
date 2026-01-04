using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Boundless.Blazor.Games;

public enum GameAccountType
{
    Fortnite,
    Valorant
}

public interface IGameAccount
{
    [JsonProperty("type")]
    public GameAccountType Type { get; }
    
    [JsonProperty("icon")]
    public string Icon { get; }
    
    [JsonProperty("username")]
    public string Username { get; }
}