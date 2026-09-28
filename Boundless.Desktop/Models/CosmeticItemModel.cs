using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Boundless.Desktop.API;
using Boundless.Desktop.API.Models;
using Boundless.Desktop.Games.EpicGames.Fortnite.Data;
using Boundless.Desktop.Utils;
using Boundless.Swapper.Cosmetics.Models;
using CUE4Parse.GameTypes.FN.Enums;

namespace Boundless.Desktop.Models;

public class CosmeticItemModel
{
    public AthenaCosmetic Cosmetic { get; }
    public string CosmeticId { get; }
    public CosmeticData? Data { get; private set; }
    public EFortRarity Rarity { get; private set; }
    public Uri IconUri { get; private set; }

    public bool IsNull => Data == null;

    // Data bindings
    public BitmapSource IconSource { get; set; }
    public Brush BackgroundRadial { get; set; }
    public Brush BorderColor { get; set; }
    public Visibility Visibility { get; set; }

    public CosmeticItemModel(AthenaCosmetic cosmetic)
    {
        Cosmetic = cosmetic;
        CosmeticId = cosmetic.GetCosmeticId();
    }

    #region Pak Cosmetics
    
    public BaseCosmetic? PakCosmetic { get; } // Cosmetic loaded from the pak files
    public bool IsPakCosmetic => PakCosmetic != null;

    public CosmeticItemModel(BaseCosmetic cosmetic)
    {
        PakCosmetic = cosmetic;
        CosmeticId = cosmetic.Id;
        Rarity = cosmetic.Rarity ?? EFortRarity.Uncommon;
    }
    
    #endregion
    
    public string GetItemName()
    {
        var name = IsPakCosmetic ? PakCosmetic?.GetItemName() : Data?.Name;
        return name ?? "NULL";
    }

    public async Task LoadAsync()
    {
        if (IsPakCosmetic)
        {
            // We only need to load icon data here
            IconUri = new Uri(await this.CachePakIconAsync() ?? "pack://application:,,,/Images/Icons/T_Ui_Outfit_256.png");
        }
        else
        {
            if (!FortniteCache.TryGetCosmetic(CosmeticId, out var model))
            {
                Log("FortniteApiCache", LogLevel.Error, $"Could not find {CosmeticId} in cache.");
                return;
            }
            
            Data = model;
            Rarity = IsTranscendent(out var channel, out var tag) ? EFortRarity.Transcendent : Data.GetRarity();

            if (Data.TryGetCachedIcon(out var iconUri))
                IconUri = iconUri;
            else
            {
                var uri = Data.GetIcon(channel, tag);
                if (uri == null) // Fallback icon
                {
                    IconUri = new Uri("pack://application:,,,/Images/Icons/T_Ui_Outfit_256.png");
                    return;
                }
            
                await Data.CacheIconAsync(uri);
                IconUri = new Uri(uri);
            }
        }

        ProcessData();
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
    
    private void ProcessData()
    {
        SetRarity(Rarity);
        IconSource = new BitmapImage(IconUri);
    }

    private void SetRarity(EFortRarity rarity)
    {
        var colors = rarity switch
        {
            EFortRarity.Common => new[] { Color.FromRgb(181, 181, 181), Color.FromRgb(158, 158, 158) },
            EFortRarity.Uncommon => new[] { Color.FromRgb(47, 173, 51), Color.FromRgb(57, 150, 60) },
            EFortRarity.Rare => new[] { Color.FromRgb(72, 125, 217), Color.FromRgb(44, 83, 150) },
            EFortRarity.Epic => new[] { Color.FromRgb(133, 63, 224), Color.FromRgb(98, 55, 153) },
            EFortRarity.Legendary => new[] { Color.FromRgb(194, 155, 47), Color.FromRgb(186, 118, 50) },
            EFortRarity.Mythic => new[] { Color.FromRgb(196, 201, 52), Color.FromRgb(173, 164, 61) },
            EFortRarity.Transcendent or EFortRarity.Unattainable => new[]
                { Color.FromRgb(199, 70, 68), Color.FromRgb(138, 39, 39) },
            _ => throw new ArgumentOutOfRangeException(nameof(rarity), rarity, null)
        };

        BackgroundRadial = new RadialGradientBrush(colors[0], colors[1]);
        BorderColor = new SolidColorBrush(colors[0]);
    }
}