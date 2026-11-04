using Boundless.Desktop.UserControls.Popups;
using Boundless.Swapper.Cosmetics.Models;
using Boundless.Swapper.Swappers;

namespace Boundless.Desktop.Commands.Swapper;

public class PickaxeSwap
{
    public static void Convert(SwapParameter parameter)
    {
        var fromPickaxe = SC.CosmeticsLoader.LoadCosmetic<PickaxeCosmetic>(parameter.SwapFrom!.CosmeticId);
        if (fromPickaxe == null)
        {
            parameter.Log("Error!");
            ErrorPopup.Show("PickaxeSwap",
                "Unable to find pickaxe in pak files. If this error persists please report it to the dev team.",
                new ApplicationException($"Could not find character ({parameter.SwapFrom.CosmeticId})"));
            return;
        }
        
        var toPickaxe = SC.CosmeticsLoader.LoadCosmetic<PickaxeCosmetic>(parameter.SwapTo.CosmeticId);
        if (toPickaxe == null)
        {
            parameter.Log("Error!");
            ErrorPopup.Show("PickaxeSwap",
                "Unable to find pickaxe in pak files. If this error persists please report it to the dev team.",
                new ApplicationException($"Could not find character ({parameter.SwapTo.CosmeticId})"));
            return;
        }

        parameter.Log("Swapping pickaxe data...");
        PickaxeSwapper.SwapPickaxe(fromPickaxe.WeaponDefinition.ToString(), toPickaxe.WeaponDefinition.ToString());

        SwapperData.PickaxeId = parameter.SwapTo.CosmeticId;
        SwapperData.Save();
    }
    
    public static void Revert(SwapParameter parameter)
    {
        PickaxeSwapper.RevertAll();
        parameter.Log("Reverted pickaxe...");

        SwapperData.PickaxeId = null;
        SwapperData.Save();
    }
}