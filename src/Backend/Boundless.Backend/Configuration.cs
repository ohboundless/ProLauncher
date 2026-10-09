global using static Boundless.Backend.Configuration;
using Boundless.Backend.Controllers.Account.Models;
using Boundless.Backend.Exceptions;
using Newtonsoft.Json;

namespace Boundless.Backend;

public class Configuration
{
    public static Configuration? Config;

    [JsonProperty("status")] public StatusModel Status { get; set; } = new();
    
    public static string DirectoryName =>
        AppDomain.CurrentDomain.BaseDirectory + "\\BoundlessLauncher";

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

    public static void CheckConfig()
    {
        if (Config is null)
            throw new ConfigurationNullException();
    }
}