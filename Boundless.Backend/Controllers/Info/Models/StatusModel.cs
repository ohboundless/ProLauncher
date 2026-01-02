using Newtonsoft.Json;

namespace Boundless.Backend.Controllers.Account.Models;

public class StatusModel
{
    [JsonProperty("is_up")] 
    public bool IsUp { get; set; }
    
    [JsonProperty("disabled_modules")]
    public List<string> DisabledModules { get; set; } = new();
    
    [JsonProperty("version_number")]
    public int VersionNumber { get; set; }
    
    [JsonProperty("version_str")]
    public string VersionString { get; set; } = string.Empty;
    
    [JsonProperty("patch_notes")]
    public string PatchNotes { get; set; } = string.Empty;
    
    [JsonProperty("motd")]
    public string Motd { get; set; } = string.Empty;
}