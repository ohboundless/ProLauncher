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
    public string EditsPath => LauncherDirectory + "\\edits.json";
    public string DataPath => LauncherDirectory + "\\data";
    
    private List<Edit> Edits { get; } = new();

    public LaunchConfig()
    {
        LauncherDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) +
                            @"\BoundlessLauncher\launcher";
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
        var duplicateEdit = Edits.FirstOrDefault(x => x.AssetPath == assetPath);
        if (duplicateEdit != null)
            Edits.RemoveAll(x => x.AssetPath == assetPath);
        
        var filePath = DataPath + "\\" + assetPath.Split("/").Last();
        var edit = new Edit(assetPath, filePath, containerPath);

        File.WriteAllBytes(filePath, buffer);
        Edits.Add(edit);
        Save();
    }

    public void RemoveAsset(string assetPath)
    {
        Edits.RemoveAll(x => x.AssetPath == assetPath);
        
        var filePath = DataPath + "\\" + assetPath.Split("/").Last();
        File.Delete(filePath);
        
        Save();
    }
}