using System.Data;
using System.IO;
using System.Net.Http;
using System.Windows.Media.Imaging;
using Boundless.Desktop.API.Models;
using Boundless.Desktop.Config;
using Boundless.Desktop.Models;
using Boundless.Desktop.UserControls;
using Boundless.Desktop.Utils;
using Boundless.Swapper.Cosmetics.Models;
using Newtonsoft.Json;
using RestSharp;
using SkiaSharp;

namespace Boundless.Desktop.API;

public static class FortniteCache
{
    private static string FolderPath => Configuration.DirectoryName + "\\Cache";
    private static string ImagePath => FolderPath + "\\Images";
    private static string FilePath => FolderPath + "\\Cosmetics.json";

    public static CosmeticData[]? Cosmetics { get; private set; } = [];

    private static RestClient _client = new();

    public static async Task WaitForCosmetics()
    {
        if (Cosmetics == null)
            return; // Cosmetics will be null when after a tab is loaded in order to save memory
        
        while (Cosmetics.Length == 0)
        {
            await Task.Delay(10);
        }
    }
    
    public static async Task GetAllCosmetics()
    {
        var result = await FortniteApi.GetBRCosmeticsAsync();
        if (result == null)
            throw new NoNullAllowedException("Unable to retrieve BR cosmetics from fortnite-api.com");
        
        Cosmetics = result.Data;
    }
    
    public static void Load(bool isFirstTime = true)
    {
        try
        {
            if (!File.Exists(FilePath))
            {
                Log("FortniteApiCache", LogLevel.Info,
                    "Data cache for fortnite-api.com did not exist. Creating one now...");
                Flush();
                return;
            }

            var text = File.ReadAllText(FilePath);
            Cosmetics = JsonConvert.DeserializeObject<CosmeticData[]>(text) ??
                        throw new JsonException($"Could not deserialize: \n\"{text}\"");

            Log("FortniteApiCache", LogLevel.Info, $"Loaded cosmetics cache. ({FilePath})");
        }
        catch (Exception ex)
        {
            if (!isFirstTime)
            {
                Log("FortniteApiCache", LogLevel.Fatal, "Unable to load FortniteApiCache. Resetting...");
                Log("FortniteApiCache", LogLevel.Fatal, ex.ToString());

                throw new ApplicationException("Unable to load FortniteApiCache!");
            }
            
            Log("FortniteApiCache", LogLevel.Error, "Unable to load FortniteApiCache. Resetting...");
            
            if (File.Exists(FilePath))
                File.Delete(FilePath);
            
            Load(false);
        }
    }
    
    public static void Flush()
    {
        Directory.CreateDirectory(FolderPath);
        Directory.CreateDirectory(ImagePath);
        
        var json = JsonConvert.SerializeObject(Cosmetics, Formatting.Indented);
        File.WriteAllText(FilePath, json);

        Cosmetics = [];
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

    private static string GetImagePath(this CosmeticData cosmeticData) => $"{ImagePath}\\{cosmeticData.Id}.png";
    private static string GetImagePath(this CosmeticItemModel model) => $"{ImagePath}\\{model.CosmeticId}.png";

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
    
    private static string GetImagePath(this BaseCosmetic model) => $"{ImagePath}\\{model.Id}.png";
    
    /// <summary>
    /// Cache icons from .pak files
    /// </summary>
    /// <param name="model"></param>
    /// <param name="bitmap"></param>
    /// <returns>The file path to the png</returns>
    public static async Task<string?> CachePakIconAsync(this CosmeticItemModel model)
    {
        var path = model.GetImagePath();
        if (!File.Exists(path))
        {
            var bitmap = model.PakCosmetic?.GetSmallIcon();
            if (bitmap == null)
            {
                Log("FortniteCache", LogLevel.Error, $"Unable to load small icon for {model.CosmeticId}");
                return null;
            }
            
            var data = bitmap.Encode(SKEncodedImageFormat.Png, 100);
            await File.WriteAllBytesAsync(path, data.ToArray());
        }

        return path;
    }
}