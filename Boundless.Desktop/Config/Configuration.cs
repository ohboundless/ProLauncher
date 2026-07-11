using System.IO;
using Boundless.Desktop.Config.Data;
using Boundless.Desktop.Utils;
using Newtonsoft.Json;

namespace Boundless.Desktop.Config;

public class Configuration
{
    public static Configuration? Config;
    
    // Binary data holders
    public static LoginStorage? LoginData;

    // Actual config data
    [JsonProperty("version")] public string Version { get; set; } = VERSION;
    
    public static string DirectoryName =>
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\BoundlessLauncher";

    private static string FilePath => DirectoryName + "\\" + "Config.json";
    public static string LoggingPath => DirectoryName + @"\Logs\Boundless.Desktop-.log";
    
    public static void Initialize()
    {
        Directory.CreateDirectory(DirectoryName);
        Directory.CreateDirectory(DirectoryName + "\\Logs");
        
        if (!File.Exists(FilePath))
            Save();

        var json = File.ReadAllText(FilePath);
        Config = JsonConvert.DeserializeObject<Configuration>(json) 
                 ?? throw new JsonException($"Could not deserialize json from '{FilePath}'");
        
        Log(ELogCategory.Startup, ELogLevel.Info, "Loaded config.");

        // Load binary data holders
        LoginData = new LoginStorage();
        LoginData.TryRead();
    }

    private static void Save()
    {
        Config ??= new();
        File.WriteAllText(FilePath, JsonConvert.SerializeObject(Config, Formatting.Indented));
    }
}