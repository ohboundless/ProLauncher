using System.Data;
using System.IO;
using System.Net.Http;
using System.Windows.Media.Imaging;
using Boundless.Desktop.API.Models;
using Boundless.Desktop.Config;
using Boundless.Desktop.Utils;
using Newtonsoft.Json;
using RestSharp;

namespace Boundless.Desktop.API;

public static class FortniteApiCache
{
    private static string FolderPath => Configuration.DirectoryName + "\\Cache";
    private static string ImagePath => FolderPath + "\\Images";
    private static string FilePath => FolderPath + "\\cosmetics.json";

    public static List<BRCosmeticModel> Cosmetics { get; private set; } = new();

    private static RestClient _client = new();

    public static void Load()
    {
        if (!File.Exists(FilePath))
        {
            Log("FortniteAPICache", LogLevel.Info,
                "Data cache for fortnite-api.com did not exist. Creating one now...");
            Save();
            return;
        }
        
        var text = File.ReadAllText(FilePath);
        Cosmetics = JsonConvert.DeserializeObject<List<BRCosmeticModel>>(text) ??
                    throw new JsonException($"Could not deserialize: \n\"{text}\"");
    }

    public static void Save()
    {
        Directory.CreateDirectory(FolderPath);
        Directory.CreateDirectory(ImagePath);
        
        var json = JsonConvert.SerializeObject(Cosmetics, Formatting.Indented);
        File.WriteAllText(FilePath, json);
    }

    public static bool TryGetCosmetic(string cosmeticId, out BRCosmeticModel? cosmeticModel)
    {
        if (Cosmetics == null)
            throw new NoNullAllowedException("Cosmetics cache not loaded.");

        foreach (var cosmetic in Cosmetics)
        {
            if (cosmetic.Data.Id.ToLower() != cosmeticId)
                continue;

            cosmeticModel = cosmetic;
            return true;
        }

        cosmeticModel = null;
        return false;
    }

    public static void SaveCosmetic(BRCosmeticModel model)
    {
        Cosmetics.Add(model);
    }

    private static string GetImagePath(this BRCosmeticModel cosmeticModel) => $"{ImagePath}\\{cosmeticModel.Data.Id}.png";

    public static bool TryGetCachedIcon(this BRCosmeticModel cosmeticModel, out Uri? source)
    {
        var path = cosmeticModel.GetImagePath();
        if (File.Exists(path))
        {
            source = new Uri(path, UriKind.Absolute);
            return true;
        }

        source = null;
        return false;
    }

    public static async Task SaveIconAsync(this BRCosmeticModel cosmeticModel, string uri)
    {
        var request = new RestRequest(uri);
        var data = await _client.DownloadDataAsync(request);

        if (data == null)
            throw new HttpRequestException($"Unable to download '{uri}'");

        var path = cosmeticModel.GetImagePath();
        await File.WriteAllBytesAsync(path, data);
    }
}