using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using Boundless.Desktop.Framework;
using Boundless.Desktop.Models;
using Boundless.Desktop.Utils;

namespace Boundless.Desktop.ViewModels;

public class CosmeticsViewModel: ViewModel
{
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
        
        if (model.Data.Name.Contains(_searchText, StringComparison.OrdinalIgnoreCase)
            || model.CosmeticId.Contains(_searchText, StringComparison.OrdinalIgnoreCase))
            return true;

        return false;
    }
}