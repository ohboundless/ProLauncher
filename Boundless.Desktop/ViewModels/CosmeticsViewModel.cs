using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using Boundless.Desktop.FortniteAPI;
using Boundless.Desktop.FortniteAPI.Models;
using Boundless.Desktop.Framework;
using Boundless.Desktop.Games.EpicGames.Fortnite.Data;
using Boundless.Desktop.UserControls.Popups;
using Boundless.Desktop.Utils;

namespace Boundless.Desktop.ViewModels;

public enum CosmeticFilter
{
    None,
    All,
    Outfits,
    Backblings,
    Pickaxes,
    Emotes,
    Gliders,
    Wraps,
    Shoes,
}

public class CosmeticsViewModel : ViewModel
{
    #region Properties
    private CancellationTokenSource? _currentCancellationSource;
    private ObservableCollection<CosmeticItemModel> _cosmetics = new();

    public ObservableCollection<CosmeticItemModel> Cosmetics
    {
        get => _cosmetics;
        set
        {
            _cosmetics = value;
            OnPropertyChanged(nameof(Cosmetics));
            
            CosmeticsView = CollectionViewSource.GetDefaultView(_cosmetics);
            CosmeticsView.Filter = FilterCosmetics;
        }
    }

    private ICollectionView _cosmeticsView;

    public ICollectionView CosmeticsView
    {
        get => _cosmeticsView;
        private set
        {
            _cosmeticsView = value;
            OnPropertyChanged(nameof(CosmeticsView));
        }
    }
    
    private bool _isProcessing;
    public bool IsProcessing
    {
        get => _isProcessing;
        set
        {
            _isProcessing = value;
            OnPropertyChanged();
        }
    }
    
    private string _searchText = string.Empty;
    public string SearchText
    {
        get => _searchText;
        set
        {
            if (_searchText == value)
                return;

            _searchText = value;
            OnPropertyChanged();

            CosmeticsView.Refresh();
        }
    }
    
    private CosmeticFilter _selectedFilter = CosmeticFilter.None;
    public CosmeticFilter SelectedFilter
    {
        get => _selectedFilter;
        set
        {
            if (_selectedFilter == value)
                return;

            _selectedFilter = value;
            OnPropertyChanged(nameof(SelectedFilter));

            OnTabSwitched();
            CosmeticsView.Refresh();
        }
    }
    
    public bool IsForSwapper { get; set; }
    #endregion

    public CosmeticsViewModel()
    {
        CosmeticsView = CollectionViewSource.GetDefaultView(Cosmetics);
        CosmeticsView.Filter = FilterCosmetics;
    }
    
    private bool FilterCosmetics(object obj)
    {
        if (obj is not CosmeticItemModel model)
            return false;

        if (string.IsNullOrWhiteSpace(SearchText))
            return true;

        if (model.Data == null)
        {
            Log("CosmeticsSearch", LogLevel.Warn, $"Cosmetic data was null unable to filter. Returning false... ({model.CosmeticId})");
            return false;
        }
        
        return model.GetItemName().Contains(_searchText, StringComparison.OrdinalIgnoreCase)
               || model.CosmeticId.Contains(_searchText, StringComparison.OrdinalIgnoreCase);
    }
    
    internal CancellationTokenSource HandleCancellation()
    {
        if (_currentCancellationSource != null)
        {
            Log("CosmeticTab", LogLevel.Info, "Requesting cancellation...");
            _currentCancellationSource.Cancel();
            _currentCancellationSource = null;
        }
        
        return _currentCancellationSource = new CancellationTokenSource();
    }
    
    public void OnTabSwitched()
    {
        if (MainWindow.ActiveAccount == null)
            return;

        if (SelectedFilter == CosmeticFilter.None)
            return;
        
        var athenaType = SelectedFilter switch
        {
            CosmeticFilter.All => "*", // All
            CosmeticFilter.Outfits => "AthenaCharacter",
            CosmeticFilter.Backblings => "AthenaBackpack",
            CosmeticFilter.Pickaxes => "AthenaPickaxe",
            CosmeticFilter.Emotes => "AthenaDance",
            CosmeticFilter.Gliders => "AthenaGlider",
            CosmeticFilter.Wraps => "AthenaItemWrap",
            CosmeticFilter.Shoes => "CosmeticShoes",
            _ => throw new ArgumentOutOfRangeException()
        };

        var myCosmetics = IsForSwapper ? null : MainWindow.ActiveAccount.ProfileData?.Cosmetics;
        Action<double> progressCallback = IsForSwapper
            ? MainWindow.SwapperTab.UpdateCosmeticsLoadingProgress
            : MainWindow.AccountCosmeticsTab.UpdateCosmeticsLoadingProgress;
        
        _ = Task.Run(async () =>
        {
            var cancellationToken = HandleCancellation().Token;
            await ProcessCosmeticsAsync(
                athenaType, 
                myCosmetics, 
                progressCallback, 
                cancellationToken);
        });
    }

    private async Task ProcessCosmeticsAsync(string athenaType, List<AthenaCosmetic>? cosmetics, Action<double> progressCallback, CancellationToken cancellationToken)
    {
        while (IsProcessing)
        {
            await Task.Delay(100, cancellationToken);
        }

        IsProcessing = true;

        // Wait for everything to be downloaded.
        await FortniteCache.WaitForCosmetics();
        
        // Load
        if (FortniteCache.Cosmetics == null)
            FortniteCache.Load();

        try
        {
            var models = new List<CosmeticItemModel>();
            var filtered = new List<AthenaCosmetic>();

            if (athenaType != "*")
            {
                if (IsForSwapper)
                {
                    var allCosmetics = FortniteCache.Cosmetics;

                    foreach (var data in allCosmetics)
                    {
                        if (data.Type.BackendValue != athenaType)
                            continue;

                        filtered.Add(new AthenaCosmetic(data));
                    }
                }
                else
                {
                    foreach (var cosmetic in cosmetics)
                    {
                        if (!cosmetic.TemplateId.StartsWith(athenaType))
                            continue;

                        filtered.Add(cosmetic);
                    }
                }

                cosmetics = filtered;
            }

            foreach (var obj in cosmetics)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (IsInvalidCosmetic(obj.TemplateId))
                    continue;

                var model = new CosmeticItemModel(obj);
                await model.LoadAsync();
                progressCallback(Convert.ToDouble(models.Count) / cosmetics.Count);

                if (model.IsNull)
                {
                    Log("CosmeticTab", LogLevel.Error,
                        $"({obj.TemplateId}) Unable to load model. Skipping...");
                    continue;
                }

                models.Add(model);
            }

            models = models.OrderByDescending(x => x.Rarity).ThenBy(x => x.Data?.Type.ParseCosmeticType()).ToList();
            Application.Current.Dispatcher.InvokeAsync(() =>
            {
                Cosmetics = new ObservableCollection<CosmeticItemModel>(models);
                progressCallback(1.0); // Just in case
            });

        }
        catch (OperationCanceledException)
        {
            Log("CosmeticsLoading", LogLevel.Error, "Loading canceled.");
        }
        catch (Exception ex)
        {
            _ = new ErrorPopup("CosmeticsLoading",
                "Error loading cosmetics. If this issue persists please report this to the dev team.", ex);
        }
        finally
        {
            Log("CosmeticsLoading", LogLevel.Info, "Flushing cosmetics cache...");
            FortniteCache.Flush();
            IsProcessing = false;
        }
    }

    private bool IsInvalidCosmetic(string templateId)
    {
        return templateId == "AthenaCharacter:cid_defaultoutfit";
    }
}