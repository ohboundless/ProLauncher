using CUE4Parse_Conversion.Textures;
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
    public string ItemName { get; }
    public string ItemDescription { get; }
    private FSoftObjectPath? LargeIcon { get; }
    private FSoftObjectPath? Icon { get; }
    
    public BaseCosmetic(UObject obj)
    {
        Id = obj.Name;
        ItemName = obj.GetOrDefault<FText>(nameof(ItemName)).Text;
        ItemDescription = obj.GetOrDefault<FText>(nameof(ItemDescription)).Text;
        
        var dataList = obj.Get<FInstancedStruct[]>("DataList");
        foreach (var data in dataList)
        {
            if (data.NonConstStruct == null)
                continue;
            
            LargeIcon = data.NonConstStruct.GetOrDefault<FSoftObjectPath>(nameof(LargeIcon));
            Icon = data.NonConstStruct.GetOrDefault<FSoftObjectPath>(nameof(Icon));

            if (LargeIcon != null && Icon != null)
                break;
        }
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