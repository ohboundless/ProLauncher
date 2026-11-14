using CUE4Parse.UE4.Assets.Exports;
using CUE4Parse.UE4.Objects.UObject;

namespace Boundless.Swapper.Cosmetics.Models;

public class PickaxeCosmetic(UObject obj, string path) : BaseCosmetic(obj, path)
{
    public FSoftObjectPath WeaponDefinition { get; } = obj.GetOrDefault<FSoftObjectPath>(nameof(WeaponDefinition));
}