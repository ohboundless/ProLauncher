using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.XPath;
using Boundless.Desktop.FortniteAPI;
using Boundless.Desktop.FortniteAPI.Models;
using Boundless.Desktop.Games.EpicGames.Fortnite.Data;
using Boundless.Desktop.Utils;
using Boundless.Desktop.ViewModels.Enums;
using Boundless.Swapper.Cosmetics.Models;
using CUE4Parse.GameTypes.FN.Enums;

namespace Boundless.Desktop.ViewModels;

public class CosmeticItemModel : INotifyPropertyChanged
{
    private readonly Lazy<Task> _loadTask;

    public AthenaCosmetic Cosmetic { get; }
    public string CosmeticId { get; }

    private string _channel;
    private string _tag;
    
    private CosmeticData? _data;

    public CosmeticData? Data
    {
        get => _data;
        private set
        {
            _data = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsNull));
            OnPropertyChanged(nameof(ItemName));
        }
    }

    private EFortRarity _rarity;

    public EFortRarity Rarity
    {
        get => _rarity;
        private set
        {
            _rarity = value;
            OnPropertyChanged();
        }
    }

    private Uri? _iconUri;

    public Uri? IconUri
    {
        get => _iconUri;
        private set
        {
            _iconUri = value;
            OnPropertyChanged();
        }
    }

    private BitmapSource? _iconSource;

    public BitmapSource? IconSource
    {
        get => _iconSource;
        private set
        {
            _iconSource = value;
            OnPropertyChanged();
        }
    }
    
    private BitmapSource? _backgroundSource;

    public BitmapSource? BackgroundSource
    {
        get => _backgroundSource;
        private set
        {
            _backgroundSource = value;
            OnPropertyChanged();
        }
    }

    private Brush? _backgroundRadial;

    public Brush? BackgroundRadial
    {
        get => _backgroundRadial;
        set
        {
            _backgroundRadial = value;
            OnPropertyChanged();
        }
    }

    private Brush? _borderColor;

    public Brush? BorderColor
    {
        get => _borderColor;
        private set
        {
            _borderColor = value;
            OnPropertyChanged();
        }
    }

    public Visibility Visibility { get; set; }

    public bool IsNull => Data == null;
    public PanelType PanelType => Cosmetic.PanelType;
    public string ItemName => Data?.Name ?? "NULL";

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propertyName));
    }

    private string? _series;
    private string? _seriesUriStr;
    public Uri? SeriesUri { get; set; }

    public CosmeticItemModel(AthenaCosmetic cosmetic)
    {
        Cosmetic = cosmetic;
        CosmeticId = cosmetic.GetCosmeticId();
        Data = cosmetic.CosmeticData;
        _loadTask = new Lazy<Task>(LoadCoreAsync);
        
        if (PanelType != PanelType.Swapper)
        {
            if (!FortniteCache.TryGetCosmetic(CosmeticId, out var model))
            {
                Log(
                    "FortniteApiCache",
                    LogLevel.Error,
                    $"Could not find {CosmeticId} in cache.");

                return;
            }

            Data = model;
        }
        
        // Check if this is a rare cosmetic
        var transcendent = IsTranscendent(out _channel, out _tag);
        
        Rarity = transcendent
            ? EFortRarity.Transcendent
            : Data.GetRarity(out _series, out _seriesUriStr);

        SetRarity(Rarity, Data?.Series);
    }

    public string GetItemName() => ItemName;

    public Task LoadAsync() => _loadTask.Value;

    private async Task LoadCoreAsync()
    {
        if (Data.TryGetCachedIcon(_channel, _tag, out var iconUri))
        {
            IconUri = iconUri;
        }
        else
        {
            var uri = Data.GetIcon(_channel, _tag);

            if (uri == null)
            {
                IconUri = new Uri(
                    "pack://application:,,,/Resources/Images/Icons/T_Ui_Outfit_256.png");

                return;
            }

            var fileUri = await Data.CacheIconAsync(uri, _channel, _tag);
            IconUri = new Uri(fileUri);
        }

        if (_seriesUriStr != null)
        {
            var uri = await FortniteCache.CacheImageFromUrlAsync(_seriesUriStr);
            SeriesUri = new Uri(uri);
        }

        await LoadIconAsync();
    }

    private async Task LoadIconAsync()
    {
        if (IconUri == null)
            return;

        var uri = IconUri;
        var iconBitmap = await Task.Run(() =>
        {
            var image = new BitmapImage();

            image.BeginInit();
            image.UriSource = uri;
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.DecodePixelWidth = 256;
            image.EndInit();

            if (image.CanFreeze)
                image.Freeze();

            return image;
        });

        IconSource = iconBitmap;
    }

    private void SetRarity(EFortRarity rarity, CosmeticSeries? series)
    {
        Color[] colors;
        
        // Set series colors if they exist
        if (series?.Colors != null)
        {
            var color1 = $"#{series.Colors.First()[..6]}";
            var color2 = $"#{series.Colors.Last()[..6]}";
            colors =
            [
                (Color)ColorConverter.ConvertFromString(color1), 
                (Color)ColorConverter.ConvertFromString(color2)
            ];
        }
        else
        {
            colors = rarity switch
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
        }

        var radial = new RadialGradientBrush(colors[0], colors[1]);
        radial.Freeze();

        var border = new SolidColorBrush(colors[0]);
        border.Freeze();

        BackgroundRadial = radial;
        BorderColor = border;
    }

    private bool IsTranscendent(out string? channel, out string? tag)
    {
        if (RareCosmetics.IsRare(CosmeticId, null, null))
        {
            channel = null;
            tag = null;
            return true;
        }
        
        if (PanelType == PanelType.AccountCosmetics) // TODO remove when adding styles
        {
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
        }

        channel = null;
        tag = null;
        return false;
    }
}