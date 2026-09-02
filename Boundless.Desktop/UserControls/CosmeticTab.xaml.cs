using System.Windows.Controls;
using Boundless.Desktop.API;
using Boundless.Desktop.API.Models;
using Boundless.Desktop.Games.EpicGames.Fortnite.Data;
using Boundless.Desktop.Models;
using Boundless.Desktop.Utils;

namespace Boundless.Desktop.UserControls;

public partial class CosmeticTab : UserControl
{
    public CosmeticTab()
    {
        InitializeComponent();
    }

    public async Task ProcessCosmeticsAsync(List<AthenaCosmetic> cosmetics, Action<double> progressCallback, CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

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
            var controls = models.Select(model => new CosmeticItem(model));

            Dispatcher.Invoke(() =>
            {
                foreach (var control in controls)
                {
                    Panel.Children.Add(control);
                }
            });
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
        }
    }
}