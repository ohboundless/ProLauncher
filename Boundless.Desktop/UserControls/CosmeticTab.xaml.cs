using System.Windows.Controls;
using Boundless.Desktop.API;
using Boundless.Desktop.API.Models;
using Boundless.Desktop.Games.EpicGames.Fortnite.Data;
using Boundless.Desktop.Models;
using Boundless.Desktop.Utils;

namespace Boundless.Desktop.UserControls;

public partial class CosmeticTab : UserControl
{
    private List<CosmeticItemModel> _unloadedModels = new();
    private List<CosmeticItemModel> _loadedModels = new();
    
    public CosmeticTab()
    {
        InitializeComponent();
    }

    public void LoadCosmeticsBuffer(int size = 50)
    {
        var models = _unloadedModels.Take(size);
        var controls = models.Select(x => new CosmeticItem(x));
        
        Dispatcher.Invoke(() =>
        {
            foreach (var control in controls)
            {
                Panel.Children.Add(control);
            }
        });

        foreach (var model in models)
        {
            _unloadedModels.RemoveAt(0); // Pop
            _loadedModels.Add(model);
        }
    }

    public async void ProcessCosmeticsAsync(List<AthenaCosmetic> cosmetics, CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            foreach (var cosmetic in cosmetics)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    // Clear cache
                    FortniteApiCache.Save();
                    FortniteApiCache.Cosmetics.Clear();
                    
                    cancellationToken.ThrowIfCancellationRequested();
                }

                var model = new CosmeticItemModel(cosmetic);
                await model.LoadAsync();

                if (model.IsNull)
                {
                    Log("CosmeticTab", LogLevel.Error, $"({cosmetic.TemplateId}) Unable to load model. Skipping...");
                    continue;
                }

                _unloadedModels.Add(model);
            }

            _unloadedModels = _unloadedModels.OrderByDescending(x => x.Rarity).ThenBy(x => x.Data?.Type.ParseCosmeticType()).ToList();
            LoadCosmeticsBuffer(150);
            
            // Clear cache
            FortniteApiCache.Save();
            FortniteApiCache.Cosmetics.Clear();
        }
        catch (OperationCanceledException)
        {
            Log("CosmeticsLoading", LogLevel.Error, "Loading canceled...");
        }
        catch (Exception ex)
        {
            Log("CosmeticsLoading", LogLevel.Error, ex.ToString());
            // TODO popup
        }
    }
}