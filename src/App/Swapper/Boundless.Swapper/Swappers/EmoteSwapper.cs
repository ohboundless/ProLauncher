using System.Data;
using UAssetEditor.Binary;

namespace Boundless.Swapper.Swappers;

public static class EmoteSwapper
{
    public static void SwapEmote(string from, string to)
    {
        var fromAsset = USystem.ReadAsset(from, out var containerPath);
        var toAsset = USystem.ReadAsset(to, out _);

        var fromExportName = fromAsset.Name.Split('/').Last();
        var fromExport = fromAsset.Exports[fromExportName];
        
        var toExportName = toAsset.Name.Split('/').Last();
        var toExport = toAsset.Exports[toExportName];

        if (fromExport == null)
            throw new NoNullAllowedException($"Could not get export using name '{fromExportName}'");
        
        if (toExport == null)
            throw new NoNullAllowedException($"Could not get export using name '{toExportName}'");
        
        // Swap animations
        var properties = new[]
        {
            "Animation",
            "AnimationFemaleOverride",
            "AnimationOverrides",
            //"ItemName",
            //"ItemDescription"
        };

        var fromObj = fromAsset.Exports[0];
        var toObj = toAsset.Exports[0];

        foreach (var property in properties)
        {
            fromObj[property] = toObj[property];
        }
        
        var writer = new Writer();
        fromAsset.WriteAll(writer);
        
        RevertAll();
        SC.LaunchConfig.SaveAssetData(SC.CUE4Parse.Provider.FixPath(from), containerPath, writer.ToArray(), "Emote");
    }

    public static void RevertAll()
    {
        SC.LaunchConfig.RemoveAssetBySwapType("Emote");
    }
}