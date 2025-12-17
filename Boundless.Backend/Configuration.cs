using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Serilog;

namespace Boundless.Backend;

public class Configuration
{
    public static Configuration? Config;

    [JsonPropertyName("version")] public string Version { get; set; } = VERSION;
    
    public static string DirectoryName =>
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\BoundlessLauncher";

    private static string FilePath => DirectoryName + "\\" + "Config.json";
    
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