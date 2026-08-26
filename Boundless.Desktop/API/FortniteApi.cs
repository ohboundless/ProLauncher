using Boundless.Desktop.API.Models;
using Boundless.Desktop.Utils;
using RestSharp;
using static Boundless.Desktop.Utils.WebUtils;

namespace Boundless.Desktop.API;

public static class FortniteApi
{
    private const string BASE_URI = "https://fortnite-api.com/";
    private static RestClient _client = new(BASE_URI);
    
    public static async Task<BRCosmeticModel> GetBRCosmeticAsync(string cosmeticId)
    {
        var request = new RestRequest("v2/cosmetics/br/" + cosmeticId);
        return await _client.GetJsonAsync<BRCosmeticModel>(request);
    }

    // TODO series
    public static EFortRarity GetRarity(this BRCosmeticModel model)
    {
        return Enum.TryParse<EFortRarity>(model.Data.Rarity.DisplayValue, out var rarity) ? rarity : EFortRarity.Common;
    }

    public static string GetName(this BRCosmeticModel model, string? channel, string? tag)
    {
        var name = model.Data.Name;
        if (channel == null || tag == null)
            return name;
        
        var option = model.GetOption(channel, tag);
        if (option != null)
            return $"{name} ({option.Name.CapitalizeWord()}";
        
        Log("FortniteAPI", LogLevel.Error, $"Unable to get name and style for {model.Data.Id} using channel '{channel}' and tag '{tag}'");
        return string.Empty;
    }

    public static string GetIcon(this BRCosmeticModel model, string? channel, string? tag)
    {
        if (channel == null || tag == null) 
            return model.Data.Images.Icon;

        var option = model.GetOption(channel, tag);
        if (option != null)
            return option.Image;
            
        
        Log("FortniteAPI", LogLevel.Error, $"Unable to get icon for {model.Data.Id} using channel '{channel}' and tag '{tag}'");
        return string.Empty;
    }

    private static Options? GetOption(this BRCosmeticModel model, string channel, string tag)
    {
        foreach (var variant in model.Data.Variants)
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