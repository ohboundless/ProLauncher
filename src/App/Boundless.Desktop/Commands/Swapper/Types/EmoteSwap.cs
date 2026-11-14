using Boundless.Desktop.UserControls.Popups;
using Boundless.Swapper.Cosmetics.Models;
using Boundless.Swapper.Swappers;

namespace Boundless.Desktop.Commands.Swapper.Types;

public static class EmoteSwap
{
    public static void Convert(SwapParameter parameter)
    {
        var fromEmote = SC.CosmeticsLoader.LoadCosmetic<EmoteCosmetic>(parameter.SwapFrom!.CosmeticId.ToLower());
        if (fromEmote == null)
        {
            parameter.Log("Error!");
            ErrorPopup.Show("EmoteSwap",
                "Unable to find emote in pak files. If this error persists please report it to the dev team.",
                new ApplicationException($"Could not find emote ({parameter.SwapFrom.CosmeticId})"));
            return;
        }
        
        var toEmote = SC.CosmeticsLoader.LoadCosmetic<EmoteCosmetic>(parameter.SwapTo.CosmeticId.ToLower());
        if (toEmote == null)
        {
            parameter.Log("Error!");
            ErrorPopup.Show("EmoteSwap",
                "Unable to find emote in pak files. If this error persists please report it to the dev team.",
                new ApplicationException($"Could not find emote ({parameter.SwapTo.CosmeticId})"));
            return;
        }

        parameter.Log("Swapping emote data...");
        EmoteSwapper.SwapEmote(fromEmote.Path, toEmote.Path);

        SwapperData.EmoteId = parameter.SwapTo.CosmeticId;
        SwapperData.Save();
    }
    
    public static void Revert(SwapParameter parameter)
    {
        EmoteSwapper.RevertAll();
        parameter.Log("Reverted emote...");

        SwapperData.EmoteId = null;
        SwapperData.Save();
    }
}