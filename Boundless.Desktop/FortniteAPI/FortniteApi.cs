using Boundless.Desktop.FortniteAPI.Models;
using Boundless.Desktop.Utils;
using Boundless.Swapper.Cosmetics.Models;
using CUE4Parse.GameTypes.FN.Enums;
using RestSharp;
using static Boundless.Desktop.Utils.WebUtils;

namespace Boundless.Desktop.FortniteAPI;

public static class FortniteApi
{
    private const string BASE_URI = "https://fortnite-api.com/";
    private static RestClient _client = new(BASE_URI);
    
    public static async Task<BRCosmeticModel?> GetBRCosmeticsAsync()
    {
        var request = new RestRequest("v2/cosmetics/br");
        return await _client.GetJsonAsync<BRCosmeticModel?>(request);
    }

    // TODO series
    public static EFortRarity GetRarity(this CosmeticData data)
    {
        return Enum.TryParse<EFortRarity>(data.Rarity.DisplayValue, out var rarity) ? rarity : EFortRarity.Common;
    }

    public static string GetName(this CosmeticData data, string? channel, string? tag)
    {
        var name = data.Name;
        if (channel == null || tag == null)
            return name;
        
        var option = data.GetOption(channel, tag);
        if (option != null)
            return $"{name} ({option.Name.CapitalizeWord()}";
        
        Log("FortniteAPI", LogLevel.Error, $"Unable to get name and style for {data.Id} using channel '{channel}' and tag '{tag}'");
        return string.Empty;
    }

    public static string? GetIcon(this CosmeticData data, string? channel, string? tag)
    {
        if (channel == null || tag == null)
        {
            var images = data.Images;
            if (images.Icon != null)
                return images.Icon;

            Log("FortniteApi", LogLevel.Warn, $"Large icon for {data.Id} is null. Falling back to small icon...");

            if (images.SmallIcon != null) 
                return images.SmallIcon;
            
            Log("FortniteApi", LogLevel.Warn,
                $"Small icon for {data.Id} is null. Unable to retrieve icon. Returning null...");
            return null;

        }

        var option = data.GetOption(channel, tag);
        if (option != null)
            return option.Image;
            
        
        Log("FortniteAPI", LogLevel.Error, $"Unable to get icon for {data.Id} using channel '{channel}' and tag '{tag}'");
        return string.Empty;
    }

    private static CosmeticVariantOptions? GetOption(this CosmeticData data, string channel, string tag)
    {
        foreach (var variant in data.Variants)
        {
            if (variant.Channel != channel)
                continue;

            foreach (var option in variant.Options)
            {
                if (option.Tag == tag)
                    return option;
            }
        }

        return null;
    }
}