using System.Collections.Concurrent;
using System.Data;
using System.IO;
using System.Net.Http;
using Boundless.Desktop.FortniteAPI.Models;
using Boundless.Desktop.Config;
using Newtonsoft.Json;
using RestSharp;

namespace Boundless.Desktop.FortniteAPI;

public static class FortniteCache
{
    private static string FolderPath => Configs.DirectoryName + "\\Cache";
    private static string ImagePath => FolderPath + "\\Images";
    private static string FilePath => FolderPath + "\\Cosmetics.json";
    private static RestClient _client = new();
    
    public static CosmeticData[]? Cosmetics { get; private set; } = [];
    
    #region Loading And Saving
    public static void Load(bool isFirstTime = true)
    {
        try
        {
            Directory.CreateDirectory(ImagePath);
            
            if (!File.Exists(FilePath))
            {
                Log("FortniteApiCache", LogLevel.Info,
                    "Data cache for fortnite-api.com did not exist. Creating one now...");
                Save();
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

    private static void Save()
    {
        Directory.CreateDirectory(FolderPath);
        Directory.CreateDirectory(ImagePath);

        Cosmetics = [];
        var json = JsonConvert.SerializeObject(Cosmetics, Formatting.Indented);
        File.WriteAllText(FilePath, json);

        Log("FortniteApiCache", LogLevel.Info, $"Saved cosmetics. ({FilePath})");
    }
    #endregion
    
    #region Cosmetic Functions
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
    
    public static bool TryGetCosmetic(string cosmeticId, out CosmeticData? cosmeticModel)
    {
        foreach (var cosmetic in Cosmetics)
        {
            if (cosmetic.Id.ToLower() != cosmeticId)
                continue;
            
            cosmeticModel = cosmetic;
            return true;
        }

        cosmeticModel = null;
        return false;
    }
    #endregion

    #region Icons
    private static string GetImagePath(this CosmeticData cosmeticData, string? channel = null, string? tag = null)
    {
        if (channel != null && tag != null)
            return $"{ImagePath}\\{cosmeticData.Id}_{channel}_{tag}.png";
        
        return $"{ImagePath}\\{cosmeticData.Id}.png";
    }

    public static bool TryGetCachedIcon(this CosmeticData cosmeticData, string? channel, string? tag, out Uri? source)
    {
        var path = cosmeticData.GetImagePath(channel, tag);
        if (File.Exists(path))
        {
            source = new Uri(path, UriKind.Absolute);
            return true;
        }

        source = null;
        return false;
    }

    public static async Task<string> CacheIconAsync(this CosmeticData cosmeticData, string uri, string? channel = null, string? tag = null)
    {
        var path = cosmeticData.GetImagePath(channel, tag);
        await CacheImageAsync(uri, path);
        
        return path;
    }
    #endregion
    
    #region Image Caching
    private static readonly ConcurrentDictionary<string, Lazy<Task>> _activeDownloads = new();

    public static async Task<string> CacheImageFromUrlAsync(string uri, string? overrideName = null)
    {
        var fileName = overrideName != null 
            ? $"{overrideName}.png" 
            : uri.Split('/').Last();
        var path = Path.Combine(ImagePath, fileName);

        var lazyTask = _activeDownloads.GetOrAdd(path, _ => new Lazy<Task>(async () =>
        {
            try
            {
                if (File.Exists(path)) 
                    return;

                await CacheImageAsync(uri, path);
            }
            finally
            {
                _activeDownloads.TryRemove(path, out var _);
            }
        }));
        
        await lazyTask.Value;
        
        return path;
    }
    
    public static async Task CacheImageAsync(string uri, string path)
    {
        if (File.Exists(path))
            return;
        
        var request = new RestRequest(uri);
        request.AddHeader("User-Agent", "ProLauncher/" + VERSION);
        request.AddHeader("Accept", "mage/avif,image/webp,image/apng,image/*,*/*;q=0.8");
        
        var data = await _client.DownloadDataAsync(request);

        if (data == null)
            throw new HttpRequestException($"Unable to download '{uri}'");

        await File.WriteAllBytesAsync(path, data);
    }
    #endregion
}