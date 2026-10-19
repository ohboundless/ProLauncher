using System.IO;
using Boundless.Desktop.Config.Data;
using Boundless.Desktop.Utils;
using Newtonsoft.Json;

namespace Boundless.Desktop.Config;

public class Configuration
{
    public static Configuration? ConfigData;

    #region Config Data

    [JsonProperty("version")] public string Version { get; set; } = VERSION;

    [JsonProperty("currentAccountEntries")]
    public CurrentAccountEntries CurrentAccounts { get; set; } = new();
    #endregion

    public class CurrentAccountEntries
    {
        [JsonProperty("accountIds")]
        public Dictionary<string, string> AccountIdsByGame = new();
    }
    
    public static string DirectoryName =>
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\BoundlessLauncher";

    private static string FilePath => DirectoryName + "\\" + "config.json";
    public static string LoggingPath => DirectoryName + @"\logs\Boundless.Desktop-.log";
    
    public static void InitializeConfig()
    {
        Directory.CreateDirectory(DirectoryName);
        Directory.CreateDirectory(DirectoryName + "\\logs");
        
        if (!File.Exists(FilePath))
            SaveConfig();

        var json = File.ReadAllText(FilePath);
        ConfigData = JsonConvert.DeserializeObject<Configuration>(json) 
                 ?? throw new JsonException($"Could not deserialize json from '{FilePath}'");
        
        Log(LogCategory.Startup, LogLevel.Info, "Loaded config.");
    }

    public static void SaveConfig()
    {
        ConfigData ??= new();
        File.WriteAllText(FilePath, JsonConvert.SerializeObject(ConfigData, Formatting.Indented));
    }
}