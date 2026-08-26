using System.Windows.Controls;
using Boundless.Desktop.Games.EpicGames.Fortnite.Data;
using Boundless.Desktop.Models;

namespace Boundless.Desktop.UserControls;

public partial class CosmeticTab : UserControl
{
    public CosmeticTab()
    {
        InitializeComponent();
    }

    public void ProcessCosmeticsAsync(List<AthenaCosmetic> cosmetics)
    {
        Panel.Children.Clear();

        Task.Run(async () =>
        {
            var models = new List<CosmeticItemModel>();
            foreach (var model in cosmetics.Select(cosmetic => new CosmeticItemModel(cosmetic)))
            {
                await model.LoadAsync();
                models.Add(model);
            }

            models = models.OrderByDescending(x => x.Rarity).ToList();
            var controls = models.Select(model => new CosmeticItem(model));

            Dispatcher.Invoke(() =>
            {
                foreach (var control in controls)
                {
                    Panel.Children.Add(control);
                }
            });
        });
    }
}