using Boundless.Desktop.Config;
using Boundless.Desktop.UserControls.Popups;
using Boundless.Swapper;
using Boundless.Swapper.Cosmetics.Models;
using Boundless.Swapper.Swappers;

namespace Boundless.Desktop.Commands.Swapper;

public static class BackpackSwap
{
    public static void Convert(SwapParameter parameter)
    {
        var character = SC.CosmeticsLoader.LoadCosmetic<CharacterCosmetic>(SwapperData.CharacterId!);
        var backpack = SC.CosmeticsLoader.LoadCosmetic<BackpackCosmetic>(parameter.SwapTo.CosmeticId);
        if (backpack == null)
        {
            parameter.Log("Error!");
            ErrorPopup.Show("BackpackSwap",
                "Unable to find backpack in pak files. If this error persists please report it to the dev team.",
                new ApplicationException($"Could not find backpack ({parameter.SwapTo.CosmeticId})"));
            return;
        }

        var characterParts = character!.BaseCharacterParts.Select(x => x.ToString()).ToList();
        var backpackParts = backpack.CharacterParts.Select(x => x.ToString()).ToArray();
        characterParts.AddRange(backpackParts);
        
        CharacterPartSwapper.SetCharacterParts(characterParts.ToArray());
        parameter.Log("Set character parts.");
        
        CharacterPartSwapper.InvalidateDefault();
        parameter.Log("Invalidated default character.");

        CharacterPartSwapper.InvalidateBackpack();
        parameter.Log("Invalidated default backpack.");
        
        SwapperData.BackpackId = parameter.SwapTo.CosmeticId;
        SwapperData.BackpackCps = backpackParts;        
        SwapperData.Save();
    }
    
    public static void Revert(SwapParameter parameter)
    {
        if (SwapperData.BackpackCps == null)
        {
            parameter.Log("No character to revert.");
            return;
        }
        
        CharacterPartSwapper.RevertBackpack(SwapperData.CharacterCps!);
        parameter.Log("Reverted backbling...");
        
        SwapperData.BackpackId = null;
        SwapperData.BackpackCps = null;
        SwapperData.Save();
    }
}