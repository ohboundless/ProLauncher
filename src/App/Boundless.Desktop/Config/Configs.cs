using System.Data;
using System.IO;
using Newtonsoft.Json;

namespace Boundless.Desktop.Config;

public abstract class AbstractConfigData(string name)
{
    [JsonIgnore]
    public readonly string Name = name;

    public void Save()
    {
        Configs.SaveConfig(Name, this);
    }
}

public static class Configs
{
    private static readonly List<AbstractConfigData> List = [];

    public static T GetConfig<T>(string name)
        where T : AbstractConfigData
    {
        var config = List.FirstOrDefault(x => x.Name == name);
        if (config != null) 
            return (T)config;
        
        var data = InitializeConfig<T>(name);
        List.Add(data);

        return data;
    }
    
    public static string DirectoryName =>
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\BoundlessLauncher";

    private static string GetFilePath(string name) => $"{DirectoryName}\\{name}.json";

    private static T InitializeConfig<T>(string name)
        where T : AbstractConfigData
    {
        var filePath = GetFilePath(name);
        if (!File.Exists(filePath))
        {
            SaveConfig(name, Activator.CreateInstance<T>());
        }

        var json = File.ReadAllText(filePath);
        var data = JsonConvert.DeserializeObject<T>(json) ?? throw new JsonException($"Could not deserialize json from '{filePath}'");

        Log(LogCategory.Startup, LogLevel.Info, $"Loaded config: '{name}'");

        return data;
    }

    internal static void SaveConfig(string name, AbstractConfigData data)
    {
        var filePath = GetFilePath(name);
        var json = JsonConvert.SerializeObject(data, Formatting.Indented);
        File.WriteAllText(filePath, json);
    }
}