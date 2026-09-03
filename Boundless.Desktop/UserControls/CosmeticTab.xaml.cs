using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Boundless.Desktop.API;
using Boundless.Desktop.API.Models;
using Boundless.Desktop.Games.EpicGames.Fortnite.Data;
using Boundless.Desktop.Models;
using Boundless.Desktop.Utils;

namespace Boundless.Desktop.UserControls;

public partial class CosmeticTab : UserControl, INotifyPropertyChanged
{
    public static bool IsProcessing;
    
    private ObservableCollection<CosmeticItemModel> _cosmetics = new();

    public ObservableCollection<CosmeticItemModel> Cosmetics
    {
        get => _cosmetics;
        set
        {
            _cosmetics = value;
            OnPropertyChanged(nameof(Cosmetics));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(
        string? propertyName = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propertyName)
        );
    }
    
    public CosmeticTab()
    {
        InitializeComponent();
        DataContext = this;
    }

    public async Task ProcessCosmeticsAsync(List<AthenaCosmetic> cosmetics, Action<double> progressCallback, CancellationToken cancellationToken)
    {
        while (IsProcessing)
        {
            await Task.Delay(100, cancellationToken);
        }

        IsProcessing = true;
        
        try
        {
            var models = new List<CosmeticItemModel>();
            foreach (var cosmetic in cosmetics)
            {
                cancellationToken.ThrowIfCancellationRequested();

                // TODO all weird null cosmetics
                if (cosmetic.TemplateId == "AthenaCharacter:CID_DefaultOutfit")
                    continue;
                
                var model = new CosmeticItemModel(cosmetic);
                await model.LoadAsync();

                if (model.IsNull)
                {
                    Log("CosmeticTab", LogLevel.Error, $"({cosmetic.TemplateId}) Unable to load model. Skipping...");
                    continue;
                }

                models.Add(model);
                progressCallback(Convert.ToDouble(models.Count) / cosmetics.Count);
            }

            models = models.OrderByDescending(x => x.Rarity).ThenBy(x => x.Data?.Type.ParseCosmeticType()).ToList();

            await Dispatcher.InvokeAsync(() =>
            {
                Cosmetics = new ObservableCollection<CosmeticItemModel>(models);
            });

            progressCallback(1.0);
        }
        catch (OperationCanceledException)
        {
            Log("CosmeticsLoading", LogLevel.Error, "Loading canceled.");
        }
        catch (Exception ex)
        {
            Log("CosmeticsLoading", LogLevel.Error, ex.ToString());
            // TODO popup
        }
        finally
        {
            Log("CosmeticsLoading", LogLevel.Error, "Flushing cosmetics cache...");
            FortniteApiCache.Flush();
            IsProcessing = false;
        }
    }

    // From ChatGPT cause I cba
    #region SCROLL_BAR_FIX
    private void ListBox_OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        if (sender is not ListBox listBox)
            return;

        var scrollViewer = FindVisualChild<ScrollViewer>(listBox);

        if (scrollViewer == null)
            return;

        const double scrollAmount = 55;

        if (e.Delta > 0)
            scrollViewer.ScrollToVerticalOffset(
                Math.Max(0, scrollViewer.VerticalOffset - scrollAmount));
        else
            scrollViewer.ScrollToVerticalOffset(
                scrollViewer.VerticalOffset + scrollAmount);

        e.Handled = true;
    }

    private static T? FindVisualChild<T>(DependencyObject obj)
        where T : DependencyObject
    {
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
        {
            var child = VisualTreeHelper.GetChild(obj, i);

            if (child is T result)
                return result;

            var descendant = FindVisualChild<T>(child);

            if (descendant != null)
                return descendant;
        }

        return null;
    }
    #endregion
}