using Boundless.Desktop.UserControls.Popups;
using Boundless.Swapper;
using Boundless.Swapper.Cosmetics.Models;
using Boundless.Swapper.Swappers;

namespace Boundless.Desktop.Commands.Swapper;

public static class CharacterSwap
{
    public static void Convert(SwapParameter parameter)
    {
        var character = SC.CosmeticsLoader.LoadCosmetic<CharacterCosmetic>(parameter.SwapTo.CosmeticId);
        if (character == null)
        {
            parameter.Log("Error!");
            _ = new ErrorPopup("CharacterSwap",
                "Unable to find character in pak files. If this error persists please report it to the dev team.",
                new ApplicationException($"Could not find character ({parameter.SwapTo.CosmeticId})"));
            return;
        }
        
        CharacterPartSwapper.SetCharacterParts(character.BaseCharacterParts.Select(x => x.ToString()).ToArray());
        parameter.Log("Set character parts.");
        
        CharacterPartSwapper.InvalidateDefault();
        parameter.Log("Invalidated default character.");
    }
}