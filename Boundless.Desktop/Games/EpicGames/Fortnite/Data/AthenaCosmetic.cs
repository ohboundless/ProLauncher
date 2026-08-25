using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Boundless.Desktop.Games.EpicGames.Fortnite.Data;

public class AthenaVariant
{
    [JsonProperty("channel")]
    public string Channel { get; set; }
    
    [JsonProperty("active")]
    public string Active { get; set; }
    
    [JsonProperty("owned")]
    public string[] Owned { get; set; }
}

public class AthenaCosmeticAttributes
{
    [JsonProperty("variants")]
    public AthenaVariant[]? Variants { get; set; }
}

public class AthenaCosmetic
{
    [JsonProperty("templateId")]
    public string TemplateId { get; set; }
    
    [JsonProperty("attributes")]
    public AthenaCosmeticAttributes? Attributes { get; set; }

    public string GetCosmeticId() => TemplateId.Split(":").Last(); // AthenaCharacter:CID...
}