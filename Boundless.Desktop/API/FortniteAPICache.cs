using System.IO;
using Boundless.Desktop.API.Models;
using Boundless.Desktop.Config;
using Boundless.Desktop.Utils;
using Newtonsoft.Json;

namespace Boundless.Desktop.API;

public static class FortniteAPICache
{
    private static string FolderPath => Configuration.DirectoryName + "\\Cache";
    private static string ImagePath => FolderPath + "\\Images";
    private static string FilePath => FolderPath + "\\cosmetics.json";
    
    public static List<BRCosmeticModel> Cosmetics { get; private set; } = new();

    public static async Task LoadAsync()
    {
        Directory.CreateDirectory(FolderPath);
        Directory.CreateDirectory(ImagePath);

        if (!File.Exists(FilePath))
        {
            Log("FortniteAPICache", LogLevel.Info,
                $"Data cache for fortnite-api.com did not exist. Creating one now...");
            await File.WriteAllTextAsync(FilePath, "[ ]");
        }

        var text = await File.ReadAllTextAsync(FilePath);
        Cosmetics = JsonConvert.DeserializeObject<List<BRCosmeticModel>>(text) ??
                    throw new JsonException($"Could not deserialize: \n\"{text}\"");
    }
}