using System.Data;
using System.IO;
using System.Net.Http;
using Boundless.Desktop.FortniteAPI.Models;
using Boundless.Desktop.Config;
using Boundless.Desktop.Utils;
using Newtonsoft.Json;
using RestSharp;

namespace Boundless.Desktop.FortniteAPI;

public static class FortniteCache
{
    private static string FolderPath => Configuration.DirectoryName + "\\Cache";
    private static string ImagePath => FolderPath + "\\Images";
    private static string FilePath => FolderPath + "\\Cosmetics.json";
    private static RestClient _client = new();
    
    public static CosmeticData[]? Cosmetics { get; private set; } = [];
    
    #region Loading And Saving
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

    public static async Task<string> CacheIconAsync(this CosmeticData cosmeticData, string uri)
    {
        var request = new RestRequest(uri);
        var data = await _client.DownloadDataAsync(request);

        if (data == null)
            throw new HttpRequestException($"Unable to download '{uri}'");

        var path = cosmeticData.GetImagePath();
        await File.WriteAllBytesAsync(path, data);

        return path;
    }
    #endregion
}