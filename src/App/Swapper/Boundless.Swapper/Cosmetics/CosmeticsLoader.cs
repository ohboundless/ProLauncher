using Boundless.Swapper.Cosmetics.Models;
using CUE4Parse.FileProvider;
using CUE4Parse.FileProvider.Objects;
using CUE4Parse.UE4.Objects.UObject;

namespace Boundless.Swapper.Cosmetics;

public class CosmeticsLoader(SwapperCore core)
{
    private DefaultFileProvider _provider => core.CUE4Parse.Provider;

    // We only need to cache pickaxes
    private Dictionary<string, PickaxeCosmetic> _loadedPickaxes = new();
    
    public List<BaseCosmetic> LoadGeneric<T>(string exportType, params string[] paths) 
        where T : BaseCosmetic
    {
        var cosmetics = new List<GameFile>();
        
        foreach (var kvp in _provider.Files)
        {
            if (!paths.Any(path => kvp.Key.ToLower().StartsWith(path)))
                continue;
            
            if (kvp.Key.Contains("/Tandem/"))
                continue;
            
            cosmetics.Add(kvp.Value);
        }

        var models = new List<BaseCosmetic>();
        
        foreach (var cosmetic in cosmetics)
        {
            if (!_provider.TryLoadPackage(cosmetic, out var pkg))
            {
                // TODO logging
                continue;
            }

            var obj = pkg.GetExport(cosmetic.NameWithoutExtension);
            if (obj.ExportType != exportType)
                continue;
            
            var model = Activator.CreateInstance(typeof(T), obj);
            models.Add((BaseCosmetic)model);
        }

        return models;
    }

    public T? LoadCosmetic<T>(string id)
        where T : BaseCosmetic
    {
        foreach (var kvp in _provider.Files)
        {
            if (kvp.Value.NameWithoutExtension.ToLower() != id)
                continue;

            if (!_provider.TryLoadPackage(kvp.Value, out var pkg))
            {
                // TODO logging
                continue;
            }

            var obj = pkg.GetExport(kvp.Value.NameWithoutExtension);
            var model = Activator.CreateInstance(typeof(T), obj, kvp.Key);

            if (model is PickaxeCosmetic pickaxe)
                _loadedPickaxes[id] = pickaxe;
            
            return (T?)model;
        }

        return null;
    }

    public bool IsPickaxeSwappable(string fromCosmeticId, string toCosmeticId)
    {
        if (fromCosmeticId == toCosmeticId)
            return false;
        
        var fromCosmetic = _loadedPickaxes.TryGetValue(fromCosmeticId, out var cosmetic)
            ? cosmetic
            : LoadCosmetic<PickaxeCosmetic>(fromCosmeticId);
        
        var toCosmetic = _loadedPickaxes.TryGetValue(toCosmeticId, out var cosmetic2)
            ? cosmetic2
            : LoadCosmetic<PickaxeCosmetic>(toCosmeticId);

        if (fromCosmetic == null || toCosmetic == null)
            return false;
        
        if (fromCosmetic.PrimaryFireAbility == null)
        {
            if (!fromCosmetic.WeaponDefinition.TryLoad(out var fromWid))
                return false;

            fromCosmetic.PrimaryFireAbility = fromWid.GetOrDefault<FSoftObjectPath>("PrimaryFireAbility").ToString();
        }

        if (toCosmetic.PrimaryFireAbility == null)
        {
            if (!toCosmetic.WeaponDefinition.TryLoad(out var toWid))
                return false;

            toCosmetic.PrimaryFireAbility = toWid.GetOrDefault<FSoftObjectPath>("PrimaryFireAbility").ToString();
        }

        return fromCosmetic.PrimaryFireAbility == toCosmetic.PrimaryFireAbility;
    }
}