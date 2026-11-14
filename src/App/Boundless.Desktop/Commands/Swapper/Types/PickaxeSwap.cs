using Boundless.Desktop.UserControls.Popups;
using Boundless.Swapper.Cosmetics.Models;
using Boundless.Swapper.Swappers;

namespace Boundless.Desktop.Commands.Swapper.Types;

public static class PickaxeSwap
{
    public static void Convert(SwapParameter parameter)
    {
        var fromPickaxe = SC.CosmeticsLoader.LoadCosmetic<PickaxeCosmetic>(parameter.SwapFrom!.CosmeticId.ToLower());
        if (fromPickaxe == null)
        {
            parameter.Log("Error!");
            ErrorPopup.Show("PickaxeSwap",
                "Unable to find pickaxe in pak files. If this error persists please report it to the dev team.",
                new ApplicationException($"Could not find pickaxe ({parameter.SwapFrom.CosmeticId})"));
            return;
        }
        
        var toPickaxe = SC.CosmeticsLoader.LoadCosmetic<PickaxeCosmetic>(parameter.SwapTo.CosmeticId.ToLower());
        if (toPickaxe == null)
        {
            parameter.Log("Error!");
            ErrorPopup.Show("PickaxeSwap",
                "Unable to find pickaxe in pak files. If this error persists please report it to the dev team.",
                new ApplicationException($"Could not find pickaxe ({parameter.SwapTo.CosmeticId})"));
            return;
        }

        parameter.Log("Swapping pickaxe data...");
        
        var fromPath = fromPickaxe.WeaponDefinition.ToString().Split(".")[0] + ".uasset";
        var toPath = toPickaxe.WeaponDefinition.ToString().Split(".")[0] + ".uasset";
        PickaxeSwapper.SwapPickaxe(fromPath, toPath);

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