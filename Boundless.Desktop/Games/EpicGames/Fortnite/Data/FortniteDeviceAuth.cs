using Newtonsoft.Json;

namespace Boundless.Desktop.Games.EpicGames.Fortnite.Data;

public class FortniteDeviceAuth
{
    [JsonProperty("deviceId")]
    
    public string DeviceId { get; set; }
    
    [JsonProperty("accountId")]
    public string AccountId { get; set; }
    
    [JsonProperty("secret")]
    public string Secret { get; set; }
}