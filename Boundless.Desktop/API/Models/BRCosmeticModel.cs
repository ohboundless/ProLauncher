using System.Text.Json.Serialization;

namespace Boundless.Desktop.API.Models;

using System.Text.Json.Serialization;

public sealed record BRCosmeticModel(
    [property: JsonPropertyName("status")] int Status,
    [property: JsonPropertyName("data")] Data Data
);

public sealed record Data(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("description")] string Description,
    [property: JsonPropertyName("type")] Type Type,
    [property: JsonPropertyName("rarity")] Rarity Rarity,
    [property: JsonPropertyName("introduction")] Introduction Introduction,
    [property: JsonPropertyName("images")] Images Images,
    [property: JsonPropertyName("variants")] Variants[] Variants,
    [property: JsonPropertyName("showcaseVideo")] string ShowcaseVideo,
    [property: JsonPropertyName("added")] DateTimeOffset Added
);

public sealed record Type(
    [property: JsonPropertyName("value")] string Value,
    [property: JsonPropertyName("displayValue")] string DisplayValue,
    [property: JsonPropertyName("backendValue")] string BackendValue
);

public sealed record Rarity(
    [property: JsonPropertyName("value")] string Value,
    [property: JsonPropertyName("displayValue")] string DisplayValue,
    [property: JsonPropertyName("backendValue")] string BackendValue
);

public sealed record Introduction(
    [property: JsonPropertyName("chapter")] string Chapter,
    [property: JsonPropertyName("season")] string Season,
    [property: JsonPropertyName("text")] string Text,
    [property: JsonPropertyName("backendValue")] int BackendValue
);

public sealed record Images(
    [property: JsonPropertyName("smallIcon")] string SmallIcon,
    [property: JsonPropertyName("icon")] string Icon,
    [property: JsonPropertyName("featured")] string Featured,
    [property: JsonPropertyName("lego")] Lego Lego,
    [property: JsonPropertyName("bean")] Bean Bean
);

public sealed record Lego(
    [property: JsonPropertyName("small")] string Small,
    [property: JsonPropertyName("large")] string Large,
    [property: JsonPropertyName("wide")] string Wide
);

public sealed record Bean(
    [property: JsonPropertyName("small")] string Small,
    [property: JsonPropertyName("large")] string Large
);

public sealed record Variants(
    [property: JsonPropertyName("channel")] string Channel,
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("options")] Options[] Options
);

public sealed record Options(
    [property: JsonPropertyName("tag")] string Tag,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("image")] string Image
);