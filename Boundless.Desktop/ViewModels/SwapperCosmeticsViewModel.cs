using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using Boundless.Desktop.API;
using Boundless.Desktop.API.Models;
using Boundless.Desktop.Framework;
using Boundless.Desktop.Games.EpicGames.Fortnite.Data;
using Boundless.Desktop.Models;
using Boundless.Desktop.Utils;

namespace Boundless.Desktop.ViewModels;

public class SwapperCosmeticsViewModel : CosmeticsViewModel
{
    public void OnTabSwitched()
    {
        if (MainWindow.ActiveAccount == null)
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
            _ => throw new ArgumentOutOfRangeException()
        };

        //_cosmeticTab.Panel.Children.Clear(); // Clear cosmetics
        var myCosmetics = MainWindow.ActiveAccount.ProfileData?.Cosmetics;
        if (myCosmetics == null)
            throw new NoNullAllowedException("Profile data was null.");
        
        var cancellationToken = HandleCancellation().Token;
        if (athenaType == "*") // All
        {
            ProcessCosmeticsAsync(myCosmetics, MainWindow.AccountCosmeticsTab.UpdateCosmeticsLoadingProgress, cancellationToken);
        }
        else
        {
            var cosmeticsToLoad =
                myCosmetics.Where(cosmetic => cosmetic.TemplateId.StartsWith(athenaType)).ToList();
            ProcessCosmeticsAsync(cosmeticsToLoad, MainWindow.AccountCosmeticsTab.UpdateCosmeticsLoadingProgress, cancellationToken);
        }
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
                if (cosmetic.TemplateId == "AthenaCharacter:cid_defaultoutfit")
                    continue;
                
                var model = new CosmeticItemModel(cosmetic);
                await model.LoadAsync();
                progressCallback(Convert.ToDouble(models.Count) / cosmetics.Count);
                
                if (model.IsNull)
                {
                    Log("CosmeticTab", LogLevel.Error, $"({cosmetic.TemplateId}) Unable to load model. Skipping...");
                    continue;
                }

                models.Add(model);
            }

            models = models.OrderByDescending(x => x.Rarity).ThenBy(x => x.Data?.Type.ParseCosmeticType()).ToList();
            Cosmetics = new ObservableCollection<CosmeticItemModel>(models);

            progressCallback(1.0); // Just in case
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
}