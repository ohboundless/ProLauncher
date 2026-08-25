using System.Windows.Controls;
using Boundless.Desktop.Games.EpicGames.Fortnite.Data;

namespace Boundless.Desktop.UserControls;

public partial class CosmeticTab : UserControl
{
    public CosmeticTab()
    {
        InitializeComponent();
    }

    public async void ProcessCosmeticsAsync(List<AthenaCosmetic> cosmetics)
    {
        Panel.Children.Clear();
        
        // TODO buffer with scroll bar
        // TODO make this a model
        var controls = new List<CosmeticItem>();
        foreach (var cosmetic in cosmetics)
        {
            var control = new CosmeticItem(cosmetic);
            await control.LoadCosmeticAsync();
            controls.Add(control);
        }

       controls = controls.OrderByDescending(x => x.Rarity).ToList();
       foreach (var control in controls)
       {
           Panel.Children.Add(control);
       }
    }
}