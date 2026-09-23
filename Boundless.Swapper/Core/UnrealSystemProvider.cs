using System.Data;
using CUE4Parse.FileProvider;
using CUE4Parse.UE4.IO.Objects;
using CUE4Parse.UE4.Readers;
using Serilog;
using UAssetEditor;
using UAssetEditor.Unreal.Assets;
using UAssetEditor.Unreal.Readers.IoStore;
using UAssetEditor.Unreal.Versioning;
using UsmapDotNet;

namespace Boundless.Swapper.Core;

public class UnrealSystemProvider
{
    private DefaultFileProvider _provider;
    private IoGlobalReader _globalReader;
    private Usmap _mappings;
    
    public UnrealSystemProvider(CUE4ParseProvider cue4ParseProvider, string mappingsFilePath)
    {
        _provider = cue4ParseProvider.Provider;
        _globalReader = IoGlobalReader.InitializeGlobalData(cue4ParseProvider.PaksPath + "\\global.utoc", EGame.GAME_UE5_LATEST);
        _mappings = Usmap.Parse(mappingsFilePath);
    }

    public ZenAsset ReadAsset(string assetPath, out string containerPath)
    {
        if (!_provider.TryGetGameFile(assetPath, out var gameFile))
            throw new ApplicationException($"Could not find '{assetPath}'");

        var ioEntry = (FIoStoreEntry)gameFile;
        containerPath = ioEntry.Vfs.Path;
        
        if (!_provider.TrySaveAsset(gameFile, out var bytes))
            throw new ApplicationException($"Could not export '{assetPath}'");

        var asset = new ZenAsset(bytes);
        asset.Game = EGame.GAME_UE5_LATEST;
        asset.Mappings = _mappings;
        asset.GlobalData = _globalReader;

        return asset;
    }
}