using Boundless.Desktop.FortniteAPI.Models;
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

    public AthenaVariant()
    { }
    
    public AthenaVariant(CosmeticVariants cosmeticVariants)
    {
        Channel = cosmeticVariants.Channel;
        Owned = cosmeticVariants.Options.Select(x => x.Tag).ToArray();
    }
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

    public AthenaCosmetic()
    { }

    [JsonIgnore]
    public CosmeticData CosmeticData { get; }
    
    public AthenaCosmetic(CosmeticData cosmeticData)
    {
        TemplateId = $"{cosmeticData.Type.BackendValue}:{cosmeticData.Id}";
        Attributes = new AthenaCosmeticAttributes
        {
            Variants = cosmeticData.Variants?.Select(x => new AthenaVariant(x)).ToArray() ?? []
        };

        CosmeticData = cosmeticData;
    }
    
    public string GetCosmeticId() => TemplateId.Split(":").Last(); // AthenaCharacter:CID...
    public static string GetCosmeticId(string templateId) => templateId.Split(":").Last();
}