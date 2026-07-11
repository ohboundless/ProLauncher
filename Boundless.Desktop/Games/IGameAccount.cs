using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Boundless.Desktop.Games;

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
    
    [JsonProperty("id")]
    public string Id { get; }
    
    [JsonProperty("username")]
    public string Username { get; }
}