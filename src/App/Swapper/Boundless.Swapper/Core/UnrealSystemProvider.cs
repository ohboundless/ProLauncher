using CUE4Parse.FileProvider;
using CUE4Parse.UE4.IO.Objects;
using UAssetEditor.Unreal.Assets;
using UAssetEditor.Unreal.Readers.IoStore;
using UAssetEditor.Unreal.Versioning;

namespace Boundless.Swapper.Core;

public class UnrealSystemProvider
{
    private DefaultFileProvider _provider;
    private IoGlobalReader _globalReader;
    private MappingsProvider _mappings;
    
    public UnrealSystemProvider(CUE4ParseProvider cue4ParseProvider, MappingsProvider mappings)
    {
        _provider = cue4ParseProvider.Provider;
        _globalReader = IoGlobalReader.InitializeGlobalData(cue4ParseProvider.PaksPath + "\\global.utoc", EGame.GAME_UE5_LATEST);
        _mappings = mappings;
    }

    public byte[] ReadAssetBytes(string assetPath, out string containerPath)
    {
        if (!_provider.TryGetGameFile(assetPath, out var gameFile))
            throw new ApplicationException($"Could not find '{assetPath}'");

        var ioEntry = (FIoStoreEntry)gameFile;
        containerPath = ioEntry.Vfs.Path;
        
        if (!_provider.TrySaveAsset(gameFile, out var bytes))
            throw new ApplicationException($"Could not export '{assetPath}'");

        return bytes;
    }

    public IoAsset ReadAsset(string assetPath, out string containerPath)
    {
        var bytes = ReadAssetBytes(assetPath, out containerPath);
        var asset = new IoAsset(bytes);
        asset.Game = EGame.GAME_UE5_LATEST;
        asset.Mappings = _mappings.Usmap;
        asset.GlobalData = _globalReader;
        asset.ReadAll();
        
        return asset;
    }
}