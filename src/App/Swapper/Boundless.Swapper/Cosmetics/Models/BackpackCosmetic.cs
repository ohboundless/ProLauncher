using CUE4Parse.UE4.Assets.Exports;
using CUE4Parse.UE4.Objects.UObject;

namespace Boundless.Swapper.Cosmetics.Models;

public class BackpackCosmetic(UObject obj, string path) : BaseCosmetic(obj, path)
{
    public FSoftObjectPath[] CharacterParts { get; } = obj.GetOrDefault<FSoftObjectPath[]>(nameof(CharacterParts));
}