using Boundless.Logging.Classes;
using Newtonsoft.Json;

namespace Boundless.Swapper.Launcher;

public class Edit(string assetPath, string dataFilePath, string containerPath, string swapType)
{
    public string AssetPath = assetPath;
    public string DataFilePath = dataFilePath;
    public string ContainerPath = containerPath;
    public string SwapType = swapType;
}

public class LaunchConfig
{
    public static string LauncherDirectory => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) +
                                              @"\BoundlessLauncher\Launcher";
    public string ExePath => LauncherDirectory + "\\ProSwapper.Launcher.exe";
    public string EditsPath => LauncherDirectory + "\\Edits.json";
    public string DataPath => LauncherDirectory + "\\Data";
    
    private List<Edit> Edits { get; } = new();

    public LaunchConfig()
    {
        Directory.CreateDirectory(LauncherDirectory);
        Directory.CreateDirectory(DataPath);

        if (File.Exists(EditsPath))
        {
            try
            {
                var json = File.ReadAllText(EditsPath);
                Edits = JsonConvert.DeserializeObject<List<Edit>>(json);
            }
            catch (Exception ex)
            {
                Log("LaunchConfig", LogLevel.Error, ex.ToString());
                Log("LaunchConfig", LogLevel.Error, "Resetting Edits.json");
                
                File.Delete(EditsPath);
                Save();
            }
        }
    }

    public void Save()
    {
        var json = JsonConvert.SerializeObject(Edits, Formatting.Indented);
        File.WriteAllText(EditsPath, json);
    }

    public void SaveAssetData(string assetPath, string containerPath, byte[] buffer, string swapType)
    {
        var duplicateEdit = Edits.FirstOrDefault(x => x.AssetPath == assetPath);
        if (duplicateEdit != null)
            Edits.RemoveAll(x => x.AssetPath == assetPath);
        
        var filePath = DataPath + "\\" + assetPath.Split("/").Last();
        var edit = new Edit(assetPath, filePath, containerPath, swapType);

        File.WriteAllBytes(filePath, buffer);
        Edits.Add(edit);
        Save();
    }

    public void RemoveAsset(string assetPath)
    {
        Edits.RemoveAll(x => x.AssetPath == assetPath);
        Save();
        
        var filePath = DataPath + "\\" + assetPath.Split("/").Last();
        File.Delete(filePath);
    }
    
    public void RemoveAssetBySwapType(string type)
    {
        var assets = Edits.FindAll(x => x.SwapType == type);
        Edits.RemoveAll(x => x.SwapType == type);
        Save();
        
        foreach (var asset in assets)
        {
            var filePath = DataPath + "\\" + asset.AssetPath.Split("/").Last();
            File.Delete(filePath);
        }
    }
}