using Boundless.Swapper.Core;
using Boundless.Swapper.Cosmetics.Models;
using CUE4Parse.FileProvider;
using CUE4Parse.FileProvider.Objects;
using CUE4Parse.UE4.Assets;

namespace Boundless.Swapper.Cosmetics;

public class CosmeticsLoader(SwapperCore core)
{
    private DefaultFileProvider _provider => core.CUE4Parse.Provider;

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
            if (kvp.Value.NameWithoutExtension != id)
                continue;

            if (!_provider.TryLoadPackage(kvp.Value, out var pkg))
            {
                // TODO logging
                continue;
            }

            var obj = pkg.GetExport(kvp.Value.NameWithoutExtension);
            var model = Activator.CreateInstance(typeof(T), obj);

            return (T?)model;
        }

        return null;
    }

    public List<BaseCosmetic> LoadCharacters()
    {
        return LoadGeneric<CharacterCosmetic>("AthenaCharacterItemDefinition", CosmeticConstants.CharacterPaths);
    }
}