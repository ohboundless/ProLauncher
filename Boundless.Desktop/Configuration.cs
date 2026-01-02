using System.IO;
using Newtonsoft.Json;

namespace Boundless.Desktop;

public class Configuration
{
    public static Configuration? Config;

    [JsonProperty("version")] public string Version { get; set; } = VERSION;
    
    public static string DirectoryName =>
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\BoundlessLauncher";

    private static string FilePath => DirectoryName + "\\" + "Config_Desktop.json";
    public static string LoggingPath => DirectoryName + "\\" + "Boundless-.log";
    
    public static void Initialize()
    {
        Directory.CreateDirectory(DirectoryName);
        
        if (!File.Exists(FilePath))
            Save();

        var json = File.ReadAllText(FilePath);
        Config = JsonConvert.DeserializeObject<Configuration>(json) 
                 ?? throw new JsonException($"Could not deserialize json from '{FilePath}'");
        
        Logger.Information("Loaded config.");
    }

    private static void Save()
    {
        Config ??= new();
        File.WriteAllText(FilePath, JsonConvert.SerializeObject(Config, Formatting.Indented));
    }
}