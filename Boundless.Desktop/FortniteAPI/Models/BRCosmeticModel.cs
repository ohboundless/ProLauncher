using System.Text.Json.Serialization;

namespace Boundless.Desktop.FortniteAPI.Models;

using System.Text.Json.Serialization;

public sealed record BRCosmeticModel(
    [property: JsonPropertyName("error")] string Error,
    [property: JsonPropertyName("status")] int Status,
    [property: JsonPropertyName("data")] CosmeticData[] Data
);

public sealed record CosmeticData(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("description")] string Description,
    [property: JsonPropertyName("type")] CosmeticType Type,
    [property: JsonPropertyName("rarity")] CosmeticRarity Rarity,
    [property: JsonPropertyName("introduction")] CosmeticIntroduction Introduction,
    [property: JsonPropertyName("images")] CosmeticImages Images,
    [property: JsonPropertyName("variants")] CosmeticVariants[]? Variants,
    [property: JsonPropertyName("showcaseVideo")] string ShowcaseVideo,
    [property: JsonPropertyName("added")] DateTimeOffset Added
);

public sealed record CosmeticType(
    [property: JsonPropertyName("value")] string Value,
    [property: JsonPropertyName("displayValue")] string DisplayValue,
    [property: JsonPropertyName("backendValue")] string BackendValue
);

public enum ECosmeticType
{
    AthenaCharacter,
    AthenaBackpack,
    AthenaPickaxe,
    AthenaDance,
    AthenaToy,
    AthenaSpray,
    AthenaGlider,
    
    EverythingElse
}

public static class CosmeticTypeUtils
{
    public static ECosmeticType ParseCosmeticType(this CosmeticType cosmeticType)
    {
        return Enum.TryParse<ECosmeticType>(cosmeticType.BackendValue, out var type)
            ? type
            : ECosmeticType.EverythingElse;
    }
}

public sealed record CosmeticRarity(
    [property: JsonPropertyName("value")] string Value,
    [property: JsonPropertyName("displayValue")] string DisplayValue,
    [property: JsonPropertyName("backendValue")] string BackendValue
);

public sealed record CosmeticIntroduction(
    [property: JsonPropertyName("chapter")] string Chapter,
    [property: JsonPropertyName("season")] string Season,
    [property: JsonPropertyName("text")] string Text,
    [property: JsonPropertyName("backendValue")] int BackendValue
);

public sealed record CosmeticImages(
    [property: JsonPropertyName("smallIcon")] string? SmallIcon,
    [property: JsonPropertyName("icon")] string? Icon,
    [property: JsonPropertyName("featured")] string? Featured,
    [property: JsonPropertyName("lego")] CosmeticLego Lego,
    [property: JsonPropertyName("bean")] CosmeticBean Bean
);

public sealed record CosmeticLego(
    [property: JsonPropertyName("small")] string Small,
    [property: JsonPropertyName("large")] string Large,
    [property: JsonPropertyName("wide")] string Wide
);

public sealed record CosmeticBean(
    [property: JsonPropertyName("small")] string Small,
    [property: JsonPropertyName("large")] string Large
);

public sealed record CosmeticVariants(
    [property: JsonPropertyName("channel")] string Channel,
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("options")] CosmeticVariantOptions[] Options
);

public sealed record CosmeticVariantOptions(
    [property: JsonPropertyName("tag")] string Tag,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("image")] string Image
);