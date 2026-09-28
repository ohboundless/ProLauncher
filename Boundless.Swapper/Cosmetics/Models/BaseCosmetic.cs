using CUE4Parse_Conversion.Textures;
using CUE4Parse.GameTypes.FN.Enums;
using CUE4Parse.UE4.Assets.Exports;
using CUE4Parse.UE4.Assets.Exports.Texture;
using CUE4Parse.UE4.Assets.Objects;
using CUE4Parse.UE4.Objects.Core.i18N;
using CUE4Parse.UE4.Objects.UObject;
using SkiaSharp;

namespace Boundless.Swapper.Cosmetics.Models;

public abstract class BaseCosmetic
{
    public string Id { get; }
    public FText? ItemName { get; }
    //public FText ItemDescription { get; }
    public EFortRarity? Rarity { get; }
    private FSoftObjectPath? LargeIcon { get; }
    private FSoftObjectPath? Icon { get; }
    
    public BaseCosmetic(UObject obj)
    {
        Id = obj.Name;
        ItemName = obj.GetOrDefault<FText>(nameof(ItemName));
        //ItemDescription = obj.GetOrDefault<FText>(nameof(ItemDescription));
        
        var dataList = obj.Get<FInstancedStruct[]>("DataList");
        foreach (var data in dataList)
        {
            if (LargeIcon != null && Icon != null && Rarity != null)
                break;
            
            if (data.NonConstStruct == null)
                continue;

            if (LargeIcon == null && data.NonConstStruct.TryGetValue(out FSoftObjectPath largeIcon, nameof(LargeIcon)))
                LargeIcon = largeIcon;
                
            if (Icon == null && data.NonConstStruct.TryGetValue(out FSoftObjectPath icon, nameof(Icon)))
                Icon = icon;
            
            if (Rarity == null && data.NonConstStruct.TryGetValue(out EFortRarity rarity, nameof(Rarity)) )
                Rarity = rarity;
        }
    }

    public string GetItemName()
    {
        return ItemName?.ToString() ?? "NULL";
    }

    public SKBitmap? GetLargeIcon()
    {
        return LargeIcon.HasValue ? GetIconFromPath(LargeIcon.Value) : null;
    }
    
    public SKBitmap? GetSmallIcon()
    {
        return Icon.HasValue ? GetIconFromPath(Icon.Value) : null;
    }

    private SKBitmap? GetIconFromPath(FSoftObjectPath path)
    {
        if (!path.TryLoad<UTexture>(out var texture))
        {
            // TODO error handling
            return null;
        }

        return texture.Decode()?.ToSkBitmap();
    }
}