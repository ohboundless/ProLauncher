using CUE4Parse.UE4.Assets.Exports;
using CUE4Parse.UE4.Objects.UObject;

namespace Boundless.Swapper.Cosmetics.Models;

public class CharacterCosmetic(UObject obj) : BaseCosmetic(obj)
{
    public FSoftObjectPath[] BaseCharacterParts { get; } = obj.GetOrDefault<FSoftObjectPath[]>(nameof(BaseCharacterParts));
}