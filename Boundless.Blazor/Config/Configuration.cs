using Boundless.Blazor.Config.Data;
using Newtonsoft.Json;

namespace Boundless.Blazor.Config;

public class Configuration
{
    public static Configuration? Config;
    
    // Binary data holders
    public static LoginStorage? Login;

    // Actual config data
    [JsonProperty("version")] public string Version { get; set; } = VERSION;
    
    public static string DirectoryName =>
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\BoundlessLauncher";

    private static string FilePath => DirectoryName + "\\" + "Config_Blazor.json";
    public static string LoggingPath => DirectoryName + "\\" + "Boundless.Blazor-.log";
    
    public static void Initialize()
    {
        Directory.CreateDirectory(DirectoryName);
        
        if (!File.Exists(FilePath))
            Save();

        var json = File.ReadAllText(FilePath);
        Config = JsonConvert.DeserializeObject<Configuration>(json) 
                 ?? throw new JsonException($"Could not deserialize json from '{FilePath}'");
        
        Logger.Information("Loaded config.");

        // Load binary data holders
        Login = new LoginStorage();
        Login.Read();
    }

    private static void Save()
    {
        Config ??= new();
        File.WriteAllText(FilePath, JsonConvert.SerializeObject(Config, Formatting.Indented));
    }
}