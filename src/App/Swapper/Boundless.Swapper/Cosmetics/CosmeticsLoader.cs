using Boundless.Logging.Classes;
using Boundless.Swapper.Cosmetics.Models;
using CUE4Parse.FileProvider;
using CUE4Parse.FileProvider.Objects;
using CUE4Parse.UE4.Objects.UObject;
using Newtonsoft.Json;

namespace Boundless.Swapper.Cosmetics;

public class CosmeticsLoader(SwapperCore core)
{
    private DefaultFileProvider _provider => core.CUE4Parse.Provider;

    // We only need to cache pickaxes
    private Dictionary<string, PickaxeCosmetic> _loadedPickaxes = new();
    private Dictionary<string, string>? _pickaxeAbilities;
    private static string PickaxeCacheFilePath = Statics.DataDirectory + "\\Pickaxe_Abilities.json"; // For abilities
    
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

    public async Task LoadSavedPickaxeAbilities()
    {
        Log("CosmeticsLoader_LoadSavedPickaxeAbilities", LogLevel.Info, "Loading saved pickaxe abilities...");
        
        if (_pickaxeAbilities != null)
            return;

        if (!File.Exists(PickaxeCacheFilePath))
        {
            _pickaxeAbilities = new();
            return;
        }

        try
        {
            var text = await File.ReadAllTextAsync(PickaxeCacheFilePath);
            var json = JsonConvert.DeserializeObject<Dictionary<string, string>>(text);

            _pickaxeAbilities = json;
        }
        catch (Exception ex)
        {
            Log("CosmeticsLoader_LoadSavedPickaxeAbilities", LogLevel.Error, $"Unable to load pickaxe abilities!\n{ex}");
        }
    }

    public async Task SavePickaxeAbilities()
    {
        Log("CosmeticsLoader_SavePickaxeAbilities", LogLevel.Info, "Saving pickaxe abilities...");

        if (_pickaxeAbilities == null)
            return;
        
        foreach (var kvp in _loadedPickaxes)
        {
            _pickaxeAbilities[kvp.Key] = kvp.Value.PrimaryFireAbility!;
        }
        
        try
        {
            var json = JsonConvert.SerializeObject(_pickaxeAbilities, Formatting.Indented);
            await File.WriteAllTextAsync(PickaxeCacheFilePath, json);
        }
        catch (Exception ex)
        {
            Log("CosmeticsLoader_SavePickaxeAbilities", LogLevel.Error, $"Unable to save pickaxe abilities!\n{ex}");
        }
    }

    public bool IsPickaxeSwappable(string fromCosmeticId, string toCosmeticId)
    {
        if (fromCosmeticId == toCosmeticId)
            return false;
        
        PickaxeCosmetic? fromCosmetic = null;
        PickaxeCosmetic? toCosmetic = null;
        
        if (!_pickaxeAbilities.TryGetValue(fromCosmeticId, out var fromAbility))
        {
            fromCosmetic = _loadedPickaxes.TryGetValue(fromCosmeticId, out var cosmetic)
                ? cosmetic
                : LoadCosmetic<PickaxeCosmetic>(fromCosmeticId);
        }

        if (!_pickaxeAbilities.TryGetValue(toCosmeticId, out var toAbility))
        {
            toCosmetic = _loadedPickaxes.TryGetValue(toCosmeticId, out var cosmetic2)
                ? cosmetic2
                : LoadCosmetic<PickaxeCosmetic>(toCosmeticId);
        }

        // Check saved abilities
        if (fromAbility != null && toAbility != null)
        {
            return fromAbility == toAbility;
        }

        if (fromCosmetic == null && fromAbility == null 
            || toCosmetic == null && toAbility == null)
            return false;
        
        if (fromAbility == null && fromCosmetic.PrimaryFireAbility == null)
        {
            if (!fromCosmetic.WeaponDefinition.TryLoad(out var fromWid))
                return false;

            fromCosmetic.PrimaryFireAbility = fromWid.GetOrDefault<FSoftObjectPath>("PrimaryFireAbility").ToString();
        }

        if (toAbility == null && toCosmetic.PrimaryFireAbility == null)
        {
            if (!toCosmetic.WeaponDefinition.TryLoad(out var toWid))
                return false;

            toCosmetic.PrimaryFireAbility = toWid.GetOrDefault<FSoftObjectPath>("PrimaryFireAbility").ToString();
        }

        if (fromAbility != null && toAbility == null)
        {
            return fromAbility == toCosmetic.PrimaryFireAbility;
        }

        if (fromAbility == null && toAbility != null)
        {
            return fromCosmetic.PrimaryFireAbility == toAbility;
        }

        return fromCosmetic.PrimaryFireAbility == toCosmetic.PrimaryFireAbility;
    }
}