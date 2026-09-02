using System.Windows.Media.Imaging;
using Boundless.Desktop.API;
using Boundless.Desktop.API.Models;
using Boundless.Desktop.Games.EpicGames.Fortnite.Data;
using Boundless.Desktop.Utils;

namespace Boundless.Desktop.Models;

public class CosmeticItemModel
{
    public AthenaCosmetic Cosmetic { get; }
    public string CosmeticId { get; }
    public CosmeticData? Data { get; private set; }
    public EFortRarity Rarity { get; private set; }
    public Uri IconUri { get; private set; }

    public bool IsNull => Data == null;
    
    public CosmeticItemModel(AthenaCosmetic cosmetic)
    {
        Cosmetic = cosmetic;
        CosmeticId = cosmetic.GetCosmeticId();
    }

    public async Task LoadAsync()
    {
        if (FortniteApiCache.TryGetCosmetic(CosmeticId, out var model))
            Data = model;
        else
        {
            var cosmeticModel = await FortniteApi.GetBRCosmeticAsync(CosmeticId);
            if (cosmeticModel.Status != 200)
            {
                Log("CosmeticItemModel", LogLevel.Error, $"({cosmeticModel.Status}): '{cosmeticModel.Error}'");
                return;
            }
            
            Data = cosmeticModel.Data;
            FortniteApiCache.CacheCosmetic(cosmeticModel);
        }
        
        Rarity = IsTranscendent(out var channel, out var tag) ? EFortRarity.Transcendent : Data.GetRarity();

        if (Data.TryGetCachedIcon(out var iconUri))
            IconUri = iconUri;
        else
        {
            var uri = Data.GetIcon(channel, tag);
            if (uri == null) // Fallback icon
            {
                IconUri = new Uri("pack://application:,,,/Images/Icons/CID_Random.png");
                return;
            }
            
            await Data.CacheIconAsync(uri);
            IconUri = new Uri(uri);
        }
    }
    
    private bool IsTranscendent(out string? channel, out string? tag)
    {
        if (RareCosmetics.IsRare(CosmeticId, null, null))
        {
            channel = null;
            tag = null;
            return true;
        }

        var variants = Cosmetic.Attributes?.Variants;
        if (variants != null)
        {
            foreach (var variant in variants)
            {
                foreach (var style in variant.Owned)
                {
                    if (!RareCosmetics.IsRare(CosmeticId, variant.Channel, style))
                        continue;

                    channel = variant.Channel;
                    tag = style;
                    return true;
                }
            }
        }

        channel = null;
        tag = null;
        return false;
    }
}