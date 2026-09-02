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
    private static string FilePath => FolderPath + "\\Cosmetics.json";

    public static List<CosmeticData>? Cosmetics { get; private set; }

    private static RestClient _client = new();

    public static void Load()
    {
        if (!File.Exists(FilePath))
        {
            Log("FortniteApiCache", LogLevel.Info,
                "Data cache for fortnite-api.com did not exist. Creating one now...");
            Flush();
            return;
        }

        var text = File.ReadAllText(FilePath);
        Cosmetics = JsonConvert.DeserializeObject<List<CosmeticData>>(text) ??
                    throw new JsonException($"Could not deserialize: \n\"{text}\"");

        Log("FortniteApiCache", LogLevel.Info, $"Loaded cosmetics cache. ({FilePath})");
    }
    
    public static void Flush()
    {
        Directory.CreateDirectory(FolderPath);
        Directory.CreateDirectory(ImagePath);
        
        var json = JsonConvert.SerializeObject(Cosmetics, Formatting.Indented);
        File.WriteAllText(FilePath, json);
        
        Cosmetics?.Clear();
        Cosmetics = null;
        Log("FortniteApiCache", LogLevel.Info, $"Flushed cosmetics cache. ({FilePath})");
    }

    public static bool TryGetCosmetic(string cosmeticId, out CosmeticData? cosmeticModel)
    {
        if (Cosmetics == null)
            Load();

        foreach (var cosmetic in Cosmetics)
        {
            if (cosmetic.Id.ToLower() != cosmeticId)
                continue;

            Log("FortniteApiCache", LogLevel.Info, $"Found cached cosmetic: {cosmeticId}");
            
            cosmeticModel = cosmetic;
            return true;
        }

        cosmeticModel = null;
        return false;
    }

    public static void CacheCosmetic(BRCosmeticModel data)
    {
        if (Cosmetics == null)
            Load();
        
        Log("FortniteApiCache", LogLevel.Info, $"Caching cosmetic with id: {data.Data.Id}");
        Cosmetics.Add(data.Data);
    }

    private static string GetImagePath(this CosmeticData cosmeticData) => $"{ImagePath}\\{cosmeticData.Id}.png";

    public static bool TryGetCachedIcon(this CosmeticData cosmeticData, out Uri? source)
    {
        var path = cosmeticData.GetImagePath();
        if (File.Exists(path))
        {
            source = new Uri(path, UriKind.Absolute);
            return true;
        }

        source = null;
        return false;
    }

    public static async Task CacheIconAsync(this CosmeticData cosmeticData, string uri)
    {
        var request = new RestRequest(uri);
        var data = await _client.DownloadDataAsync(request);

        if (data == null)
            throw new HttpRequestException($"Unable to download '{uri}'");

        var path = cosmeticData.GetImagePath();
        await File.WriteAllBytesAsync(path, data);
    }
}