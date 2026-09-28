using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using System.Windows.Threading;
using Boundless.Desktop.API;
using Boundless.Desktop.API.Models;
using Boundless.Desktop.Framework;
using Boundless.Desktop.Games.EpicGames.Fortnite.Data;
using Boundless.Desktop.Models;
using Boundless.Desktop.Utils;
using Boundless.Swapper;
using Boundless.Swapper.Cosmetics;
using Boundless.Swapper.Cosmetics.Models;
using Globals = Boundless.Swapper.Globals;

namespace Boundless.Desktop.ViewModels;

public class SwapperCosmeticsViewModel() : CosmeticsViewModel
{
    private CosmeticsLoader _loader => Globals.SC.CosmeticsLoader;
    
    private List<BaseCosmetic>? _characters;
    private List<BaseCosmetic>? _backpacks;
    private List<BaseCosmetic>? _pickaxes;
    private List<BaseCosmetic>? _emotes;
    private List<BaseCosmetic>? _shoes;
    
    public override void OnTabSwitched()
    {
        if (MainWindow.ActiveAccount == null)
            return;
        
        var cosmetics = SelectedFilter switch
        {
            CosmeticFilter.Outfits => _characters ??= _loader.LoadCharacters(),
            /*CosmeticFilter.Backblings => _backpacks ??= _loader.LoadCharacters(),
            CosmeticFilter.Pickaxes => _pickaxes ??= _loader.LoadCharacters(),
            CosmeticFilter.Emotes => _emotes ??= _loader.LoadCharacters(),
            CosmeticFilter.Shoes => _shoes ??= _loader.LoadCharacters(),*/
            _ => new()
        };
        
        var cancellationToken = HandleCancellation().Token;
        ProcessCosmeticsAsync(cosmetics, MainWindow.SwapperTab.UpdateCosmeticsLoadingProgress,
            cancellationToken);
    }

    public async Task ProcessCosmeticsAsync(List<BaseCosmetic> cosmetics, Action<double> progressCallback, CancellationToken cancellationToken)
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
                if (cosmetic.Id.ToLower() == "cid_defaultoutfit")
                    continue;
                
                var model = new CosmeticItemModel(cosmetic);
                await model.LoadAsync();
                
                progressCallback(Convert.ToDouble(models.Count) / cosmetics.Count);

                models.Add(model);
                Log("SwapperCosmetics", LogLevel.Info, $"Loaded {cosmetic.Id}");
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
            IsProcessing = false;
        }
    }
}