using System.IO;
using Newtonsoft.Json;

namespace Boundless.Desktop.Config;

public sealed class SwapperConfig() : AbstractConfigData("swapper")
{
    public string? CharacterId { get; set; }
    public string? BackpackId { get; set; }
    public string[]? CharacterCps { get; set; }
    public string[]? BackpackCps { get; set; }
    public string? PickaxeId { get; set; }
    public string? EmoteId { get; set; }
}