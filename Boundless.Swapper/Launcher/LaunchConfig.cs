using Newtonsoft.Json;

namespace Boundless.Swapper.Launcher;

public class Edit(string assetPath, string dataFilePath, string containerPath)
{
    public string AssetPath = assetPath;
    public string DataFilePath = dataFilePath;
    public string ContainerPath = containerPath;
}

public class LaunchConfig
{
    public string LauncherDirectory { get; }
    public string ExePath => LauncherDirectory + "\\ProSwapper.Launcher.exe";
    public string EditsPath => LauncherDirectory + "\\Edits.json";
    public string DataPath => LauncherDirectory + "\\Data";
    
    private List<Edit> Edits { get; } = new();

    public LaunchConfig(string launcherDirectory)
    {
        LauncherDirectory = launcherDirectory;
        Directory.CreateDirectory(LauncherDirectory);
        Directory.CreateDirectory(DataPath);
    }

    public void Save()
    {
        var json = JsonConvert.SerializeObject(Edits, Formatting.Indented);
        File.WriteAllText(EditsPath, json);
    }

    public void SaveAssetData(string assetPath, string containerPath, byte[] buffer)
    {
        var filePath = DataPath + "\\" + assetPath.Split("/").Last();
        var edit = new Edit(assetPath, filePath, containerPath);

        File.WriteAllBytes(filePath, buffer);
        Edits.Add(edit);
        Save();
    }
}