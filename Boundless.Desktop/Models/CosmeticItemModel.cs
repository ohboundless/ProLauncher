using System.Windows.Media.Imaging;
using Boundless.Desktop.API;
using Boundless.Desktop.API.Models;
using Boundless.Desktop.Games.EpicGames.Fortnite.Data;

namespace Boundless.Desktop.Models;

public class CosmeticItemModel
{
    public AthenaCosmetic Cosmetic { get; }
    public string CosmeticId { get;}
    public BRCosmeticModel Model { get; private set; }
    public EFortRarity Rarity { get; private set; }
    public Uri IconUri { get; private set; }
    
    public CosmeticItemModel(AthenaCosmetic cosmetic)
    {
        Cosmetic = cosmetic;
        CosmeticId = cosmetic.GetCosmeticId();
    }

    public async Task LoadAsync()
    {
        if (FortniteApiCache.TryGetCosmetic(CosmeticId, out var model))
            Model = model;
        else
        {
            Model = await FortniteApi.GetBRCosmeticAsync(CosmeticId);
            FortniteApiCache.SaveCosmetic(Model);
            FortniteApiCache.Save();
        }
        
        Rarity = IsTranscendent(out var channel, out var tag) ? EFortRarity.Transcendent : Model.GetRarity();

        if (Model.TryGetCachedIcon(out var iconUri))
            IconUri = iconUri;
        else
        {
            var uri = Model.GetIcon(channel, tag);
            await Model.SaveIconAsync(uri);
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