using Boundless.Desktop.Config;
using Boundless.Desktop.UserControls.Popups;
using Boundless.Swapper;
using Boundless.Swapper.Cosmetics.Models;
using Boundless.Swapper.Swappers;

namespace Boundless.Desktop.Commands.Swapper;

public static class CharacterSwap
{
    public static void Convert(SwapParameter parameter)
    {
        var character = SC.CosmeticsLoader.LoadCosmetic<CharacterCosmetic>(parameter.SwapTo.CosmeticId.ToLower());
        if (character == null)
        {
            parameter.Log("Error!");
            ErrorPopup.Show("CharacterSwap",
                "Unable to find character in pak files. If this error persists please report it to the dev team.",
                new ApplicationException($"Could not find character ({parameter.SwapTo.CosmeticId})"));
            return;
        }

        var parts = character.BaseCharacterParts.Select(x => x.ToString()).ToArray();
        CharacterPartSwapper.SetCharacterParts(parts);
        parameter.Log("Set character parts.");
        
        CharacterPartSwapper.InvalidateDefault();
        parameter.Log("Invalidated default character.");

        SwapperData.CharacterId = parameter.SwapTo.CosmeticId;
        SwapperData.CharacterCps = parts;
        SwapperData.Save();
    }
    
    public static void Revert(SwapParameter parameter)
    {
        CharacterPartSwapper.RevertAll();
        parameter.Log("Reverted outfit and backbling...");
        
        SwapperData.CharacterId = null;
        SwapperData.BackpackId = null;
        SwapperData.CharacterCps = null;
        SwapperData.BackpackCps = null;
        SwapperData.Save();
    }
}