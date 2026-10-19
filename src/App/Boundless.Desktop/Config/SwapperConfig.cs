using System.IO;
using Newtonsoft.Json;

namespace Boundless.Desktop.Config;

public class SwapperConfig
{
    public static SwapperConfig? ConfigData;

    #region Config Data
    
    public string? CharacterId { get; set; }
    public string? BackpackId { get; set; }
    public string[]? CharacterCps { get; set; }
    public string[]? BackpackCps { get; set; }
    
    #endregion
    
    public static string DirectoryName =>
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\BoundlessLauncher";

    private static string FilePath => DirectoryName + "\\" + "swapper_config.json";
    
    public static void InitializeConfig()
    {
        Directory.CreateDirectory(DirectoryName);
        
        if (!File.Exists(FilePath))
            SaveConfig();

        var json = File.ReadAllText(FilePath);
        ConfigData = JsonConvert.DeserializeObject<SwapperConfig>(json) 
                     ?? throw new JsonException($"Could not deserialize json from '{FilePath}'");
        
        Log(LogCategory.Startup, LogLevel.Info, "Loaded config.");
    }

    public static void SaveConfig()
    {
        ConfigData ??= new();
        File.WriteAllText(FilePath, JsonConvert.SerializeObject(ConfigData, Formatting.Indented));
    }
}