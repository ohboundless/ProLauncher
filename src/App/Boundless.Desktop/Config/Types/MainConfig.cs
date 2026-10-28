using Newtonsoft.Json;

namespace Boundless.Desktop.Config.Types;

public class MainConfig() : AbstractConfigData("config")
{
    [JsonProperty("version")] public string Version { get; set; } = VERSION;

    [JsonProperty("currentAccountEntries")]
    public CurrentAccountEntries CurrentAccounts { get; set; } = new();

    public class CurrentAccountEntries
    {
        [JsonProperty("accountIds")]
        public Dictionary<string, string> AccountIdsByGame = new();
    }
}