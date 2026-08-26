using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Boundless.Desktop.API;
using Boundless.Desktop.API.Models;
using Boundless.Desktop.Games.EpicGames.Fortnite.Data;
using Boundless.Desktop.Models;

namespace Boundless.Desktop.UserControls;

public partial class CosmeticItem : UserControl
{
    private CosmeticItemModel _model;
    
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

    public CosmeticItem(CosmeticItemModel model)
    {
        _model = model;
        InitializeComponent();
        ProcessModel();
    }

    private void ProcessModel()
    {
        SetRarity(_model.Rarity);
        Icon.Source = new BitmapImage(_model.IconUri);
    }

    private void SetIcon(string uri) => Icon.Source = new BitmapImage(new Uri(uri));

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
        Border.BorderBrush = new SolidColorBrush(colors[0]);
    }
}