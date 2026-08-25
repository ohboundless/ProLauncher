using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Boundless.Desktop.API;
using Boundless.Desktop.API.Models;
using Boundless.Desktop.Games.EpicGames.Fortnite.Data;

namespace Boundless.Desktop.UserControls;

public partial class CosmeticItem : UserControl
{
    private AthenaCosmetic _cosmetic;

    private EFortRarity _rarity;

    public EFortRarity Rarity
    {
        get => _rarity;
        set
        {
            _rarity = value;
            SetRarity(value);
        }
    }

    public CosmeticItem(AthenaCosmetic athenaCosmetic)
    {
        _cosmetic = athenaCosmetic;
        InitializeComponent();
    }

    // TODO make this entire thing a model then process it here
    public async Task LoadCosmeticAsync()
    {
        var id = _cosmetic.GetCosmeticId();
        var model = await FortniteAPI.GetBRCosmeticAsync(id);
        var rarity = IsTranscendent(out var channel, out var tag) ? EFortRarity.Transcendent : model.GetRarity();
        
        // TODO tooltip name
        SetIcon(model.GetIcon(channel, tag));
        Rarity = rarity;
    }

    private void SetIcon(string uri)
    {
        Icon.Source = new BitmapImage(new Uri(uri));
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

        Border.Background = new RadialGradientBrush(colors[0], colors[1]);
        BorderBrush = new SolidColorBrush(colors[0]);
    }

    private bool IsTranscendent(out string? channel, out string? tag)
    {
        var id = _cosmetic.GetCosmeticId();
        if (RareCosmetics.IsRare(id, null, null))
        {
            channel = null;
            tag = null;
            return true;
        }

        var variants = _cosmetic.Attributes?.Variants;
        if (variants != null)
        {
            foreach (var variant in variants)
            {
                foreach (var style in variant.Owned)
                {
                    if (!RareCosmetics.IsRare(id, variant.Channel, style))
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