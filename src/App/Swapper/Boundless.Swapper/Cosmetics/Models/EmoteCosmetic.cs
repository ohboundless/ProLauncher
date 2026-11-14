using CUE4Parse.UE4.Assets.Exports;
using CUE4Parse.UE4.Objects.UObject;

namespace Boundless.Swapper.Cosmetics.Models;

public class EmoteCosmetic(UObject obj, string path) : BaseCosmetic(obj, path)
{
    // Not needed
    /*public FSoftObjectPath Animation { get; } = obj.GetOrDefault<FSoftObjectPath>(nameof(Animation));
    public FSoftObjectPath AnimationFemaleOverride { get; } = obj.GetOrDefault<FSoftObjectPath>(nameof(AnimationFemaleOverride));*/
}